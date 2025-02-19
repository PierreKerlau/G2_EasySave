using Livrable1.ViewModel;
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
                MessageBox.Show($"Sauvegarde '{selectedType.Content}' exécutée pour {selectedBackup.NameSave} !");
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une sauvegarde et un type."); // Show error message if selections are missing
            }
        }

        // Event handler for the leave button click (alternative button)
        private void ButtonLeave_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow viewMain = new MainWindow(); // Create a new instance of MainWindow
            viewMain.Show(); // Show the MainWindow
            this.Close(); // Close the current window
        }
    }
    //------------Class ViewExecuteBackup------------//
}
//---------------------View---------------------//