using System;
using System.Collections.ObjectModel;
using System.IO;
using Livrable1.Model;
using Livrable1.Controller;
using System.Linq;

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
                    string destFile = Path.Combine(backup.CheminDestination, file.FileName);
                    File.Copy(file.FilePath, destFile, true);
                }

                Console.WriteLine($"Sauvegarde complète terminée pour {backup.NameSave}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la sauvegarde complète : {ex.Message}");
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
                        File.Copy(file.FilePath, destFile, true);
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
