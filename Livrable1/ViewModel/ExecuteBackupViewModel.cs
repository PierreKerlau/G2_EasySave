using System;
using System.Collections.ObjectModel;
using System.IO;
using Livrable1.Model;
using Livrable1.ViewModel;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using Livrable1.logger;

namespace Livrable1.ViewModel
{
    public class ExecuteBackupViewModel
    {
        public ObservableCollection<SaveInformation> Backups { get; set; }

        public ExecuteBackupViewModel()
        {
            Backups = new ObservableCollection<SaveInformation>(SaveManager.Instance.GetBackups());
        }

        public void ExecuteBackup(SaveInformation backup, string backupType)
        {
            if (backupType == "Full Backup")
            {
                ExecuteFullBackup(backup);
            }
            else if (backupType == "Differential Backup")
            {
                ExecuteDifferentialBackup(backup);
            }
        }

        private void ExecuteFullBackup(SaveInformation backup)
        {
            try
            {
                string backupFolder = Path.Combine(backup.DestinationPath, backup.NameSave);
                Directory.CreateDirectory(backupFolder);

                foreach (var file in backup.Files)
                {
                    try
                    {
                        string destFile = Path.Combine(backupFolder, file.FileName);
                        string extension = Path.GetExtension(file.FilePath).ToLower();
                        var stopwatch = Stopwatch.StartNew();
                        long cryptingTime = 0;

                        bool shouldEncrypt = StateViewModel.IsPdfEnabled && extension == ".pdf" ||
                                           StateViewModel.IsTxtEnabled && extension == ".txt" ||
                                           StateViewModel.IsPngEnabled && extension == ".png" ||
                                           StateViewModel.IsJsonEnabled && extension == ".json" ||
                                           StateViewModel.IsXmlEnabled && extension == ".xml" ||
                                           StateViewModel.IsDocxEnabled && extension == ".docx";

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
                                    cryptoProcess.StartInfo.RedirectStandardError = false;

                                    cryptoProcess.Start();
                                    cryptoProcess.WaitForExit();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"{LanguageManager.GetText("error_encryption")}: {ex.Message}");
                            }
                            cryptingTime = stopwatch.ElapsedMilliseconds;
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
                    catch (Exception fileEx)
                    {
                        MessageBox.Show($"{LanguageManager.GetText("error_on_file")} {file.FileName} : {fileEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageManager.GetText("error_general")}: {ex.Message}");
            }
        }

        private void ExecuteDifferentialBackup(SaveInformation backup)
        {
            try
            {
                string backupFolder = Path.Combine(backup.DestinationPath, backup.NameSave);
                Directory.CreateDirectory(backupFolder);

                foreach (var file in backup.Files)
                {
                    string destFile = Path.Combine(backupFolder, file.FileName);

                    if (!File.Exists(destFile) || File.GetLastWriteTime(file.FilePath) > File.GetLastWriteTime(destFile))
                    {
                        CopyOrEncryptFile(file.FilePath, destFile);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageManager.GetText("error_during_diff_backup")}: {ex.Message}");
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
                    MessageBox.Show($"{LanguageManager.GetText("error_during_encryption")}: {ex.Message}");
                }
            }
            else
            {
                File.Copy(sourceFile, destinationFile, true);
            }
        }
    }
}
