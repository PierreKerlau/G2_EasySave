using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Windows;

//---------------------Model---------------------//
namespace Livrable1.Model
{
    public class SaveInformation : INotifyPropertyChanged
    {
        public string NameSave { get; set; } = "";
        // Property to hold the source path
        public string SourcePath { get; set; } = "";
        // Property to hold the destination path
        public string DestinationPath { get; set; } = "";
        // List to hold file information
        public List<FileInformation> Files { get; set; } = new List<FileInformation>();
        public bool IsSelected { get; set; }

        private int _progression;
        public int Progression
        {
            get => _progression;
            set
            {
                if (_progression != value)
                {
                    _progression = value;
                    OnPropertyChanged(nameof(Progression));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SaveInformation(string nameSave, string cheminSource, string cheminDestination)
            NameSave = nameSave;
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            LoadFiles();
        }

        public bool ValidatePaths()
        {
            if (!Directory.Exists(CheminSource))
            {
                MessageBox.Show(LanguageManager.GetText("source_path_not_exists"));
                return false;
            }

            if (!Directory.Exists(CheminDestination))
            {
                MessageBox.Show(LanguageManager.GetText("destination_path_not_exists"));
                return false;
            }

            string[] files = Directory.GetFiles(CheminSource);
            if (files == null || files.Length == 0)
            {
                MessageBox.Show(LanguageManager.GetText("source_path_contains_no_file"));
                return false;
            }

            return true; 
        }

        public void LoadFiles()
        {
            Files.Clear();
            if (Directory.Exists(CheminSource))
            {
                string[] filePaths = Directory.GetFiles(SourcePath);
                foreach (string filePath in filePaths)
                {
                    Files.Add(new FileInformation(filePath)); 
                }
            }
        }
  
        public void SetSelectedFiles(List<FileInformation> selectedFiles)
        {
            Files = selectedFiles; 
        }
    }
}
