using System;
using System.Collections.Generic;
using System.IO; 
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Livrable1.ViewModel;
using System.Diagnostics;

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
            UpdateUILanguageRecoveryBackup(); // Update language
        }

        // Method to get back backup jobs list
        private List<Backup> GetBackupJobs()
        {
            return new List<Backup>
            {
                new Backup { Name = "tteesstt.txt", DestinationPath = "C:\\Users\\picou\\Documents\\FISA-INFO-A3\\GENIE_LOGICIEL\\ProjetV2\\G2_EasySave\\clonage\\Destination", SourcePath = "C:\\Users\\picou\\Documents\\FISA-INFO-A3\\GENIE_LOGICIEL\\ProjetV2\\G2_EasySave\\clonage\\Source" },
                new Backup { Name = "PGE - Grille évaluation RE - 2024 V1.docx", DestinationPath = "C:\\Users\\picou\\Documents\\FISA-INFO-A3\\GENIE_LOGICIEL\\ProjetV2\\G2_EasySave\\clonage\\Destination", SourcePath = "C:\\Users\\picou\\Documents\\FISA-INFO-A3\\GENIE_LOGICIEL\\ProjetV2\\G2_EasySave\\clonage\\Source" },
                new Backup { Name = "Rapport_Etonnement_PICOUL_Nathan.pdf", DestinationPath = "C:\\Users\\picou\\Documents\\FISA-INFO-A3\\GENIE_LOGICIEL\\ProjetV2\\G2_EasySave\\clonage\\Destination", SourcePath = "C:\\Users\\picou\\Documents\\FISA-INFO-A3\\GENIE_LOGICIEL\\ProjetV2\\G2_EasySave\\clonage\\Source" }

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
                MessageBox.Show(LanguageManager.GetText("select_backup_type"));
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
            MessageBox.Show(LanguageManager.GetText("recover_complete"));
        }

        // Recover selected files (from destination folder to source folder)
        private void RecoverBackup(Backup selectedBackup, string backupType)
        {
            try
            {
                string sourceDirectory = selectedBackup.SourcePath;
                string destinationFolder = selectedBackup.DestinationPath;

                // Select only files marked for recovery in the GUI
                var selectedFiles = BackupCheckboxesPanel.Children.OfType<CheckBox>()
                    .Where(cb => cb.IsChecked == true)
                    .Select(cb => cb.Content.ToString()) // Assume that the file name is in the CheckBox content
                    .ToList();

                foreach (var fileName in selectedFiles)
                {
                    string sourceFile = Path.Combine(sourceDirectory, fileName);
                    string recoverPath = Path.Combine(destinationFolder, fileName);
                    string extension = Path.GetExtension(fileName).ToLower();

                    bool shouldEncrypt = StateViewModel.IsPdfEnabled && extension == ".pdf" ||
                                       StateViewModel.IsTxtEnabled && extension == ".txt" ||
                                       StateViewModel.IsPngEnabled && extension == ".png" ||
                                       StateViewModel.IsJsonEnabled && extension == ".json" ||
                                       StateViewModel.IsXmlEnabled && extension == ".xml" ||
                                       StateViewModel.IsDocxEnabled && extension == ".docx";

                    if (shouldEncrypt)
                    {
                         try
                            {
                                string key = Environment.GetEnvironmentVariable("EASYSAVE_CRYPTO_KEY");
                                
                                using (Process cryptoProcess = new Process())
                                {
                                    cryptoProcess.StartInfo.FileName = "CryptoSoft.exe";
                                    cryptoProcess.StartInfo.Arguments = $"\"{recoverPath}\" \"{sourceFile}\" \"{key}\"";
                                    cryptoProcess.StartInfo.UseShellExecute = true;
                                    cryptoProcess.StartInfo.RedirectStandardError = false;
                                    
                                    cryptoProcess.Start();
                                    cryptoProcess.WaitForExit();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Erreur lors du cryptage : {ex.Message}");
                            }
                    }
                    else
                    {
                        File.Copy(recoverPath, sourceFile, true);
                        MessageBox.Show($"Copie simple de {fileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
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

        private void UpdateUILanguageRecoveryBackup()
        {
            LabelMainViewRecoverBackup.Content = LanguageManager.GetText("recover_backup_jobs");
            LabelMainSelectRecoverBackup.Content = LanguageManager.GetText("select_backup_to_recover");
            LabelMainBackupType.Content = LanguageManager.GetText("choose_backup_type");
            FullBackupCheckBox.Content = LanguageManager.GetText("full_backup_checkbox");
            DifferentialBackupCheckBox.Content = LanguageManager.GetText("differential_backup_checkbox");
            ButtonCancel.Content = LanguageManager.GetText("cancel");
            ButtonValidate.Content = LanguageManager.GetText("validate");
            ButtonLeave.Content = LanguageManager.GetText("menu_leave");
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
