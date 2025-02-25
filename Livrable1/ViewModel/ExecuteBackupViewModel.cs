using System;
using System.Collections.ObjectModel;
using System.IO;
using Livrable1.Model;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Livrable1.logger;

namespace Livrable1.ViewModel
{
    public class ExecuteBackupViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<SaveInformation> Backups { get; set; }
        private readonly Dictionary<string, CancellationTokenSource> _cancellationTokens = new();
        private readonly Dictionary<string, ManualResetEventSlim> _pauseEvents = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ExecuteBackupViewModel()
        {
            Backups = new ObservableCollection<SaveInformation>(SaveManager.Instance.GetBackups());
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                    string englishBackupType = ConvertBackupType(backupType);
                    bool hasPriorityFiles = PriorityExtensionManager.Instance.HasPriorityFiles(backup.SourcePath);
                    
                    if (hasPriorityFiles)
                    {
                        ExecutePriorityBackup(backup, englishBackupType, cts.Token);
                    }
                    else
                    {
                        if (englishBackupType == "Full Backup")
                        {
                            ExecuteFullBackup(backup, cts.Token);
                        }
                        else if (englishBackupType == "Differential Backup")
                        {
                            ExecuteDifferentialBackup(backup, cts.Token);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show($"Backup cancelled for {backup.NameSave}.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Backup error: {ex.Message}");
                }
                finally
                {
                    _cancellationTokens.Remove(backup.NameSave);
                    _pauseEvents.Remove(backup.NameSave);
                    cts.Dispose();
                }
            }, cts.Token);
        }

        private string ConvertBackupType(string backupType)
        {
            if (backupType == LanguageManager.GetText("combobox_full_backup"))
                return "Full Backup";
            if (backupType == LanguageManager.GetText("combobox_differential_backup"))
                return "Differential Backup";
            return backupType;
        }

        private void ExecutePriorityBackup(SaveInformation backup, string backupType, CancellationToken token)
        {
            string backupFolder = Path.Combine(backup.DestinationPath, backup.NameSave);
            Directory.CreateDirectory(backupFolder);

            // Séparer les fichiers en deux groupes
            var priorityFiles = backup.Files
                .Where(f => PriorityExtensionManager.Instance.PriorityExtensions
                    .Contains(Path.GetExtension(f.FilePath).ToLower()))
                .ToList();

            var nonPriorityFiles = backup.Files
                .Where(f => !PriorityExtensionManager.Instance.PriorityExtensions
                    .Contains(Path.GetExtension(f.FilePath).ToLower()))
                .ToList();

            foreach (var file in priorityFiles)
            {
                token.ThrowIfCancellationRequested();
                _pauseEvents[backup.NameSave].Wait(token);

                string destFile = Path.Combine(backupFolder, file.FileName);
                var stopwatch = Stopwatch.StartNew();
                long cryptingTime = 0;

                string extension = Path.GetExtension(file.FilePath).ToLower();
                bool shouldEncrypt = (StateViewModel.IsPdfEnabled && extension == ".pdf") ||
                                   (StateViewModel.IsTxtEnabled && extension == ".txt") ||
                                   (StateViewModel.IsPngEnabled && extension == ".png") ||
                                   (StateViewModel.IsJsonEnabled && extension == ".json") ||
                                   (StateViewModel.IsXmlEnabled && extension == ".xml") ||
                                   (StateViewModel.IsDocxEnabled && extension == ".docx") ||
                                   (StateViewModel.IsMkvEnabled && extension == ".mkv") ||
                                   (StateViewModel.IsJpgEnabled && extension == ".jpg");

                if (shouldEncrypt)
                {
                    try
                    {
                        string key = Environment.GetEnvironmentVariable("EASYSAVE_CRYPTO_KEY");
                        using (Process cryptoProcess = new Process())
                        {
                            cryptoProcess.StartInfo.FileName = "CryptoSoft.exe";
                            cryptoProcess.StartInfo.Arguments = $"\"{file.FilePath}\" \"{destFile}\" \"{key}\"";
                            cryptoProcess.StartInfo.UseShellExecute = true;
                            cryptoProcess.Start();
                            cryptoProcess.WaitForExit();
                        }
                        cryptingTime = stopwatch.ElapsedMilliseconds;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{LanguageManager.GetText("error_encryption")}: {ex.Message}");
                    }
                }
                else
                {
                    File.Copy(file.FilePath, destFile, true);
                }

                stopwatch.Stop();
                long transferTime = stopwatch.ElapsedMilliseconds;

                Logger logger = new Logger();
                logger.LogBackupOperation(backup.NameSave, file.FilePath, destFile,
                    new FileInfo(file.FilePath).Length, transferTime, cryptingTime, StateViewModel.IsJsonOn);
            }

            // Puis traiter les fichiers non prioritaires de la même manière
            foreach (var file in nonPriorityFiles)
            {
                token.ThrowIfCancellationRequested();
                _pauseEvents[backup.NameSave].Wait(token);

                string destFile = Path.Combine(backupFolder, file.FileName);
                var stopwatch = Stopwatch.StartNew();
                long cryptingTime = 0;

                string extension = Path.GetExtension(file.FilePath).ToLower();
                bool shouldEncrypt = (StateViewModel.IsPdfEnabled && extension == ".pdf") ||
                                   (StateViewModel.IsTxtEnabled && extension == ".txt") ||
                                   (StateViewModel.IsPngEnabled && extension == ".png") ||
                                   (StateViewModel.IsJsonEnabled && extension == ".json") ||
                                   (StateViewModel.IsXmlEnabled && extension == ".xml") ||
                                   (StateViewModel.IsDocxEnabled && extension == ".docx") ||
                                   (StateViewModel.IsMkvEnabled && extension == ".mkv") ||
                                   (StateViewModel.IsJpgEnabled && extension == ".jpg");

                if (shouldEncrypt)
                {
                    try
                    {
                        string key = Environment.GetEnvironmentVariable("EASYSAVE_CRYPTO_KEY");
                        using (Process cryptoProcess = new Process())
                        {
                            cryptoProcess.StartInfo.FileName = "CryptoSoft.exe";
                            cryptoProcess.StartInfo.Arguments = $"\"{file.FilePath}\" \"{destFile}\" \"{key}\"";
                            cryptoProcess.StartInfo.UseShellExecute = true;
                            cryptoProcess.Start();
                            cryptoProcess.WaitForExit();
                        }
                        cryptingTime = stopwatch.ElapsedMilliseconds;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{LanguageManager.GetText("error_encryption")}: {ex.Message}");
                    }
                }
                else
                {
                    File.Copy(file.FilePath, destFile, true);
                }

                stopwatch.Stop();
                long transferTime = stopwatch.ElapsedMilliseconds;

                Logger logger = new Logger();
                logger.LogBackupOperation(backup.NameSave, file.FilePath, destFile,
                    new FileInfo(file.FilePath).Length, transferTime, cryptingTime, StateViewModel.IsJsonOn);
            }
        }

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
                var stopwatch = Stopwatch.StartNew();
                
                CopyOrEncryptFile(file.FilePath, destFile);
                
                stopwatch.Stop();
                long transferTime = stopwatch.ElapsedMilliseconds;

                copiedSize += new FileInfo(file.FilePath).Length;
                backup.Progression = (int)((double)copiedSize / totalSize * 100);
                OnPropertyChanged(nameof(backup.Progression));

                // Log the backup operation
                Logger logger = new Logger();
                logger.LogBackupOperation(backup.NameSave, file.FilePath, destFile,
                    new FileInfo(file.FilePath).Length, transferTime, 0, StateViewModel.IsJsonOn);
            }
        }

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
                    var stopwatch = Stopwatch.StartNew();
                    
                    CopyOrEncryptFile(file.FilePath, destFile);
                    
                    stopwatch.Stop();
                    long transferTime = stopwatch.ElapsedMilliseconds;

                    copiedSize += new FileInfo(file.FilePath).Length;
                    
                    // Log the backup operation
                    Logger logger = new Logger();
                    logger.LogBackupOperation(backup.NameSave, file.FilePath, destFile,
                        new FileInfo(file.FilePath).Length, transferTime, 0, StateViewModel.IsJsonOn);
                }

                backup.Progression = (int)((double)copiedSize / totalSize * 100);
                OnPropertyChanged(nameof(backup.Progression));
            }
        }

        private void CopyOrEncryptFile(string sourceFile, string destinationFile)
        {
            string extension = Path.GetExtension(sourceFile).ToLower();
            bool shouldEncrypt = (StateViewModel.IsPdfEnabled && extension == ".pdf") ||
                                 (StateViewModel.IsTxtEnabled && extension == ".txt") ||
                                 (StateViewModel.IsPngEnabled && extension == ".png") ||
                                 (StateViewModel.IsJsonEnabled && extension == ".json") ||
                                 (StateViewModel.IsXmlEnabled && extension == ".xml") ||
                                 (StateViewModel.IsDocxEnabled && extension == ".docx") ||
                                 (StateViewModel.IsMkvEnabled && extension == ".mkv") ||
                                 (StateViewModel.IsJpgEnabled && extension == ".jpg");

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
                    MessageBox.Show($"{LanguageManager.GetText("error_encryption")}: {ex.Message}");
                }
            }
            else
            {
                File.Copy(sourceFile, destinationFile, true);
            }
        }

        public void PauseBackup(SaveInformation backup)
        {
            if (_pauseEvents.ContainsKey(backup.NameSave))
            {
                _pauseEvents[backup.NameSave].Reset();
            }
            SaveManager.Instance.PauseBackup(backup);
        }

        public void ResumeBackup(SaveInformation backup)
        {
            if (_pauseEvents.ContainsKey(backup.NameSave))
            {
                _pauseEvents[backup.NameSave].Set();
            }
            SaveManager.Instance.ResumeBackup(backup);
        }

        public void StopBackup(SaveInformation backup)
        {
            if (_cancellationTokens.ContainsKey(backup.NameSave))
            {
                _cancellationTokens[backup.NameSave].Cancel();
            }
            SaveManager.Instance.StopBackup(backup);
        }

        public void DeleteBackup(SaveInformation backup)
        {
            SaveManager.Instance.DeleteBackup(backup);
            Backups.Remove(backup);
        }
    }
}
