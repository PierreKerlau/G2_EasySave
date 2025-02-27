using Livrable1.ViewModel;
using Livrable1.Model;
using Livrable1.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;
using IOPath = System.IO.Path;

//---------------------View---------------------//
namespace Livrable1.View
{
    /// <summary>
    /// Interaction logic for ViewAddBackup.xaml
    /// </summary>

    //------------Class ViewAddBackup------------//
    public partial class ViewAddBackup : Window
    {
        // Constructor for ViewAddBackup
        public ViewAddBackup()
        {
            InitializeComponent(); // Initialize UI components
            UpdateUILanguageAddBackup();
            this.DataContext = new AddSaveViewModel(); // Set the DataContext to a new instance of AddSaveViewModel
            ListFiles.Visibility = Visibility.Collapsed; // Initially hide the file list
            ButtonValidate2.Visibility = Visibility.Collapsed;
            ButtonCancel2.Visibility = Visibility.Collapsed;
        }

        // Event handler for the leave button click
        private void ButtonLeave_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            mainWindow.Left = this.Left;
            mainWindow.Top = this.Top;
            mainWindow.Show();
            this.Close();
        }

        // Event handler for the first cancel button click
        private void FirstCancelAddBackups_Click(object sender, RoutedEventArgs e)
        {
                // Clear the text boxes for name, source path, and destination path
                txtNameSave.Clear();
                txtSourcePath.Clear();
                txtDestinationPath.Clear();
        }

        // Event handler for the first validate button click
        private void FirstValidateAddBackup(object sender, RoutedEventArgs e)
        {
           if (DataContext is AddSaveViewModel viewModel)
            {
                // Get the input values from the text boxes
                string name = txtNameSave.Text;
                string sourcePath = txtSourcePath.Text;
                string destinationPath = txtDestinationPath.Text;

                if (viewModel.VerifAddName(name))
                {
                    MessageBox.Show("Ce nom de sauvegarde existe déjà !", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return; // Arrêter l'exécution pour éviter la duplication
                }

                // Check if the save was added successfully
                if (viewModel != null)
                {
                    ButtonValidate2.Visibility = Visibility.Visible;
                    ButtonCancel2.Visibility = Visibility.Visible;
                    ListFiles.Visibility = Visibility.Visible; // Show the list of files


                    SaveInformation viewChargement = new SaveInformation(name, sourcePath, destinationPath);
                    viewChargement.LoadFiles();

                    txtNameSave.IsReadOnly = true;
                    txtSourcePath.IsReadOnly = true;
                    txtDestinationPath.IsReadOnly = true;

                    ButtonValidate.Visibility = Visibility.Collapsed;
                    ButtonCancel.Visibility = Visibility.Collapsed;
                    
                    viewModel.LoadFilesFromSource(sourcePath); // Load files from the source path

                    MessageBox.Show(LanguageManager.GetText("backup_added_success")); // Show success message
                }
                else
                {
                    MessageBox.Show(LanguageManager.GetText("error_during_addition_of_backup")); // Show error message
                }
            }
            else
            {
                MessageBox.Show(LanguageManager.GetText("fail_validate")); // Show validation failure message
            }
            
        }

        // Event handler for the second cancel button click
        private void SecondCancelAddBackups(object sender, RoutedEventArgs e)
        {
            foreach (var item in ListFiles.Items)
            {
                if (ListFiles.ItemContainerGenerator.ContainerFromItem(item) is ListViewItem listViewItem)
                {
                    if (listViewItem.Content is FileInformation model)
                    {
                        model.IsSelected = false;
                    }
                }
            }
            ListFiles.Items.Refresh();

        }

        // Event handler for the second validate button click
        private void SecondValidateAddBackup(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddSaveViewModel viewModel)
            {
                bool isAnyChecked = viewModel.Files.Any(file => file.IsSelected);

                if (!isAnyChecked)
                {
                    MessageBox.Show($"{LanguageManager.GetText("select_at_least_one_element")}", $"{LanguageManager.GetText("error_title")}", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get the input values from the text boxes
                string name = txtNameSave.Text;
                string sourcePath = txtSourcePath.Text;
                string destinationPath = txtDestinationPath.Text;

                // Gather the selected files
                var selectedFiles = viewModel.Files.Where(file => file.IsSelected).ToList();
                int numberFile = selectedFiles.Count;
                DateTime date = DateTime.Now;
                bool isActive = false; // Set active status as per your logic
                long totalSize = selectedFiles.Sum(file => file.Size);
                int remainingFiles = numberFile;
                long remainingSize = totalSize; // Assuming remaining size equals total size for the selected files

                // Create a new SaveInformation instance with selected files
                var save = new SaveInformation(name, sourcePath, destinationPath, numberFile, date, isActive, totalSize, remainingFiles, remainingSize, selectedFiles);

                // Save the selected files for the backup
                viewModel.SaveSelectedFiles(save);

                // Call the method to add the save and get the result
                bool result = viewModel.AddSaveMethod(save);

                MessageBox.Show(LanguageManager.GetText("selected_files_saved")); // Show success message

                // Reset UI
                txtNameSave.IsReadOnly = false;
                txtSourcePath.IsReadOnly = false;
                txtDestinationPath.IsReadOnly = false;

                txtNameSave.Clear();
                txtSourcePath.Clear();
                txtDestinationPath.Clear();

                ButtonValidate.Visibility = Visibility.Visible;
                ButtonCancel.Visibility = Visibility.Visible;

                ButtonValidate2.Visibility = Visibility.Collapsed;
                ButtonCancel2.Visibility = Visibility.Collapsed;
                ListFiles.Visibility = Visibility.Collapsed;
            }
        }
        //-------------------End Methods for Validate 2-------------------//

        //-------------------------------------Second Part of Add Save-------------------------------------//
        private void UpdateUILanguageAddBackup()
        {
            LabelTitleAddBackup.Content = LanguageManager.GetText("add_backup_jobs");
            ButtonCancel.Content = LanguageManager.GetText("cancel");
            ButtonValidate.Content = LanguageManager.GetText("validate");
            ButtonCancel2.Content = LanguageManager.GetText("cancel");
            ButtonValidate2.Content = LanguageManager.GetText("validate");
            ButtonLeave.Content = LanguageManager.GetText("menu_leave");
            LabelBackupName.Content = LanguageManager.GetText("label_backup_name");
            LabelSourcePath.Content = LanguageManager.GetText("label_source_name");
            LabelDestinationPath.Content = LanguageManager.GetText("label_destination_path");
        }
    }
    //------------Class ViewAddBackup------------//

}
//---------------------View---------------------//