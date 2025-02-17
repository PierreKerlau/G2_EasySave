using System;
using System.Collections.ObjectModel;
using System.IO;
using Livrable1.Model;
using Livrable1.ViewModel;
using System.Linq;
using EasySave.Cryptography;
using System.Windows;

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
                if (!Directory.Exists(backup.CheminDestination))
                {
                    Directory.CreateDirectory(backup.CheminDestination);
                }

                foreach (var file in backup.Files)
                {
                    try
                    {
                        string destFile = Path.Combine(backup.CheminDestination, file.FileName);
                        string extension = Path.GetExtension(file.FilePath);
                        
                        MessageBox.Show($"Traitement du fichier : {file.FileName}\n" +
                                      $"Extension : {extension}\n" +
                                      $"Chemin source : {file.FilePath}\n" +
                                      $"Chemin destination : {destFile}");

                        if (CryptoManager.ShouldEncrypt(file.FilePath))
                        {
                            MessageBox.Show($"Tentative de cryptage pour {file.FileName}");
                            CryptoManager.EncryptFile(file.FilePath, destFile);
                            MessageBox.Show($"Cryptage terminé pour {file.FileName}");
                        }
                        else
                        {
                            MessageBox.Show($"Copie simple pour {file.FileName}");
                            File.Copy(file.FilePath, destFile, true);
                        }
                    }
                    catch (Exception fileEx)
                    {
                        MessageBox.Show($"Erreur sur le fichier {file.FileName} : {fileEx.Message}\n" +
                                      $"Stack trace : {fileEx.StackTrace}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur générale : {ex.Message}\n" +
                               $"Stack trace : {ex.StackTrace}");
            }
        }

        private void ExecuteDifferentialBackup(SaveInformation backup)
        {
            try
            {
                if (!Directory.Exists(backup.CheminDestination))
                {
                    Directory.CreateDirectory(backup.CheminDestination);
                }

                foreach (var file in backup.Files)
                {
                    string destFile = Path.Combine(backup.CheminDestination, file.FileName);

                    if (!File.Exists(destFile) || File.GetLastWriteTime(file.FilePath) > File.GetLastWriteTime(destFile))
                    {
                        // Vérifier si le fichier doit être crypté
                        if (CryptoManager.ShouldEncrypt(file.FilePath))
                        {
                            MessageBox.Show($"Cryptage activé pour {file.FileName}");
                            CryptoManager.EncryptFile(file.FilePath, destFile);
                        }
                        else
                        {
                            MessageBox.Show($"Pas de cryptage pour {file.FileName}");
                            File.Copy(file.FilePath, destFile, true);
                        }
                    }
                }

                Console.WriteLine($"Sauvegarde différentielle terminée pour {backup.NameSave}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la sauvegarde différentielle : {ex.Message}");
            }
        }
    }
}
