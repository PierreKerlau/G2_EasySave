using Livrable1.ViewModel;
using Livrable1.Model;
using System.Linq;
using System.Windows;
using Livrable1.Controller;
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

        private void ButtonLeave_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridBackups.SelectedItem is SaveInformation selectedBackup &&
                BackupTypeSelector.SelectedItem is ComboBoxItem selectedType)
            {
                viewModel.ExecuteBackup(selectedBackup, selectedType.Content.ToString());
                MessageBox.Show($"Sauvegarde '{selectedType.Content}' exécutée pour {selectedBackup.NameSave} !");
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une sauvegarde et un type.");
            }
        }

        private void ButtonLeave_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow viewMain = new MainWindow();
            viewMain.Show();
            this.Close();
        }
    }
}
