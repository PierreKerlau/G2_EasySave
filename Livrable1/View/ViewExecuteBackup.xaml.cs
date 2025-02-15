using Livrable1.Controller;
using Livrable1.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using IOPath = System.IO.Path;

namespace Livrable1.View
{
    /// <summary>
    /// Logique d'interaction pour ViewExecuteBackup.xaml
    /// </summary>
    public partial class ViewExecuteBackup : Window
    {
        public ViewExecuteBackup()
        {
            InitializeComponent();
            DataContext = new ViewExecuteBackupViewModel();
        }

        private void ButtonLeave_Click_1(object sender, RoutedEventArgs e)
        {
            // Ouverture de la fenêtre principale et fermeture de la fenêtre actuelle
            MainWindow viewMain = new MainWindow();
            viewMain.Show();
            this.Close();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataContext = new ViewExecuteBackupViewModel();
        }

        private void ShowSelectedBackups(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ViewExecuteBackupViewModel;
            if (viewModel != null)
            {
                var selectedBackups = viewModel.Save.Where(b => b.IsSelected).ToList();

                if (selectedBackups.Any())
                {
                    string message = "Sauvegardes sélectionnées :\n" +
                        string.Join("\n", selectedBackups.Select(b => b.name));
                    MessageBox.Show(message, "Sélection");
                }
                else
                {
                    MessageBox.Show("Aucune sauvegarde sélectionnée.", "Sélection");
                }
            }
        }



        //------------------Start Methods for Cancel------------------//
        private void CancelSelectedBackups(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ViewExecuteBackupViewModel;
            if (viewModel != null)
            {
                viewModel.RemoveSelectBackup();
            }
        }
        //-------------------End Methods for Cancel-------------------//



        //------------------Start Methods for Validate------------------//
        private void ValidateSelectedBackups(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ViewExecuteBackupViewModel;
            if (viewModel != null)
            {
                var selectedBackups = viewModel.Save.Where(b => b.IsSelected).ToList();

                double progressStep = 100.0 / selectedBackups.Count;
                //double currentProgress = 0;

                foreach (var backup in selectedBackups)
                {
                    try
                    {
                        if (backup.isDirectory)
                        {
                            DirectoryCopy(backup.sourcePath, backup.destinationPath);
                        }
                        else
                        {
                            File.Copy(backup.sourcePath, backup.destinationPath, overwrite: true);
                        }

                        //currentProgress += progressStep;
                        //ProgressBar.Value = currentProgress;  // Mise à jour de la barre de progression

                        MessageBox.Show($"Sauvegarde réussie : {backup.name}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la sauvegarde de {backup.name}: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        //-------------------End Methods for Validate-------------------//



        private void DirectoryCopy(string sourceDirName, string destDirName)
        {
            var dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Le répertoire source {sourceDirName} n'existe pas.");
            }

            Directory.CreateDirectory(destDirName);

            // Copie des fichiers
            foreach (var file in dir.GetFiles())
            {
                string tempPath = IOPath.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, overwrite: true);
            }

            // Copie des sous-répertoires
            foreach (var subdir in dir.GetDirectories())
            {
                string tempPath = IOPath.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath);
            }
        }

    }
}
