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

//---------------------Model---------------------//
namespace Livrable1.Model
{
    //------------Class SaveInformation------------//
    public class SaveInformation
    {
        // Property to hold the name of the save
        public string NameSave { get; set; } = "";

        // Property to hold the source path
        public string CheminSource { get; set; } = "";

        // Property to hold the destination path
        public string CheminDestination { get; set; } = "";

        // List to hold file information
        public List<FileInformation> Files { get; set; } = new List<FileInformation>();

        // Constructor to initialize the save information and load files
        public SaveInformation(string nameSave, string cheminSource, string cheminDestination)
        {
            NameSave = nameSave;
            CheminSource = cheminSource;
            CheminDestination = cheminDestination;
            LoadFiles();
        }

        // Method to validate the source and destination paths
        public bool ValidatePaths()
        {
            // Check if the source directory exists
            if (!Directory.Exists(CheminSource))
            {
                MessageBox.Show("Le chemin source n'existe pas.");
                return false;
            }

            // Check if the destination directory exists
            if (!Directory.Exists(CheminDestination))
            {
                MessageBox.Show("Le chemin de destination n'existe pas.");
                return false;
            }

            // Get all files in the source directory
            string[] files = Directory.GetFiles(CheminSource);
            if (files == null || files.Length == 0)
            {
                MessageBox.Show("Le chemin source ne contient aucun fichier.");
                return false;
            }

            return true; // Paths are valid
        }

        // Method to load files from the source directory
        public void LoadFiles()
        {
            Files.Clear(); // Resets the file list
            if (Directory.Exists(CheminSource))
            {
                string[] filePaths = Directory.GetFiles(CheminSource);
                foreach (string filePath in filePaths)
                {
                    Files.Add(new FileInformation(filePath)); // Add each file to the list
                }
            }
        }

        // Method to set selected files
        public void SetSelectedFiles(List<FileInformation> selectedFiles)
        {
            Files = selectedFiles; // Update the list of files with the selected files
        }
    }
    //------------Class SaveInformation------------//
}
//---------------------Model---------------------//