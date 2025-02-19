//using System;
//using System.Collections.Generic;
//using System.IO; 
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;
//using Livrable1.ViewModel;
//using System.Diagnostics;

//namespace Livrable1.View
//{
//    /// <summary>
//    /// Logique d'interaction pour ViewRecoverBackup.xaml
//    /// </summary>
//    public partial class ViewRecoverBackup : Window
//    {
//        private List<SaveInformation> backupJobs; // List of backup jobs
//        private RecoverBackupViewModel _viewModel;
//        public ViewRecoverBackup() 
//        {
//            InitializeComponent();
//            _viewModel = new RecoverBackupViewModel();
//            this.DataContext = _viewModel;
//            DisplayBackupJobs(); // Display backup jobs in Checkboxes
//            UpdateUILanguageRecoveryBackup(); // Update language
//        }

//        // Display backup jobs in Checkboxes
//        private void DisplayBackupJobs()
//        {
//            foreach (var backup in _viewModel.Backups)
//            {
//                _viewModel.FilesToRecover.Add(new BackupFileViewModel { FileName = backup.NameSave, IsSelected = false });
//            }
//        }

//        // Validation Button : run the recovery
//        private void ButtonValidate_Click(object sender, RoutedEventArgs e)
//        {
//            // Checks whether a backup has been selected
//            //var selectedBackups = BackupCheckboxesPanel.Children.OfType<CheckBox>()
//            //    .Where(cb => cb.IsChecked == true)
//            //    .Select(cb => cb.Tag as SaveInformation)
//            //    .ToList();
//            var selectedFiles = _viewModel.FilesToRecover
//            .Where(f => f.IsSelected)
//            .Select(f => f.FileName)
//            .ToList();

//            if (selectedFiles.Count == 0)
//            {
//                MessageBox.Show(LanguageManager.GetText("select_backup_type"));
//                return;
//            }


//            //Récupérer le type de sauvegarde(full ou différentiel)
//            string backupType = FullBackupCheckBox.IsChecked == true ? "full" : "differential";

//            // Appeler le ViewModel pour effectuer la récupération
//            foreach (var selectedFile in selectedFiles)
//            {
//                var backupToRecover = _viewModel.Backups.FirstOrDefault(b => b.NameSave == selectedFile);
//                if (backupToRecover != null)
//                {
//                    _viewModel.RecoverBackup(backupToRecover, backupType); // Appel à la méthode du ViewModel
//                }
//            }
//        }

//        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
//        {
//            // Reset all RadioButtons in the backup panel
//            foreach (CheckBox checkBox in BackupCheckboxesPanel.Children)
//            {
//                checkBox.IsChecked = false;  // Uncheck each box
//            }

//            // Reset backup type choices
//            FullBackupCheckBox.IsChecked = false;  // Uncheck Full Backup
//            DifferentialBackupCheckBox.IsChecked = false;  // Uncheck Differential Backup
//        }

//        private void ButtonLeave_Click(object sender, RoutedEventArgs e)
//        {
//            // Opening of the main window and closing actual window 
//            MainWindow viewMain = new MainWindow();
//            viewMain.Show();
//            this.Close();
//        }

//        private void UpdateUILanguageRecoveryBackup()
//        {
//            LabelMainViewRecoverBackup.Content = LanguageManager.GetText("recover_backup_jobs");
//            LabelMainSelectRecoverBackup.Content = LanguageManager.GetText("select_backup_to_recover");
//            LabelMainBackupType.Content = LanguageManager.GetText("choose_backup_type");
//            FullBackupCheckBox.Content = LanguageManager.GetText("full_backup_checkbox");
//            DifferentialBackupCheckBox.Content = LanguageManager.GetText("differential_backup_checkbox");
//            ButtonCancel.Content = LanguageManager.GetText("cancel");
//            ButtonValidate.Content = LanguageManager.GetText("validate");
//            ButtonLeave.Content = LanguageManager.GetText("menu_leave");
//        }
//    }

//    ////  Class representing a backup
//    //public class Backup
//    //{
//    //    public string Name { get; set; }
//    //    public string DestinationPath { get; set; }
//    //    public string SourcePath { get; set; }
//    //}
//}


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
        private List<SaveInformation> backupJobs; // List of backup jobs
        private RecoverBackupViewModel _viewModel;

        public ViewRecoverBackup()
        {
            InitializeComponent();
            _viewModel = new RecoverBackupViewModel();
            this.DataContext = _viewModel;
            DisplayBackupJobs(); // Display backup jobs in Checkboxes
            UpdateUILanguageRecoveryBackup(); // Update language
        }

        // Display backup jobs in Checkboxes
        private void DisplayBackupJobs()
        {
            // Add all backups to the FilesToRecover collection (used for displaying checkboxes)
            foreach (var backup in _viewModel.Backups)
            {
                _viewModel.FilesToRecover.Add(new BackupFileViewModel { FileName = backup.NameSave, IsSelected = false });
            }
        }

        // Validation Button : run the recovery
        private void ButtonValidate_Click(object sender, RoutedEventArgs e)
        {
            // Get selected files to recover
            var selectedFiles = _viewModel.FilesToRecover
                .Where(f => f.IsSelected)
                .Select(f => f.FileName)
                .ToList();

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

            MessageBox.Show(LanguageManager.GetText("recovery_completed"));
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
            MainWindow viewMain = new MainWindow();
            viewMain.Show();
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