using System;
using System.Collections.Generic;
using System.IO; 
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Livrable1.View
{
    /// <summary>
    /// Logique d'interaction pour ViewRecoverBackup.xaml
    /// </summary>
    public partial class ViewRecoverBackup : Window
    {
        private List<Backup> backupJobs; // List of backup jobs
        public ViewRecoverBackup() 
        {
            InitializeComponent();
            backupJobs = GetBackupJobs(); // Charge backup jobs
            DisplayBackupJobs(); // Display backup jobs in Checkboxes
        }

        // Method to get back backup jobs list
        private List<Backup> GetBackupJobs()
        {
            return new List<Backup>
            {
                new Backup { Name = "test.docx", DestinationPath = "D:\\CESI\\FISA A3 INFO\\Semestre 5\\Bloc 2 - Génie logiciel\\Livrables\\Livrable 1\\Destination", SourcePath = "D:\\CESI\\FISA A3 INFO\\Semestre 5\\Bloc 2 - Génie logiciel\\Livrables\\Livrable 1\\Source" },
                new Backup { Name = "test.xlsx", DestinationPath = "D:\\CESI\\FISA A3 INFO\\Semestre 5\\Bloc 2 - Génie logiciel\\Livrables\\Livrable 1\\Destination", SourcePath = "D:\\CESI\\FISA A3 INFO\\Semestre 5\\Bloc 2 - Génie logiciel\\Livrables\\Livrable 1\\Source" }
            };
        }

        // Display backup jobs in Checkboxes
        private void DisplayBackupJobs()
        {
            foreach (var backup in backupJobs)
            {
                CheckBox checkBox = new CheckBox
                {
                    Content = backup.Name,
                    Tag = backup // Add the backup in the Tag propriety to recover the object latter 
                };
                BackupCheckboxesPanel.Children.Add(checkBox); // Add every checkbox in the Grid
            }
        }

        // Validation Button : run the recovery
        private void ButtonValidate_Click(object sender, RoutedEventArgs e)
        {
            // Checks whether a backup has been selected
            var selectedBackups = BackupCheckboxesPanel.Children.OfType<CheckBox>()
                .Where(cb => cb.IsChecked == true)
                .Select(cb => cb.Tag as Backup)
                .ToList();

            if (selectedBackups.Count == 0)
            {
                MessageBox.Show("Please select a backup type.");
                return;
            }

            // Checks which type of backup has been selected (full or differential)
            string backupType = "full"; // Full backup by default
            if (FullBackupCheckBox.IsChecked == true)
                backupType = "full";
            else if (DifferentialBackupCheckBox.IsChecked == true)
                backupType = "differential";

            // For each backup selected, the recovery is performed
            foreach (var selectedBackup in selectedBackups)
            {
                RecoverBackup(selectedBackup, backupType);
            }
            MessageBox.Show("Recover complete.");
        }

        // Recover selected files (from destination folder to source folder)
        private void RecoverBackup(Backup selectedBackup, string backupType)
        {
            string sourceDirectory = selectedBackup.SourcePath;   // Source folder where files can be recovered
            string destinationFolder = selectedBackup.DestinationPath;  // Destination folder from which to retrieve files

            try
            {
                if (Directory.Exists(destinationFolder))
                {
                    // Select only files marked for recovery in the GUI
                    var selectedFiles = BackupCheckboxesPanel.Children.OfType<CheckBox>()
                        .Where(cb => cb.IsChecked == true)
                        .Select(cb => cb.Content.ToString()) // Assume that the file name is in the CheckBox content
                        .ToList();

                    // Debug: Display files in the destination folder
                    string[] filesInDestination = Directory.GetFiles(destinationFolder);
                    MessageBox.Show("Files in the destination folder: " + string.Join(", ", filesInDestination.Select(f => Path.GetFileName(f))));

                    foreach (var fileName in selectedFiles)
                    {
                        // Search for the exact file in the destination folder
                        string sourceFile = Path.Combine(sourceDirectory, fileName); // Full path to source file
                        string recoverPath = Path.Combine(destinationFolder, fileName);  // Path of the file in the destination

                        // Debug: Display files searched for
                        MessageBox.Show($"File search '{fileName}' in the destination folder: {recoverPath}");

                        // Case-insensitive file name comparison
                        bool fileExists = filesInDestination.Any(file => string.Equals(Path.GetFileName(file), fileName, StringComparison.OrdinalIgnoreCase));

                        if (fileExists)
                        {
                            // Full backup: always copy the file from destination to source
                            if (backupType == "full")
                            {
                                File.Copy(recoverPath, sourceFile, true); // Copies the file to the source folder
                                MessageBox.Show($"File'{fileName}' has been successfully recovered (complete).");
                            }
                            // Differential backup: only copy if the file was modified after the last copy
                            else if (backupType == "differential")
                            {
                                if (File.Exists(sourceFile))
                                {
                                    DateTime sourceLastModified = File.GetLastWriteTime(sourceFile);
                                    DateTime destinationLastModified = File.GetLastWriteTime(recoverPath);

                                    // Copy only if the destination file is newer
                                    if (destinationLastModified > sourceLastModified)
                                    {
                                        File.Copy(recoverPath, sourceFile, true);
                                        MessageBox.Show($"File '{fileName}' has been successfully recovered (différentiel).");
                                    }
                                    else
                                    {
                                        MessageBox.Show($"File '{fileName}' has not been modified, no recovery required.");
                                    }
                                }
                                else
                                {
                                    // Source file does not exist, copy directly
                                    File.Copy(recoverPath, sourceFile, true);
                                    MessageBox.Show($"File '{fileName}' has been successfully recovered (differential - new file).");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"File '{fileName}' does not exist in the destination folder.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"The destination directory '{destinationFolder}' does not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during recovery: {ex.Message}");
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            // Reset all RadioButtons in the backup panel
            foreach (CheckBox checkBox in BackupCheckboxesPanel.Children)
            {
                checkBox.IsChecked = false;  // Uncheck each box
            }

            // Reset backup type choices
            FullBackupCheckBox.IsChecked = false;  // Uncheck Full Backup
            DifferentialBackupCheckBox.IsChecked = false;  // Uncheck Differential Backup
        }

        private void ButtonLeave_Click(object sender, RoutedEventArgs e)
        {
            // Opening of the main window and closing actual window 
            MainWindow viewMain = new MainWindow();
            viewMain.Show();
            this.Close();
        }
    }

    //  Class representing a backup
    public class Backup
    {
        public string Name { get; set; }
        public string DestinationPath { get; set; }
        public string SourcePath { get; set; }
    }
}
