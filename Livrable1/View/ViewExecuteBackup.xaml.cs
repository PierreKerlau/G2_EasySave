using Livrable1.ViewModel;
using Livrable1.Model;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Net.Sockets;
using System.Xml;

namespace Livrable1.View
{
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

        // Method to check if a process with the given name is running
        private bool IsProcessRunning(string processName)
        {
            return System.Diagnostics.Process.GetProcessesByName(processName).Any();
        }

        // Event handler for the execute button click
        private void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            // Check if any forbidden process is running based on settings in ProcessWatcher
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
                // Check if a backup is selected in the DataGrid and a backup type is selected
                if (BackupTypeSelector.SelectedItem is ComboBoxItem selectedType)
                {
                    foreach (var backup in viewModel.Backups)
                    {
                        if (backup.IsSelected)
                        {
                            // Exécute la sauvegarde pour les éléments sélectionnés
                            viewModel.ExecuteBackup(backup, selectedType.Content.ToString());
                        }
                    }
                }
                else
                {
                    // Show error message if selections are missing
                    MessageBox.Show("Veuillez sélectionner un type de sauvegarde.");
                }
            }
        }

        // Event handler for the leave button click (alternative button)
        private void ButtonLeave_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            mainWindow.Left = this.Left;
            mainWindow.Top = this.Top;
            mainWindow.Show();
            this.Close();
        }

        // Method to update UI elements with language-specific texts
        private void UpdateUILanguageExecuteBackup()
        {
            LabelMainViewExecuteBackup.Content = LanguageManager.GetText("execute_backup_jobs");
            NameColumn.Header = LanguageManager.GetText("column_name");
            SourceColumn.Header = LanguageManager.GetText("column_source");
            DestinationColumn.Header = LanguageManager.GetText("column_destination");
            ButtonExecute.Content = LanguageManager.GetText("button_execute");
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

        // Event handlers for backup management (pause, resume, stop, delete)
        private void PauseBackup_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridBackups.SelectedItem is SaveInformation selectedBackup)
            {
                viewModel.PauseBackup(selectedBackup); // Pause the selected backup
            }
        }

        private void ResumeBackup_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridBackups.SelectedItem is SaveInformation selectedBackup)
            {
                viewModel.ResumeBackup(selectedBackup); // Resume the selected backup
            }
        }

        private void StopBackup_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridBackups.SelectedItem is SaveInformation selectedBackup)
            {
                viewModel.StopBackup(selectedBackup); // Stop the selected backup
            }
        }

        //private void DeleteBackup_Click(object sender, RoutedEventArgs e)
        //{
        //    if (DataGridBackups.SelectedItem is SaveInformation selectedBackup)
        //    {
        //        viewModel.DeleteBackup(selectedBackup); // Delete the selected backup
        //    }
        //}
    }
}
