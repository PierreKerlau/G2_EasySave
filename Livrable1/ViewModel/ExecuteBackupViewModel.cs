﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using Livrable1.Model;
using Livrable1.ViewModel;
using System.Linq;
using System.Windows;
using System.Diagnostics;

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
            if (backupType == "Sauvegarde complète")
            {
                ExecuteFullBackup(backup);
            }
            else if (backupType == "Sauvegarde différentielle")
            {
                ExecuteDifferentialBackup(backup);
            }
        }

        private void ExecuteFullBackup(SaveInformation backup)
        {
            try
            {
                string backupFolder = Path.Combine(backup.CheminDestination, backup.NameSave);
                Directory.CreateDirectory(backupFolder);

                foreach (var file in backup.Files)
                {
                    try
                    {
                        string destFile = Path.Combine(backupFolder, file.FileName);
                        CopyOrEncryptFile(file.FilePath, destFile);
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

        private void ExecuteDifferentialBackup(SaveInformation backup)
        {
            try
            {
                string backupFolder = Path.Combine(backup.CheminDestination, backup.NameSave);
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
                MessageBox.Show($"Erreur lors de la sauvegarde différentielle : {ex.Message}");
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
