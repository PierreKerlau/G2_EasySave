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

//---------------------ViewModel---------------------//
namespace Livrable1.ViewModel
{
    //------------Class AddSaveViewModel------------//   
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

            Backups.Add(backup); // Add backup to the list
            CreationLogsSave.WriteState(Backups);
            // Save the backup in SaveManager to make it visible in ExecuteBackup
            SaveManager.Instance.AddBackup(backup);

            return true; // Return true if backup is successfully added
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
    //------------Class AddSaveViewModel------------//
}
//---------------------ViewModel---------------------//