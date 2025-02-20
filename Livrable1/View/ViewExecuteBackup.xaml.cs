using Livrable1.ViewModel;
using Livrable1.Model;
using System.Linq;
using System.Windows;
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

        private void ButtonLeave_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
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
            }
            else
            {
                // Show error message if selections are missing
                MessageBox.Show(LanguageManager.GetText("please_select_backup_and_type"));
            }
        }
        
        private void PauseBackup_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridBackups.SelectedItem is SaveInformation selectedBackup)
            {
                viewModel.PauseBackup(selectedBackup);
            }
        }

        private void ResumeBackup_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridBackups.SelectedItem is SaveInformation selectedBackup)
            {
                viewModel.ResumeBackup(selectedBackup);
            }
        }

        private void StopBackup_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridBackups.SelectedItem is SaveInformation selectedBackup)
            {
                viewModel.StopBackup(selectedBackup);
            }
        }
        private void DeleteBackup_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridBackups.SelectedItem is SaveInformation selectedBackup)
            {
                viewModel.DeleteBackup(selectedBackup);
            }
        }
    }
}
