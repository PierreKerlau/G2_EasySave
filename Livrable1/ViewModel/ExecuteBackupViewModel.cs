using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Livrable1.Model;

namespace Livrable1.ViewModel
{
    public class ExecuteBackupViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<SaveInformation> Backups { get; set; }
        private readonly Dictionary<string, CancellationTokenSource> _cancellationTokens = new();
        private readonly Dictionary<string, ManualResetEventSlim> _pauseEvents = new();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ExecuteBackupViewModel()
        {
            Backups = new ObservableCollection<SaveInformation>(SaveManager.Instance.GetBackups());
        }

        public void ExecuteSelectedBackups(string backupType)
        {
            var selectedBackups = Backups.Where(b => b.IsSelected).ToList();

            foreach (var backup in selectedBackups)
            {
                ExecuteBackup(backup, backupType);
            }
        }

        public void ExecuteBackup(SaveInformation backup, string backupType)
        {
            if (!_pauseEvents.ContainsKey(backup.NameSave))
                _pauseEvents[backup.NameSave] = new ManualResetEventSlim(true);

            var cts = new CancellationTokenSource();
            _cancellationTokens[backup.NameSave] = cts;

            Task.Run(() =>
            {
                try
                {
                    if (backupType == "Sauvegarde complète")
                    {
                        ExecuteFullBackup(backup, cts.Token);
                    }
                    else if (backupType == "Sauvegarde différentielle")
                    {
                        ExecuteDifferentialBackup(backup, cts.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show($"Sauvegarde annulée pour {backup.NameSave}.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}");
                }
                finally
                {
                    _cancellationTokens.Remove(backup.NameSave);
                    _pauseEvents.Remove(backup.NameSave);
                    cts.Dispose();
                }
            }, cts.Token);
        }

        private void ExecuteFullBackup(SaveInformation backup, CancellationToken token)
        {
            string backupFolder = Path.Combine(backup.CheminDestination, backup.NameSave);
            Directory.CreateDirectory(backupFolder);

            int totalFiles = backup.Files.Count;
            int filesCopied = 0;

            foreach (var file in backup.Files)
            {
                token.ThrowIfCancellationRequested();
                _pauseEvents[backup.NameSave].Wait(token);

                string destFile = Path.Combine(backupFolder, file.FileName);
                CopyOrEncryptFile(file.FilePath, destFile);

                filesCopied++;
                backup.Progression = (int)((double)filesCopied / totalFiles * 100);
            }
        }

        private void ExecuteDifferentialBackup(SaveInformation backup, CancellationToken token)
        {
            string backupFolder = Path.Combine(backup.CheminDestination, backup.NameSave);
            Directory.CreateDirectory(backupFolder);

            int totalFiles = backup.Files.Count;
            int filesCopied = 0;

            foreach (var file in backup.Files)
            {
                token.ThrowIfCancellationRequested();
                _pauseEvents[backup.NameSave].Wait(token);

                string destFile = Path.Combine(backupFolder, file.FileName);

                if (!File.Exists(destFile) || File.GetLastWriteTime(file.FilePath) > File.GetLastWriteTime(destFile))
                {
                    CopyOrEncryptFile(file.FilePath, destFile);
                    filesCopied++;
                    backup.Progression = (int)((double)filesCopied / totalFiles * 100);
                }
            }
        }

        public void PauseBackup(SaveInformation backup)
        {
            if (_pauseEvents.ContainsKey(backup.NameSave))
            {
                _pauseEvents[backup.NameSave].Reset();
            }
        }

        public void ResumeBackup(SaveInformation backup)
        {
            if (_pauseEvents.ContainsKey(backup.NameSave))
            {
                _pauseEvents[backup.NameSave].Set();
            }
        }

        public void StopBackup(SaveInformation backup)
        {
            SaveManager.Instance.StopBackup(backup);
            if (_cancellationTokens.ContainsKey(backup.NameSave))
            {
                _cancellationTokens[backup.NameSave].Cancel();
            }
        }

        public void DeleteBackup(SaveInformation backup)
        {
            SaveManager.Instance.DeleteBackup(backup);
            Backups.Remove(backup);
        }

        private void CopyOrEncryptFile(string sourceFile, string destinationFile)
        {
            string extension = Path.GetExtension(sourceFile).ToLower();
            bool shouldEncrypt = (StateViewModel.IsPdfEnabled && extension == ".pdf") ||
                                (StateViewModel.IsTxtEnabled && extension == ".txt") ||
                                (StateViewModel.IsPngEnabled && extension == ".png") ||
                                (StateViewModel.IsJsonEnabled && extension == ".json") ||
                                (StateViewModel.IsXmlEnabled && extension == ".xml") ||
                                (StateViewModel.IsDocxEnabled && extension == ".docx");

            if (shouldEncrypt)
            {
                try
                {
                    string key = Environment.GetEnvironmentVariable("EASYSAVE_CRYPTO_KEY");
                    using (Process cryptoProcess = new Process())
                    {
                        cryptoProcess.StartInfo.FileName = "CryptoSoft.exe";
                        cryptoProcess.StartInfo.Arguments = $"\"{sourceFile}\" \"{destinationFile}\" \"{key}\"";
                        cryptoProcess.StartInfo.UseShellExecute = true;
                        cryptoProcess.Start();
                        cryptoProcess.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors du cryptage : {ex.Message}");
                }
            }
            else
            {
                File.Copy(sourceFile, destinationFile, true);
            }
        }
    }
}
