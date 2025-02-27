using Livrable1.Model;
using Livrable1.ViewModel;
using Livrable1.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.IO;
using System.Windows;
using static System.Net.WebRequestMethods;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Xml.Linq;
using System.Text.Json;

namespace Livrable1.ViewModel
{
    public class AddSaveViewModel
    {
        // Constructor for AddSaveViewModel
        public AddSaveViewModel()
        {
        }

        // List to store backups
        public List<SaveInformation> Backups { get; set; } = new List<SaveInformation>();

        public EtatSauvegarde CreationLogsSave { get; set; } = new EtatSauvegarde();

        // Method to add a new backup
        public bool AddSaveMethod(SaveInformation backup)
        {
            // Validate the backup paths before adding it
            if (!backup.ValidatePaths())
            {
                return false; // Return false if validation fails
            }

            // Calculate the total size of selected files
            backup.RemainingSize = backup.Files.Sum(file => new FileInfo(file.FilePath).Length);
            backup.TotalSize = backup.Files.Sum(file => new FileInfo(file.FilePath).Length);

            Backups.Add(backup); // Add backup to the list
            CreationLogsSave.WriteState(Backups); // Write the current state of backups

            // Save the backup in SaveManager to make it visible in ExecuteBackup
            SaveManager.Instance.AddBackup(backup);

            return true; // Return true if backup is successfully added
        }

        // Method to check if the backup name already exists
        public bool VerifAddName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var savedState = EtatSauvegarde.ReadState("../../../Logs/state.json"); // Read the JSON file

                // Check if the name already exists in the backups
                return savedState.Any(s => s.NameSave == name);
            }

            return false; // Return false if the name is empty or null
        }

        // Collection to store file information
        public ObservableCollection<FileInformation> Files { get; set; } = new ObservableCollection<FileInformation>();

        // Method to load files from the source path
        public void LoadFilesFromSource(string sourcePath)
        {
            Files.Clear(); // Clear the list before populating it

            // Check if the source path exists
            if (Directory.Exists(sourcePath))
            {
                var filePaths = Directory.GetFiles(sourcePath); // Get all files in the source directory
                foreach (var filePath in filePaths)
                {
                    Files.Add(new FileInformation(filePath)); // Add each file to the collection
                }
            }
        }

        // Method to save selected files to a specific backup
        public void SaveSelectedFiles(SaveInformation save)
        {
            // Filter selected files
            var selectedFiles = Files.Where(f => f.IsSelected).ToList();

            // Associate the selected files with the backup
            save.SetSelectedFiles(selectedFiles);
        }
    }
}