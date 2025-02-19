﻿using Livrable1.ViewModel;
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
            this.DataContext = new AddSaveViewModel(); // Set the DataContext to a new instance of AddSaveViewModel
            ListFiles.Visibility = Visibility.Collapsed; // Initially hide the file list
        }

        // Event handler for the leave button click
        private void ButtonLeave_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow viewMain = new MainWindow(); // Create a new instance of MainWindow
            viewMain.Show(); // Show the MainWindow
            this.Close(); // Close the current window
        }

        // Event handler for the first cancel button click
        private void FirstCancelAddBackups_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SaveInformation viewModel)
            {
                // Clear the text boxes for name, source path, and destination path
                txtNameSave.Clear();
                txtCheminSource.Clear();
                txtCheminDestination.Clear();
            }
        }

        // Event handler for the first validate button click
        private void FirstValidateAddBackup(object sender, RoutedEventArgs e)
        {
           if (DataContext is AddSaveViewModel viewModel)
            {
                // Get the input values from the text boxes
                string name = txtNameSave.Text; 
                string sourcePath = txtCheminSource.Text; 
                string destinationPath = txtCheminDestination.Text;

                // Create a new SaveInformation object with the input values
                var save = new SaveInformation(name, sourcePath, destinationPath);

                // Call the method to add the save and get the result
                bool result = viewModel.AddSaveMethod(save);

                // Check if the save was added successfully
                if (result)
                {
                    ListFiles.Visibility = Visibility.Visible; // Show the list of files
                    viewModel.LoadFilesFromSource(sourcePath); // Load files from the source path

                    MessageBox.Show("Sauvegarde ajoutée avec succès !"); // Show success message
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'ajout de la sauvegarde."); // Show error message
                }
            }
            else
            {
                MessageBox.Show("Echec du Valider !"); // Show validation failure message
            }
            
        }

        // Event handler for the second cancel button click
        private void SecondCancelAddBackups(object sender, RoutedEventArgs e)
        {
            
        }

        // Event handler for the second validate button click
        private void SecondValidateAddBackup(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddSaveViewModel viewModel)
            {
                // Get the last saved backup
                var save = viewModel.Backups.LastOrDefault();

                if (save != null)
                {
                    // Save the selected files for the backup
                    viewModel.SaveSelectedFiles(save);
                    
                    MessageBox.Show("Fichiers sélectionnés enregistrés !"); // Show success message

                    // Clear the text boxes and hide the file list
                    txtNameSave.Clear();
                    txtCheminSource.Clear();
                    txtCheminDestination.Clear();
                    ListFiles.Visibility = Visibility.Collapsed;

                }
            }
        }
    }
    //------------Class ViewAddBackup------------//

}
//---------------------View---------------------//