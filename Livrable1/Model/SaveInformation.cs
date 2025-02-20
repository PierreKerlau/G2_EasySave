using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Livrable1.Model
{
    public class SaveInformation : INotifyPropertyChanged
    {
        public string NameSave { get; set; } = "";
        public string CheminSource { get; set; } = "";
        public string CheminDestination { get; set; } = "";
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
        {
            NameSave = nameSave;
            CheminSource = cheminSource;
            CheminDestination = cheminDestination;
            LoadFiles();
        }

        public bool ValidatePaths()
        {
            if (!Directory.Exists(CheminSource))
            {
                MessageBox.Show("Le chemin source n'existe pas.");
                return false;
            }

            if (!Directory.Exists(CheminDestination))
            {
                MessageBox.Show("Le chemin de destination n'existe pas.");
                return false;
            }

            string[] files = Directory.GetFiles(CheminSource);
            if (files == null || files.Length == 0)
            {
                MessageBox.Show("Le chemin source ne contient aucun fichier.");
                return false;
            }

            return true;
        }

        public void LoadFiles()
        {
            Files.Clear();
            if (Directory.Exists(CheminSource))
            {
                string[] filePaths = Directory.GetFiles(CheminSource);
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