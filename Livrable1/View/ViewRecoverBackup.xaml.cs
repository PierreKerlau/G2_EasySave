using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Livrable1.ViewModel;
using System.Diagnostics;
using Livrable1.Model;

namespace Livrable1.View
{
    public partial class ViewRecoverBackup : Window
    {
        private List<SaveInformation> backupJobs; // List of backup jobs
        private RecoverBackupViewModel _viewModel; // ViewModel for backup recovery

        // Constructor for ViewRecoverBackup
        public ViewRecoverBackup()
        {
            InitializeComponent(); // Initialize UI components
            _viewModel = new RecoverBackupViewModel(); // Create an instance of the ViewModel
            this.DataContext = _viewModel; // Set the DataContext to the ViewModel
            DisplayBackupJobs(); // Display backup jobs in Checkboxes
            UpdateUILanguageRecoveryBackup(); // Update language
        }

        // Method to check if a process with the given name is running
        private bool IsProcessRunning(string processName)
        {
            return System.Diagnostics.Process.GetProcessesByName(processName).Any();
        }

        // Method to display backup jobs in checkboxes
        private void DisplayBackupJobs()
        {
            foreach (var backup in _viewModel.Backups)
            {
                _viewModel.FilesToRecover.Add(new BackupFileViewModel { FileName = backup.NameSave, IsSelected = false });
            }
        }

        // Event handler for the validate button click - runs the recovery process
        private void ButtonValidate_Click(object sender, RoutedEventArgs e)
        {
            // Check if any forbidden process is running
            if (ProcessWatcher.Instance.BloquerNotepad && IsProcessRunning("Notepad") ||
                ProcessWatcher.Instance.BloquerCalculator && IsProcessRunning("CalculatorApp"))
            {
                MessageBox.Show(
                    $"{LanguageManager.GetText("action_blocked_software")}",
                    LanguageManager.GetText("alert_software"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }
            else
            {
                // Get selected files for recovery
                var selectedFiles = _viewModel.FilesToRecover
                    .Where(f => f.IsSelected)
                    .Select(f => f.FileName)
                    .ToList();

                // Check if at least one file is selected
                if (selectedFiles.Count == 0)
                {
                    MessageBox.Show(LanguageManager.GetText("select_backup_type"));
                    return;
                }

                // Get the type of backup (Full or Differential)
                string backupType = FullBackupCheckBox.IsChecked == true ? "full" : "differential";

                ProgressBarRecovery.Value = 0;
                ProgressBarRecovery.Maximum = selectedFiles.Count;
                LabelProgress.Content = "0%";

                // Call the ViewModel to perform the recovery
                foreach (var selectedFile in selectedFiles)
                {
                    var backupToRecover = _viewModel.Backups.FirstOrDefault(b => b.NameSave == selectedFile);
                    if (backupToRecover != null)
                    {
                        _viewModel.RecoverBackup(backupToRecover, backupType); // Recover the backup
                    }
                    ProgressBarRecovery.Value++;
                    LabelProgress.Content = $"{(int)((ProgressBarRecovery.Value / ProgressBarRecovery.Maximum) * 100)}%";
                }

                MessageBox.Show(LanguageManager.GetText("recovery_completed")); // Show success message
            }
        }

        // Cancel Button: Reset the selections
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            // Uncheck all checkboxes
            foreach (CheckBox checkBox in BackupCheckboxesPanel.Children)
            {
                checkBox.IsChecked = false; // Uncheck each box
            }

            // Reset backup type choices
            FullBackupCheckBox.IsChecked = false; // Uncheck Full Backup
            DifferentialBackupCheckBox.IsChecked = false; // Uncheck Differential Backup
        }

        // Leave Button: Open main window and close this one
        private void ButtonLeave_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            mainWindow.Left = this.Left;
            mainWindow.Top = this.Top;
            mainWindow.Show();
            this.Close();
        }

        // Method to update UI elements with language-specific texts
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
}