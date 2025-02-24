﻿using Livrable1.ViewModel;
using Livrable1.Model;
using System.Linq;
using System.Windows;
using Livrable1.ViewModel;
using System.Windows.Controls;

//---------------------View---------------------//
namespace Livrable1.View
{
    //------------Class ViewExecuteBackup------------//
    public partial class ViewExecuteBackup : Window
    {
        private ExecuteBackupViewModel viewModel; // ViewModel for executing backups

        // Constructor for ViewExecuteBackup
        public ViewExecuteBackup()
        {
            InitializeComponent(); // Initialize UI components
            UpdateUILanguageExecuteBackup(); // Update language
            viewModel = new ExecuteBackupViewModel(); // Create a new instance of ExecuteBackupViewModel
            this.DataContext = viewModel; // Set the DataContext to the ViewModel
        }

        // Event handler for the leave button click
        private void ButtonLeave_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close the current window
        }

        // Event handler for the execute button click
        private void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            // Check if a backup is selected in the DataGrid and a backup type is selected
            if (DataGridBackups.SelectedItem is SaveInformation selectedBackup &&
                BackupTypeSelector.SelectedItem is ComboBoxItem selectedType)
            {
                // Execute the backup using the ViewModel
                viewModel.ExecuteBackup(selectedBackup, selectedType.Content.ToString());
                // Show success message
                MessageBox.Show($"{LanguageManager.GetText("backup_execute")} '{selectedType.Content}' {LanguageManager.GetText("execute_for")} {selectedBackup.NameSave} !");
            }
            else
            {
                // Show error message if selections are missing
                MessageBox.Show(LanguageManager.GetText("please_select_backup_and_type"));
            }
        }

        // Event handler for the leave button click (alternative button)
        private void ButtonLeave_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow viewMain = new MainWindow(); // Create a new instance of MainWindow
            viewMain.Show(); // Show the MainWindow
            this.Close(); // Close the current window
        }

        // Method to update UI elements with language-specific texts
        private void UpdateUILanguageExecuteBackup()
        {
            LabelMainViewExecuteBackup.Content = LanguageManager.GetText("execute_backup_jobs");
            NameColumn.Header = LanguageManager.GetText("column_name");
            SourceColumn.Header = LanguageManager.GetText("column_source");
            DestinationColumn.Header = LanguageManager.GetText("column_destination");
            ButtonExecuteBackup.Content = LanguageManager.GetText("button_execute");
            ButtonLeave.Content = LanguageManager.GetText("menu_leave");

            var comboBoxItems = BackupTypeSelector.Items;
            foreach (var item in comboBoxItems)
            {
                var comboBoxItem = item as ComboBoxItem;
                if (comboBoxItem != null)
                {
                    if (comboBoxItem.Content.ToString() == "Full Backup")
                    {
                        comboBoxItem.Content = LanguageManager.GetText("combobox_full_backup");
                    }
                    else if (comboBoxItem.Content.ToString() == "Differential Backup")
                    {
                        comboBoxItem.Content = LanguageManager.GetText("combobox_differential_backup");
                    }
                }
            }
        }
    }
    //------------Class ViewExecuteBackup------------//
}
//---------------------View---------------------//