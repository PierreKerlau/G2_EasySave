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
        public string SourcePath { get; set; } = "";
        // Property to hold the destination path
        public string DestinationPath { get; set; } = "";

        //---

        public int NumberFile { get; set; } = 0;

        public DateTime Date {get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = false;

        public long TotalSize { get; set; } = 0;

        public int RemainingFiles { get; set; } = 0;

        public long RemainingSize { get; set; } = 0;
        //---


        // List to hold file information
        public List<FileInformation> Files { get; set; } = new List<FileInformation>();

        // Constructor to initialize the save information and load files
        public SaveInformation(string nameSave, string sourcePath, string destinationPath, int numberFile, DateTime date, bool isActive, long totalSize, int remainingFiles, long remainingSize)
        {
            NameSave = nameSave;
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            //---
            NumberFile = numberFile;
            Date = date;
            IsActive = isActive;
            TotalSize = totalSize;
            RemainingFiles = remainingFiles;
            RemainingSize = remainingSize;
            //---
            LoadFiles();
        }

        // Method to validate the source and destination paths
        public bool ValidatePaths()
        {
            // Check if the source directory exists
            if (!Directory.Exists(SourcePath))
            {
                MessageBox.Show(LanguageManager.GetText("source_path_not_exists"));
                return false;
            }

            // Check if the destination directory exists
            if (!Directory.Exists(DestinationPath))
            {
                MessageBox.Show(LanguageManager.GetText("destination_path_not_exists"));
                return false;
            }

            // Get all files in the source directory
            string[] files = Directory.GetFiles(SourcePath);
            if (files == null || files.Length == 0)
            {
                MessageBox.Show(LanguageManager.GetText("source_path_contains_no_file"));
                return false;
            }

            return true; // Paths are valid
        }

        // Method to load files from the source directory
        public void LoadFiles()
        {
            Files.Clear(); // Resets the file list
            if (Directory.Exists(SourcePath))
            {
                string[] filePaths = Directory.GetFiles(SourcePath);
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