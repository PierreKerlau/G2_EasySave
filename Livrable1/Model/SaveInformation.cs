using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livrable1.ViewModel;
using Livrable1.View;
using Livrable1.Model;
using System.IO;
using System.Windows;

namespace Livrable1.Model
{
    //---------------------Model---------------------//
    public class SaveInformation
    {
        //---------Variable---------//
        public string NameSave { get; set; } = "";
        public string SourcePath { get; set; } = "";
        public string DestinationPath { get; set; } = "";
        public List<FileInformation> Files { get; set; } = new List<FileInformation>();
        //---------Variable---------//


        //---------Constructeur---------//
        public SaveInformation(string nameSave, string sourcePath, string destinationPath)
        {
            NameSave = nameSave;
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            LoadFiles();
        }
        //---------Constructeur---------//


        //---------Méthodes pour valider les chemins---------//
        public bool ValidatePaths()
        {
            // Vérifier si le chemin source existe
            if (!Directory.Exists(SourcePath))
            {
                MessageBox.Show(LanguageManager.GetText("source_path_not_exists"));
                return false;
            }

            // Vérifier si le chemin de destination existe
            if (!Directory.Exists(DestinationPath))
            {
                MessageBox.Show(LanguageManager.GetText("destination_path_not_exists"));
                return false;
            }

            // Vérifier que le chemin source contient des fichiers
            string[] files = Directory.GetFiles(SourcePath);
            if (files == null || files.Length == 0)
            {
                MessageBox.Show(LanguageManager.GetText("source_path_contains_no_file"));
                return false;
            }

            return true;
        }
        //---------Méthodes pour valider les chemins---------//


        //---------Méthodes pour charger les fichiers---------//
        public void LoadFiles()
        {
            Files.Clear(); // Réinitialise la liste
            if (Directory.Exists(SourcePath))
            {
                string[] filePaths = Directory.GetFiles(SourcePath);
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
    
    //---------------------Model---------------------//
}
