//-------------------------------------------------------------------------------------------------------------------------------------------//

using Livrable1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Livrable1.ViewModel;
using Livrable1.View;
using System.ComponentModel;
using System.IO;
using System.Windows;
using static System.Net.WebRequestMethods;
using System.Collections.ObjectModel;
using System.Windows.Media;

//-------------------------------------------------------------------------------------------------------------------------------------------//

namespace Livrable1.ViewModel
{

    


    //---------------------ViewModel---------------------//
    public class AddSaveViewModel
    {
        public AddSaveViewModel() { }

        public List<SaveInformation> Backups { get; set; } = new List<SaveInformation>();

        public bool AddSaveMethod(SaveInformation backup)
        {
            // Vérifier que la sauvegarde est valide avant de l'ajouter
            if (!backup.ValidatePaths())
            {
                return false;
            }

            Backups.Add(backup);

            // ✅ Enregistrer la sauvegarde dans SaveManager pour qu'elle soit visible dans ExecuteBackup
            SaveManager.Instance.AddBackup(backup);

            return true;
        }

        public ObservableCollection<FileInformation> Files { get; set; } = new ObservableCollection<FileInformation>();

        public void LoadFilesFromSource(string sourcePath)
        {
            Files.Clear(); // Nettoie la liste avant de la remplir

            if (Directory.Exists(sourcePath))
            {
                var filePaths = Directory.GetFiles(sourcePath);
                foreach (var filePath in filePaths)
                {
                    Files.Add(new FileInformation(filePath));
                }
            }
        }

        public void SaveSelectedFiles(SaveInformation save)
        {
            // Filtrer les fichiers sélectionnés
            var selectedFiles = Files.Where(f => f.IsSelected).ToList();

            // Associer les fichiers sélectionnés à la sauvegarde
            save.SetSelectedFiles(selectedFiles);
        }
    }
    //---------------------ViewModel---------------------//
}
