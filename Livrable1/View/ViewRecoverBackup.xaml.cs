﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Livrable1.ViewModel;
using System.Diagnostics;
using Livrable1.Model;

//---------------------View---------------------//
namespace Livrable1.View
{
    /// <summary>
    /// Interaction logic for ViewRecoverBackup.xaml
    /// </summary>

    //------------Class ViewRecoverBackup------------//
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

        // Method to display backup jobs in checkboxes
        private void DisplayBackupJobs()
        {
            // Add all backups to the FilesToRecover collection (used for displaying checkboxes)
            foreach (var backup in _viewModel.Backups)
            {
                _viewModel.FilesToRecover.Add(new BackupFileViewModel { FileName = backup.NameSave, IsSelected = false });
            }
        }

        // Event handler for the validate button click - runs the recovery process
        private void ButtonValidate_Click(object sender, RoutedEventArgs e)
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

            // Call the ViewModel to perform the recovery
            foreach (var selectedFile in selectedFiles)
            {
                var backupToRecover = _viewModel.Backups.FirstOrDefault(b => b.NameSave == selectedFile);
                if (backupToRecover != null)
                {
                    _viewModel.RecoverBackup(backupToRecover, backupType); // Call the ViewModel method to recover the backup
                }
            }

            MessageBox.Show(LanguageManager.GetText("recovery_completed")); // Show success message
        }

        // Cancel Button : Reset the selections
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            // Uncheck all checkboxes
            foreach (CheckBox checkBox in BackupCheckboxesPanel.Children)
            {
                checkBox.IsChecked = false;  // Uncheck each box
            }

            // Reset backup type choices
            FullBackupCheckBox.IsChecked = false;  // Uncheck Full Backup
            DifferentialBackupCheckBox.IsChecked = false;  // Uncheck Differential Backup
        }

        // Leave Button : Open main window and close this one
        private void ButtonLeave_Click(object sender, RoutedEventArgs e)
        {
            MainWindow viewMain = new MainWindow(); // Create a new instance of MainWindow
            viewMain.Show(); // Show the MainWindow
            this.Close(); // Close the current window
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
    //------------Class ViewRecoverBackup------------//
}
//---------------------View---------------------//