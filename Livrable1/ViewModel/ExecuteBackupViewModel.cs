﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using Livrable1.Model;
using Livrable1.ViewModel;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Livrable1.ViewModel
{
    public class ExecuteBackupViewModel : INotifyPropertyChanged
    {
        // Collection of saved backups
        public ObservableCollection<SaveInformation> Backups { get; set; }
        private readonly Dictionary<string, CancellationTokenSource> _cancellationTokens = new();
        private readonly Dictionary<string, ManualResetEventSlim> _pauseEvents = new();
        EtatSauvegarde StateSave { get; set; } = new EtatSauvegarde();

        public event PropertyChangedEventHandler PropertyChanged;

        // Constructor: initializes the backup list from SaveManager
        public ExecuteBackupViewModel()
        {
            Backups = new ObservableCollection<SaveInformation>(SaveManager.Instance.GetBackups());
            LoadData();
        }

        public void LoadData()
        {
            StateSave LoadSaves();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Method to execute a backup based on the selected type
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
                    if (backupType == "Sauvegarde complète" || backupType == LanguageManager.GetText("combobox_full_backup"))
                    {
                        ExecuteFullBackup(backup, cts.Token);
                    }
                    else if (backupType == "Sauvegarde différentielle" || backupType == LanguageManager.GetText("combobox_differential_backup"))
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

        

        // Method to execute a full backup
        private void ExecuteFullBackup(SaveInformation backup, CancellationToken token)
        {
            string backupFolder = Path.Combine(backup.DestinationPath, backup.NameSave);
            Directory.CreateDirectory(backupFolder);
            long totalSize = backup.Files.Sum(f => new FileInfo(f.FilePath).Length);
            long copiedSize = 0;

            foreach (var file in backup.Files)
            {
                token.ThrowIfCancellationRequested();
                _pauseEvents[backup.NameSave].Wait(token);

                string destFile = Path.Combine(backupFolder, file.FileName);
                CopyOrEncryptFile(file.FilePath, destFile);

                copiedSize += new FileInfo(file.FilePath).Length;
                backup.Progression = (int)((double)copiedSize / totalSize * 100);
                OnPropertyChanged(nameof(backup.Progression));
            }
        }

        // Method to execute a differential backup (only modified files)
        private void ExecuteDifferentialBackup(SaveInformation backup, CancellationToken token)
        {
            string backupFolder = Path.Combine(backup.DestinationPath, backup.NameSave);
            Directory.CreateDirectory(backupFolder);
            long totalSize = backup.Files.Sum(f => new FileInfo(f.FilePath).Length);
            long copiedSize = 0;

            foreach (var file in backup.Files)
            {
                token.ThrowIfCancellationRequested();
                _pauseEvents[backup.NameSave].Wait(token);

                string destFile = Path.Combine(backupFolder, file.FileName);

                if (!File.Exists(destFile) || File.GetLastWriteTime(file.FilePath) > File.GetLastWriteTime(destFile))
                {
                    CopyOrEncryptFile(file.FilePath, destFile);
                    copiedSize += new FileInfo(file.FilePath).Length;
                }

                backup.Progression = (int)((double)copiedSize / totalSize * 100);
                OnPropertyChanged(nameof(backup.Progression));
            }
        }

        // Method to copy or encrypt files based on their type
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

        // Methods for pause, resume, and stop functionality
        public void PauseBackup(SaveInformation backup)
        {
            _pauseEvents[backup.NameSave].Reset();
        }

        public void ResumeBackup(SaveInformation backup)
        {
            _pauseEvents[backup.NameSave].Set();
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
    }
}
