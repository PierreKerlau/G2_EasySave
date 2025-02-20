using Livrable1.ViewModel;
using Livrable1.Model;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Livrable1.View
{
    public partial class ViewExecuteBackup : Window
    {
        private ExecuteBackupViewModel viewModel;

        public ViewExecuteBackup()
        {
            InitializeComponent();
            viewModel = new ExecuteBackupViewModel();
            this.DataContext = viewModel;
        }

        private void ButtonLeave_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridBackups.SelectedItem is SaveInformation selectedBackup &&
                BackupTypeSelector.SelectedItem is ComboBoxItem selectedType)
            {
                viewModel.ExecuteBackup(selectedBackup, selectedType.Content.ToString());
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une sauvegarde et un type.");
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