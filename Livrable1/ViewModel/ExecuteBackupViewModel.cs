using System;
using System.Collections.ObjectModel;
using System.IO;
using Livrable1.Model;
using Livrable1.ViewModel;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Xml;

namespace Livrable1.ViewModel
{
    public class ExecuteBackupViewModel : INotifyPropertyChanged
    {
        // Collection of saved backups
        public ObservableCollection<SaveInformation> Backups { get; set; }
        private readonly Dictionary<string, CancellationTokenSource> _cancellationTokens = new();
        private readonly Dictionary<string, ManualResetEventSlim> _pauseEvents = new();

        public event PropertyChangedEventHandler PropertyChanged;

        // Constructor: initializes the backup list from SaveManager
        public ExecuteBackupViewModel()
        {
            Backups = new ObservableCollection<SaveInformation>(SaveManager.Instance.GetBackups());
            LoadSaves();
        }
        public void LoadSaves()
        {
            // Vérifier si le chemin source existe
            try
            {
                string jsonFilePath = "../../../Logs/state.json"; // Remplace par le chemin de ton fichier JSON

                if (System.IO.File.Exists(jsonFilePath))
                {
                    var saveList = EtatSauvegarde.ReadState(jsonFilePath);
                    foreach (var save in saveList)
                    {
                        // Vérifier si la sauvegarde existe déjà dans la collection
                        if (!Backups.Any(b => b.NameSave == save.NameSave))
                        {
                            Backups.Add(save); // Ajouter seulement si elle n'existe pas déjà
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la liste : {ex.Message}");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Method to execute a backup based on the selected type
        public void ExecuteBackup(SaveInformation backup, string backupType)
        {
            if (!_pauseEvents.ContainsKey(backup.NameSave))
                _pauseEvents[backup.NameSave] = new ManualResetEventSlim(true);

            var cts = new CancellationTokenSource();
            _cancellationTokens[backup.NameSave] = cts;

            Task.Run(() =>
            {
                try
                {
                    if (backupType == "Sauvegarde complète" || backupType == LanguageManager.GetText("combobox_full_backup"))
                    {
                        ExecuteFullBackup(backup, cts.Token);
                    }
                    else if (backupType == "Sauvegarde différentielle" || backupType == LanguageManager.GetText("combobox_differential_backup"))
                    {
                        ExecuteDifferentialBackup(backup, cts.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show($"Sauvegarde annulée pour {backup.NameSave}.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}");
                }
                finally
                {
                    _cancellationTokens.Remove(backup.NameSave);
                    _pauseEvents.Remove(backup.NameSave);
                    cts.Dispose();
                }
            }, cts.Token);
        }

        /*string backupFolder = Path.Combine(backup.DestinationPath, backup.NameSave);
            Directory.CreateDirectory(backupFolder);
            long totalSize = backup.Files.Sum(f => new FileInfo(f.FilePath).Length);
            long copiedSize = 0;

            foreach (var file in backup.Files)
            {
                token.ThrowIfCancellationRequested();
                _pauseEvents[backup.NameSave].Wait(token);

                string destFile = Path.Combine(backupFolder, file.FileName);
                CopyOrEncryptFile(file.FilePath, destFile);

                copiedSize += new FileInfo(file.FilePath).Length;
                backup.Progression = (int)((double)copiedSize / totalSize * 100);
                OnPropertyChanged(nameof(backup.Progression));
            }*/















        // Method to execute a full backup
        private void ExecuteFullBackup(SaveInformation backup, CancellationToken token)
        {
            string backupFolder = Path.Combine(backup.DestinationPath, backup.NameSave);
            Directory.CreateDirectory(backupFolder);

            // Lire l'état à partir du JSON
            var savedState = EtatSauvegarde.ReadState("../../../Logs/state.json"); // Appel statique

            // Trouver le bon enregistrement basé sur le nom de sauvegarde
            var savedBackup = savedState.FirstOrDefault(s => s.NameSave == backup.NameSave);

            // Vérifier si un enregistrement a été trouvé
            if (savedBackup == null)
            {
                MessageBox.Show("Aucun enregistrement de sauvegarde trouvé.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 4 fichiers
            var selectedFiles = savedBackup.Files;

            // 0 fichiers
            //var selectedFiles = savedBackup.Files.Where(f => f.);

            // Vérifier si des fichiers sont sélectionnés
            if (selectedFiles.Count == 0)
            {
                MessageBox.Show("Aucun fichier sélectionné pour la sauvegarde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Quitter la méthode si aucun fichier n'est sélectionné
            }

            long totalSize = selectedFiles.Sum(f => new FileInfo(f.FilePath).Length);
            long copiedSize = 0;

            foreach (var file in selectedFiles)
            {
                token.ThrowIfCancellationRequested();
                _pauseEvents[backup.NameSave].Wait(token);

                string destFile = Path.Combine(backupFolder, Path.GetFileName(file.FilePath)); // Utilisez Path.GetFileName pour le nom de fichier
                CopyOrEncryptFile(file.FilePath, destFile);

                copiedSize += new FileInfo(file.FilePath).Length;
                backup.Progression = (int)((double)copiedSize / totalSize * 100);
                OnPropertyChanged(nameof(backup.Progression));
            }

        }


        // Method to execute a differential backup (only modified files)
        private void ExecuteDifferentialBackup(SaveInformation backup, CancellationToken token)
        {
            string backupFolder = Path.Combine(backup.DestinationPath, backup.NameSave);
            Directory.CreateDirectory(backupFolder);

            // Lire l'état à partir du JSON
            var savedState = EtatSauvegarde.ReadState("../../../Logs/state.json"); // Appel statique

            // Trouver le bon enregistrement basé sur le nom de sauvegarde
            var savedBackup = savedState.FirstOrDefault(s => s.NameSave == backup.NameSave);

            // Vérifier si un enregistrement a été trouvé
            if (savedBackup == null)
            {
                MessageBox.Show("Aucun enregistrement de sauvegarde trouvé.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 4 fichiers
            var selectedFiles = savedBackup.Files;

            // 0 fichiers
            //var selectedFiles = savedBackup.Files.Where(f => f.);

            // Vérifier si des fichiers sont sélectionnés
            if (selectedFiles.Count == 0)
            {
                MessageBox.Show("Aucun fichier sélectionné pour la sauvegarde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Quitter la méthode si aucun fichier n'est sélectionné
            }

            long totalSize = backup.Files.Sum(f => new FileInfo(f.FilePath).Length);
            long copiedSize = 0;

            foreach (var file in backup.Files)
            {
                token.ThrowIfCancellationRequested();
                _pauseEvents[backup.NameSave].Wait(token);

                string destFile = Path.Combine(backupFolder, file.FileName);

                if (!File.Exists(destFile) || File.GetLastWriteTime(file.FilePath) > File.GetLastWriteTime(destFile))
                {
                    CopyOrEncryptFile(file.FilePath, destFile);
                    copiedSize += new FileInfo(file.FilePath).Length;
                }

                backup.Progression = (int)((double)copiedSize / totalSize * 100);
                OnPropertyChanged(nameof(backup.Progression));
            }
        }

        // Method to copy or encrypt files based on their type
        private void CopyOrEncryptFile(string sourceFile, string destinationFile)
        {
            string extension = Path.GetExtension(sourceFile).ToLower();
            bool shouldEncrypt = (StateViewModel.IsPdfEnabled && extension == ".pdf") ||
                                 (StateViewModel.IsTxtEnabled && extension == ".txt") ||
                                 (StateViewModel.IsPngEnabled && extension == ".png") ||
                                 (StateViewModel.IsJsonEnabled && extension == ".json") ||
                                 (StateViewModel.IsXmlEnabled && extension == ".xml") ||
                                 (StateViewModel.IsDocxEnabled && extension == ".docx");

            if (shouldEncrypt)
            {
                try
                {
                    string key = Environment.GetEnvironmentVariable("EASYSAVE_CRYPTO_KEY");
                    using (Process cryptoProcess = new Process())
                    {
                        cryptoProcess.StartInfo.FileName = "CryptoSoft.exe";
                        cryptoProcess.StartInfo.Arguments = $"\"{sourceFile}\" \"{destinationFile}\" \"{key}\"";
                        cryptoProcess.StartInfo.UseShellExecute = true;
                        cryptoProcess.Start();
                        cryptoProcess.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors du cryptage : {ex.Message}");
                }
            }
            else
            {
                File.Copy(sourceFile, destinationFile, true);
            }
        }

        // Methods for pause, resume, and stop functionality
        public void PauseBackup(SaveInformation backup)
        {
            _pauseEvents[backup.NameSave].Reset();
        }

        public void ResumeBackup(SaveInformation backup)
        {
            _pauseEvents[backup.NameSave].Set();
        }

        public void StopBackup(SaveInformation backup)
        {
            SaveManager.Instance.StopBackup(backup);
            if (_cancellationTokens.ContainsKey(backup.NameSave))
            {
                _cancellationTokens[backup.NameSave].Cancel();
            }
        }

        public void DeleteBackup(SaveInformation backup)
        {
            SaveManager.Instance.DeleteBackup(backup);
            Backups.Remove(backup);
        }
    }
}
