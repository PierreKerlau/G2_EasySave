using System.IO;
using System.Windows.Controls;
using System.Windows;
using Livrable1.View;
using Livrable1.Model;
using Livrable1.ViewModel;
using System.Collections.ObjectModel;
using System.Diagnostics;

//---------------------ViewModel---------------------//
namespace Livrable1.ViewModel
{
    //------------Class BackupFileViewModel------------//
    public class BackupFileViewModel
    {
        public string FileName { get; set; }
        public bool IsSelected { get; set; } // Property indicating whether the file is selected
    }
    //------------Class BackupFileViewModel------------//

    //------------Class RecoverBackupViewModel------------//
    internal class RecoverBackupViewModel
    {
        public ObservableCollection<SaveInformation> Backups { get; set; }
        public ObservableCollection<BackupFileViewModel> FilesToRecover { get; set; }

        public RecoverBackupViewModel()
        {
            Backups = new ObservableCollection<SaveInformation>(SaveManager.Instance.GetBackups());
            FilesToRecover = new ObservableCollection<BackupFileViewModel>();
            LoadSaves();
        }

        public void LoadSaves()
        {
            // Vérifier si le chemin source existe
            try
            {
                string jsonFilePath = "../../../Logs/state.json"; // Remplace par le chemin de ton fichier JSON

                if (System.IO.File.Exists(jsonFilePath))
                {
                    var saveList = EtatSauvegarde.ReadState(jsonFilePath);
                    foreach (var save in saveList)
                    {
                        // Vérifier si la sauvegarde existe déjà dans la collection
                        if (!Backups.Any(b => b.NameSave == save.NameSave))
                        {
                            Backups.Add(save); // Ajouter seulement si elle n'existe pas déjà
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la liste : {ex.Message}");
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
                        // Copy all the contents of the backup folder to the source folder
                        CopyDirectory(backupFolder, sourceDirectory, true);
                        MessageBox.Show($"{LanguageManager.GetText("file_has")} '{selectedBackup.NameSave}' {LanguageManager.GetText("been_success_recover_complete")}");
                    }
                    // Differential backup: copy only the most recent files
                    else if (backupType == "differential")
                    {
                        // Copy all the contents of the backup folder to the source folder, but check the modification dates
                        CopyDirectory(backupFolder, sourceDirectory, false);
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

            try
            {
                // Select only files marked for recovery in the GUI
                var selectedFiles = FilesToRecover
                    .Where(f => f.IsSelected)
                    .Select(f => f.FileName) // Assume that the file name is in the CheckBox content
                    .ToList();

                foreach (var fileName in selectedFiles)
                {
                    string sourceFile = Path.Combine(sourceDirectory, fileName);
                    string recoverPath = Path.Combine(destinationFolder, fileName);
                    string extension = Path.GetExtension(fileName).ToLower();

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
                                cryptoProcess.StartInfo.Arguments = $"\"{recoverPath}\" \"{sourceFile}\" \"{key}\"";
                                cryptoProcess.StartInfo.UseShellExecute = true;
                                cryptoProcess.StartInfo.RedirectStandardError = false;

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
                        File.Copy(recoverPath, sourceFile, true);
                        MessageBox.Show($"{LanguageManager.GetText("simple_copy_of")} '{fileName}'");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageManager.GetText("error_encryption")}: {ex.Message}");
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

                // For differential backup, only copy if the source file is older.
                if (!overwrite)
                {
                    if (File.Exists(destFile))
                    {
                        DateTime sourceLastModified = File.GetLastWriteTime(file);
                        DateTime destLastModified = File.GetLastWriteTime(destFile);

                        if (sourceLastModified > destLastModified)
                        {
                            // Copy only if the file is more recent
                            File.Copy(file, destFile, true);
                        }
                    }
                    else
                    {
                        // Copy if the file does not exist in the destination folder
                        File.Copy(file, destFile, true);
                    }
                }
                else
                {
                    // Copy all files unconditionally for full backup
                    File.Copy(file, destFile, true);
                }
            }

            // Copy sub-folders recursively
            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                string subDirDest = Path.Combine(destDir, Path.GetFileName(subDir));
                CopyDirectory(subDir, subDirDest, overwrite);
            }
        }
    }
    //------------Class RecoverBackupViewModel------------//
}
//---------------------ViewModel---------------------//
