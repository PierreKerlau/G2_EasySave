using System.IO;
using System.Windows.Controls;
using System.Windows;
using Livrable1.View;
using Livrable1.Model;
using Livrable1.ViewModel;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Livrable1.ViewModel
{
    public class BackupFileViewModel
    {
        public string FileName { get; set; }
        public bool IsSelected { get; set; } // Property indicating whether the file is selected
    }
    internal class RecoverBackupViewModel
    {
        public ObservableCollection<SaveInformation> Backups { get; set; }
        public ObservableCollection<BackupFileViewModel> FilesToRecover { get; set; }

        public RecoverBackupViewModel()
        {
            Backups = new ObservableCollection<SaveInformation>(SaveManager.Instance.GetBackups());
            FilesToRecover = new ObservableCollection<BackupFileViewModel>();
            LoadSaves(); // Load saved backup information
        }

        public void LoadSaves()
        {
            // Check if the source path exists
            try
            {
                string jsonFilePath = "../../../Logs/state.json"; // Path to JSON file

                if (System.IO.File.Exists(jsonFilePath))
                {
                    var saveList = EtatSauvegarde.ReadState(jsonFilePath);
                    foreach (var save in saveList)
                    {
                        // Add only if the backup does not already exist in the collection
                        if (!Backups.Any(b => b.NameSave == save.NameSave))
                        {
                            Backups.Add(save);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageManager.GetText("error_charging_list")}: {ex.Message}"); // Show error message
            }
        }

        // Recover selected files (from destination folder to source folder)
        public void RecoverBackup(SaveInformation selectedBackup, string backupType)
        {
            string sourceDirectory = selectedBackup.SourcePath;   // Source folder where files can be recovered
            string destinationFolder = selectedBackup.DestinationPath;  // Destination folder from which to retrieve files

            string backupFolder = Path.Combine(destinationFolder, selectedBackup.NameSave);

            try
            {
                if (Directory.Exists(backupFolder))  // Checks whether the backup folder exists
                {
                    // Full backup: copy the entire contents of the backup folder to the source folder
                    if (backupType == "full")
                    {
                        CopyDirectory(backupFolder, sourceDirectory, true); // Copy all contents
                        MessageBox.Show($"{LanguageManager.GetText("file_has")} '{selectedBackup.NameSave}' {LanguageManager.GetText("been_success_recover_complete")}");
                    }
                    // Differential backup: copy only the most recent files
                    else if (backupType == "differential")
                    {
                        CopyDirectory(backupFolder, sourceDirectory, false); // Check modification dates
                        MessageBox.Show($"{LanguageManager.GetText("file_has")} '{selectedBackup.NameSave}' {LanguageManager.GetText("been_success_recover_diff")}");
                    }
                }
                else
                {
                    MessageBox.Show($"{LanguageManager.GetText("destination_directory")} '{backupFolder}' {LanguageManager.GetText("does_not_exist")}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageManager.GetText("error_during_recovery")} {ex.Message}");
            }
        }

        // Copy or decrypt a file based on its type
        private void CopyOrDecryptFile(string sourceFile, string destinationFile)
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
                        cryptoProcess.StartInfo.FileName = "CryptoSoft.exe"; // CryptoSoft application for encryption
                        cryptoProcess.StartInfo.Arguments = $"\"{sourceFile}\" \"{destinationFile}\" \"{key}\"";
                        cryptoProcess.StartInfo.UseShellExecute = true;
                        cryptoProcess.Start();
                        cryptoProcess.WaitForExit(); // Wait for the encryption process to finish
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{LanguageManager.GetText("error_encryption")}: {ex.Message}");
                }
            }
            else
            {
                File.Copy(sourceFile, destinationFile, true); // Copy file without encryption
            }
        }

        // Function for copying the entire contents of one folder to another, with management of recent files for differential backup
        private void CopyDirectory(string sourceDir, string destDir, bool overwrite)
        {
            // Create the destination folder if it does not exist
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            // Copy all files from the source folder to the destination folder
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));

                // For differential backup, only copy if the source file is newer
                if (!overwrite)
                {
                    if (File.Exists(destFile))
                    {
                        DateTime sourceLastModified = File.GetLastWriteTime(file);
                        DateTime destLastModified = File.GetLastWriteTime(destFile);

                        if (sourceLastModified > destLastModified)
                        {
                            CopyOrDecryptFile(file, destFile); // Copy only if the file is more recent
                        }
                    }
                    else
                    {
                        CopyOrDecryptFile(file, destFile); // Copy if the file does not exist in the destination folder
                    }
                }
                else
                {
                    CopyOrDecryptFile(file, destFile); // Copy all files unconditionally for full backup
                }
            }

            // Copy sub-folders recursively
            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                string subDirDest = Path.Combine(destDir, Path.GetFileName(subDir));
                CopyDirectory(subDir, subDirDest, overwrite); // Recursively copy sub-directories
            }
        }
    }
}