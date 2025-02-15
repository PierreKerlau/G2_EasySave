using Livrable1.Controller;
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

namespace Livrable1.View
{
    /// <summary>
    /// Logique d'interaction pour ViewAddBackup.xaml
    /// </summary>
    public partial class ViewAddBackup : Window
    {
        public ViewAddBackup()
        {
            InitializeComponent();
            this.DataContext = new AddSaveViewModel();
        }

        private void ButtonLeave_Click_1(object sender, RoutedEventArgs e)
        {
            // Ouverture de la fenêtre principale et fermeture de la fenêtre actuelle
            MainWindow viewMain = new MainWindow();
            viewMain.Show();
            this.Close();
        }

    //-------------------------------------First Part of Add Save-------------------------------------//

        //------------------Start Methods for Cancel 1------------------//
        private void FirstCancelAddBackups_Click(object sender, RoutedEventArgs e)
        {
            /*if (DataContext is TexteSaisie viewModel)
            {
                TexteSaisie TexteSaisiName = null;
                TexteSaisie TexteSaisiSource = null;
                TexteSaisie TexteSaisiDestination = null;
            }*/
        }
        //-------------------End Methods for Cancel 1-------------------//



        //------------------Start Methods for Validate 1------------------//
        private void FirstValidateAddBackup(object sender, RoutedEventArgs e)
        {
           if (DataContext is AddSaveViewModel viewModel)
            {
                // Récupère les informations depuis l'interface utilisateur (exemple)
                string name = txtNameSave.Text; // Par exemple, un TextBox pour le nom
                string sourcePath = txtCheminSource.Text; // Un TextBox pour le chemin source
                string destinationPath = txtCheminDestination.Text; // Un TextBox pour le chemin destination

                // Création de l'instance de SaveInformation avec les informations saisies
                var save = new SaveInformation(name, sourcePath, destinationPath);

                // Appel de la méthode d'ajout
                bool result = viewModel.AddSaveMethod(save);

                if (result)
                {
                    viewModel.LoadFilesFromSource(sourcePath);

                    MessageBox.Show("Sauvegarde ajoutée avec succès !");
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'ajout de la sauvegarde.");
                }
            }
            else
            {
                MessageBox.Show("Echec du Valider !");
            }
        }
        //-------------------End Methods for Validate 1-------------------//

    //-------------------------------------First Part of Add Save-------------------------------------//

    //-------------------------------------Second Part of Add Save-------------------------------------//

        //------------------Start Methods for Cancel 2------------------//
        private void SecondCancelAddBackups(object sender, RoutedEventArgs e)
        {
            
        }
        //-------------------End Methods for Cancel 2-------------------//



        //------------------Start Methods for Validate 2------------------//
        private void SecondValidateAddBackup(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddSaveViewModel viewModel)
            {
                // Supposons qu'on a une sauvegarde en cours
                var save = viewModel.Backups.LastOrDefault(); // Prendre la dernière sauvegarde ajoutée

                if (save != null)
                {
                    viewModel.SaveSelectedFiles(save);
                    
                    MessageBox.Show("Fichiers sélectionnés enregistrés !");
                    
                    txtNameSave.Clear();
                    txtCheminSource.Clear();
                    txtCheminDestination.Clear();
                    ListFiles.ItemsSource = null;

                }
            }
        }
        //-------------------End Methods for Validate 2-------------------//
        
    //-------------------------------------Second Part of Add Save-------------------------------------//
    }

}
