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
    
    //---------------------Model---------------------//
}
