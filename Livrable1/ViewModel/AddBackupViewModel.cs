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

    //---------------------Model---------------------//
    public class SaveInformation
    {
        //---------Variable---------//
        public string NameSave { get; set; } = "";
        public string CheminSource { get; set; } = "";
        public string CheminDestination { get; set; } = "";
        public List<FileInformation> Files { get; set; } = new List<FileInformation>();
        //---------Variable---------//


        //---------Constructeur---------//
        public SaveInformation(string nameSave, string cheminSource, string cheminDestination)
        {
            NameSave = nameSave;
            CheminSource = cheminSource;
            CheminDestination = cheminDestination;
            LoadFiles();
        }
        //---------Constructeur---------//


        //---------Méthodes pour valider les chemins---------//
        public bool ValidatePaths()
        {
            // Vérifier si le chemin source existe
            if (!Directory.Exists(CheminSource))
            {
                MessageBox.Show("Le chemin source n'existe pas.");
                return false;
            }

            // Vérifier si le chemin de destination existe
            if (!Directory.Exists(CheminDestination))
            {
                MessageBox.Show("Le chemin de destination n'existe pas.");
                return false;
            }

            // Vérifier que le chemin source contient des fichiers
            string[] files = Directory.GetFiles(CheminSource);
            if (files == null || files.Length == 0)
            {
                MessageBox.Show("Le chemin source ne contient aucun fichier.");
                return false;
            }

            return true;
        }
        //---------Méthodes pour valider les chemins---------//


        //---------Méthodes pour charger les fichiers---------//
        public void LoadFiles()
        {
            Files.Clear(); // Réinitialise la liste
            if (Directory.Exists(CheminSource))
            {
                string[] filePaths = Directory.GetFiles(CheminSource);
                foreach (string filePath in filePaths)
                {
                    Files.Add(new FileInformation(filePath));
                }
            }
        }
        //---------Méthodes pour charger les fichiers---------//


        //---------Méthodes pour sélectionner les fichiers---------//
        public void SetSelectedFiles(List<FileInformation> selectedFiles)
        {
            Files = selectedFiles;
        }
        //---------Méthodes pour sélectionner les fichiers---------//

    }
    //---------------------------//
    public class FileInformation
    {
        //---------Variable---------//
        public string FilePath { get; set; }
        public string FileName => Path.GetFileName(FilePath);
        public bool IsSelected { get; set; } // Pour suivre la sélection
        //---------Variable---------//

        //---------Constructeur---------//
        public FileInformation(string filePath)
        {
            FilePath = filePath;
        }
        //---------Constructeur---------//

    }
    //---------------------Model---------------------//


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
