using Livrable1.ViewModel;
using Livrable1.Model;
using System.Linq;
using System.Windows;
using Livrable1.ViewModel;
using System.Windows.Controls;

namespace Livrable1.View
{
    public partial class ViewExecuteBackup : Window
    {
        private ExecuteBackupViewModel viewModel;

        public ViewExecuteBackup()
        {
            InitializeComponent();
            UpdateUILanguageExecuteBackup(); // Update language
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
                MessageBox.Show($"{LanguageManager.GetText("backup_execute")} '{selectedType.Content}' {LanguageManager.GetText("execute_for")} {selectedBackup.NameSave} !");
            }
            else
            {
                MessageBox.Show(LanguageManager.GetText("please_select_backup_and_type"));
            }
        }

        private void ButtonLeave_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow viewMain = new MainWindow();
            viewMain.Show();
            this.Close();
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
}
