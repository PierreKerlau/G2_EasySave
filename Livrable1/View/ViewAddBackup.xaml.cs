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
            UpdateUILanguageAddBackup();
            this.DataContext = new AddSaveViewModel();
            ListFiles.Visibility = Visibility.Collapsed;
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
                string sourcePath = txtSourcePath.Text; // Un TextBox pour le chemin source
                string destinationPath = txtDestinationPath.Text; // Un TextBox pour le chemin destination

                // Création de l'instance de SaveInformation avec les informations saisies
                var save = new SaveInformation(name, sourcePath, destinationPath);

                // Appel de la méthode d'ajout
                bool result = viewModel.AddSaveMethod(save);

                if (result)
                {
                    ListFiles.Visibility = Visibility.Visible;
                    viewModel.LoadFilesFromSource(sourcePath);

                    MessageBox.Show(LanguageManager.GetText("backup_added_success"));
                }
                else
                {
                    MessageBox.Show(LanguageManager.GetText("error_during_addition_of_backup"));
                }
            }
            else
            {
                MessageBox.Show(LanguageManager.GetText("fail_validate"));
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
                    
                    MessageBox.Show(LanguageManager.GetText("selected_files_saved"));

                    txtNameSave.Clear();
                    txtSourcePath.Clear();
                    txtDestinationPath.Clear();
                    //ListFiles.ItemsSource = null;
                    ListFiles.Visibility = Visibility.Collapsed;

                }
            }
        }
        //-------------------End Methods for Validate 2-------------------//

        //-------------------------------------Second Part of Add Save-------------------------------------//
        private void UpdateUILanguageAddBackup()
        {
            LabelTitleExecuteBackup.Content = LanguageManager.GetText("add_backup_jobs");
            ButtonCancel.Content = LanguageManager.GetText("cancel");
            ButtonValidate.Content = LanguageManager.GetText("validate");
            ButtonCancel2.Content = LanguageManager.GetText("cancel");
            ButtonValidate2.Content = LanguageManager.GetText("validate");
            ButtonLeave.Content = LanguageManager.GetText("menu_leave");
        }
    }

}
