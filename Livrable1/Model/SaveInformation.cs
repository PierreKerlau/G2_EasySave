using Livrable1.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Livrable1.Model
{
    //------------Class SaveInformation------------//
    public class SaveInformation : INotifyPropertyChanged
    {
        // Property change notification
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Properties for Save Information
        public string NameSave { get; set; } = "";
        public string SourcePath { get; set; } = "";
        public string DestinationPath { get; set; } = "";

        public int NumberFile { get; set; } = 0;
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = false;
        public long TotalSize { get; set; } = 0;
        public int RemainingFiles { get; set; } = 0;
        public long RemainingSize { get; set; } = 0;

        // List to hold file information
        public List<FileInformation> Files { get; set; } = new List<FileInformation>();

        // Progression tracking property with INotifyPropertyChanged support
        private double _progression;
        public double Progression
        {
            get { return _progression; }
            set
            {
                if (_progression != value)
                {
                    _progression = value;
                    OnPropertyChanged(nameof(Progression));
                }
            }
        }

        // Property for selecting a backup job in UI (checkbox in DataGrid)
        public bool IsSelected { get; set; }

        // Constructor
        public SaveInformation(string nameSave, string sourcePath, string destinationPath, int numberFile = 0, DateTime date = default, bool isActive = false, long totalSize = 0, int remainingFiles = 0, long remainingSize = 0)
        {
            NameSave = nameSave;
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            NumberFile = numberFile;
            Date = date == default ? DateTime.Now : date; // Use current date if no date is provided
            IsActive = isActive;
            TotalSize = totalSize;
            RemainingFiles = remainingFiles;
            RemainingSize = remainingSize;

            LoadFiles(); // Load files during initialization
        }

        // Validate the paths for source and destination
        public bool ValidatePaths()
        {
            if (!Directory.Exists(SourcePath))
            {
                MessageBox.Show(LanguageManager.GetText("source_path_not_exists"));
                return false;
            }

            if (!Directory.Exists(DestinationPath))
            {
                MessageBox.Show(LanguageManager.GetText("destination_path_not_exists"));
                return false;
            }

            string[] files = Directory.GetFiles(SourcePath);
            if (files == null || files.Length == 0)
            {
                MessageBox.Show(LanguageManager.GetText("source_path_contains_no_file"));
                return false;
            }

            return true;
        }

        // Load files from the source directory
        public void LoadFiles()
        {
            Files.Clear();
            if (Directory.Exists(SourcePath))
            {
                string[] filePaths = Directory.GetFiles(SourcePath);
                foreach (string filePath in filePaths)
                {
                    Files.Add(new FileInformation(filePath)); // Add each file to the list
                }
            }
        }

        // Set the selected files for backup
        public void SetSelectedFiles(List<FileInformation> selectedFiles)
        {
            Files = selectedFiles; // Update the list with selected files
        }
    }
    //------------Class SaveInformation------------//
}
