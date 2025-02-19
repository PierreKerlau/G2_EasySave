using System;
using System.Collections.ObjectModel;
using System.IO;
using Livrable1.Model;
using Livrable1.ViewModel;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using Livrable1.logger;

//---------------------ViewModel---------------------//
namespace Livrable1.ViewModel
{
    //------------Class ExecuteBackupViewModel------------//
    public class ExecuteBackupViewModel
    {
        // Collection of saved backups
        public ObservableCollection<SaveInformation> Backups { get; set; }

        // Constructor: initializes the backup list from SaveManager
        public ExecuteBackupViewModel()
        {
            Backups = new ObservableCollection<SaveInformation>(SaveManager.Instance.GetBackups());
        }

        // Method to execute a backup based on the selected type
        public void ExecuteBackup(SaveInformation backup, string backupType)
        {
            if (backupType == "Sauvegarde complète") // Full backup
            {
                ExecuteFullBackup(backup);
            }
            else if (backupType == "Sauvegarde différentielle") // Differential backup
            {
                ExecuteDifferentialBackup(backup);
            }
        }

        // Method to execute a full backup
        private void ExecuteFullBackup(SaveInformation backup)
        {
            try
            {
                // Create backup directory
                string backupFolder = Path.Combine(backup.CheminDestination, backup.NameSave);
                Directory.CreateDirectory(backupFolder);

                foreach (var file in backup.Files)
                {
                    try
                    {
                        string destFile = Path.Combine(backupFolder, file.FileName);
                        string extension = Path.GetExtension(file.FilePath).ToLower();
                        var stopwatch = Stopwatch.StartNew();
                        long cryptingTime = 0;

                        // Check if the file should be encrypted based on its extension
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
                                // Retrieve encryption key from environment variables
                                string key = Environment.GetEnvironmentVariable("EASYSAVE_CRYPTO_KEY");

                                // Call external encryption software
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
                                MessageBox.Show($"Erreur lors du cryptage : {ex.Message}");
                            }
                            cryptingTime = stopwatch.ElapsedMilliseconds;
                        }
                        else
                        {
                            // Copy file without encryption
                            File.Copy(file.FilePath, destFile, true);
                        }
                        
                        stopwatch.Stop();
                        long transferTime = stopwatch.ElapsedMilliseconds;

                        // Log the backup operation
                        Logger logger = new Logger();
                        logger.LogBackupOperation(backup.NameSave, file.FilePath, destFile, 
                            new FileInfo(file.FilePath).Length, transferTime, cryptingTime, StateViewModel.IsJsonOn);
                    }
                    catch (Exception fileEx)
                    {
                        MessageBox.Show($"Erreur sur le fichier {file.FileName} : {fileEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur générale : {ex.Message}");
            }
        }

        // Method to execute a differential backup (only modified files)
        private void ExecuteDifferentialBackup(SaveInformation backup)
        {
            try
            {
                // Create backup directory
                string backupFolder = Path.Combine(backup.CheminDestination, backup.NameSave);
                Directory.CreateDirectory(backupFolder);

                foreach (var file in backup.Files)
                {
                    string destFile = Path.Combine(backupFolder, file.FileName);

                    // Only copy if the file does not exist or has been modified
                    if (!File.Exists(destFile) || File.GetLastWriteTime(file.FilePath) > File.GetLastWriteTime(destFile))
                    {
                        CopyOrEncryptFile(file.FilePath, destFile);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde différentielle : {ex.Message}");
            }
        }

        // Method to copy or encrypt files based on their type
        private void CopyOrEncryptFile(string sourceFile, string destinationFile)
        {
            string extension = Path.GetExtension(sourceFile).ToLower();

            // Determine if encryption is required
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
                    // Retrieve encryption key
                    string key = Environment.GetEnvironmentVariable("EASYSAVE_CRYPTO_KEY");
                    using (Process cryptoProcess = new Process())
                    {
                        // Encrypt file using external software
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
                // Copy file without encryption
                File.Copy(sourceFile, destinationFile, true);
            }
        }
    }
    //------------Class ExecuteBackupViewModel------------//
}
//---------------------ViewModel---------------------//