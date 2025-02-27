using System;
using System.Collections.ObjectModel;
using System.IO;
using Livrable1.Model;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Xml;
using Livrable1.logger;

namespace Livrable1.ViewModel
{
    public class ExecuteBackupViewModel : BaseViewModel
    {
        public int TailleLimiteKo { get; set; } = 1024;
        public ObservableCollection<SaveInformation> Backups { get; set; }
        private readonly Dictionary<string, CancellationTokenSource> _cancellationTokens = new();
        private readonly Dictionary<string, ManualResetEventSlim> _pauseEvents = new();
        private readonly SemaphoreSlim _largeFileSemaphore = new(1, 1);

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
                    string englishBackupType = ConvertBackupType(backupType);
                    bool hasPriorityFiles = PriorityExtensionManager.Instance.HasPriorityFiles(backup.SourcePath);
                    
                    if (hasPriorityFiles)
                    {
                        ExecutePriorityBackup(backup, englishBackupType, cts.Token);
                    }
                    else
                    {
                        if (englishBackupType == "Full Backup")
                        {
                            ExecuteFullBackup(backup, cts.Token);
                        }
                        else if (englishBackupType == "Differential Backup")
                        {
                            ExecuteDifferentialBackup(backup, cts.Token);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show($"Backup cancelled for {backup.NameSave}.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Backup error: {ex.Message}");
                }
                finally
                {
                    _cancellationTokens.Remove(backup.NameSave);
                    _pauseEvents.Remove(backup.NameSave);
                    cts.Dispose();
                }
            }, cts.Token);
        }


        private string ConvertBackupType(string backupType)
        {
            if (backupType == LanguageManager.GetText("combobox_full_backup"))
                return "Full Backup";
            if (backupType == LanguageManager.GetText("combobox_differential_backup"))
                return "Differential Backup";
            return backupType;
        }

        private void ExecutePriorityBackup(SaveInformation backup, string backupType, CancellationToken token)
        {
            string backupFolder = Path.Combine(backup.DestinationPath, backup.NameSave);
            Directory.CreateDirectory(backupFolder);

            // Séparer les fichiers en deux groupes
            var priorityFiles = backup.Files
                .Where(f => PriorityExtensionManager.Instance.PriorityExtensions
                    .Contains(Path.GetExtension(f.FilePath).ToLower()))
                .ToList();

            var nonPriorityFiles = backup.Files
                .Where(f => !PriorityExtensionManager.Instance.PriorityExtensions
                    .Contains(Path.GetExtension(f.FilePath).ToLower()))
                .ToList();

            foreach (var file in priorityFiles)
            {
                token.ThrowIfCancellationRequested();
                _pauseEvents[backup.NameSave].Wait(token);

                string destFile = Path.Combine(backupFolder, file.FileName);

                var stopwatch = Stopwatch.StartNew();
                long cryptingTime = 0;

                string extension = Path.GetExtension(file.FilePath).ToLower();
                bool shouldEncrypt = (StateViewModel.IsPdfEnabled && extension == ".pdf") ||
                                   (StateViewModel.IsTxtEnabled && extension == ".txt") ||
                                   (StateViewModel.IsPngEnabled && extension == ".png") ||
                                   (StateViewModel.IsJsonEnabled && extension == ".json") ||
                                   (StateViewModel.IsXmlEnabled && extension == ".xml") ||
                                   (StateViewModel.IsDocxEnabled && extension == ".docx") ||
                                   (StateViewModel.IsMkvEnabled && extension == ".mkv") ||
                                   (StateViewModel.IsJpgEnabled && extension == ".jpg");

                if (shouldEncrypt)
                {
                    try
                    {
                        string key = Environment.GetEnvironmentVariable("EASYSAVE_CRYPTO_KEY");
                        using (Process cryptoProcess = new Process())
                        {
                            cryptoProcess.StartInfo.FileName = "CryptoSoft.exe";
                            cryptoProcess.StartInfo.Arguments = $"\"{file.FilePath}\" \"{destFile}\" \"{key}\"";
                            cryptoProcess.StartInfo.UseShellExecute = true;
                            cryptoProcess.Start();
                            cryptoProcess.WaitForExit();
                        }
                        cryptingTime = stopwatch.ElapsedMilliseconds;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{LanguageManager.GetText("error_encryption")}: {ex.Message}");
                    }
                }
                else
                {
                    File.Copy(file.FilePath, destFile, true);
                }

                stopwatch.Stop();
                long transferTime = stopwatch.ElapsedMilliseconds;

                Logger logger = new Logger();
                logger.LogBackupOperation(backup.NameSave, file.FilePath, destFile,
                    new FileInfo(file.FilePath).Length, transferTime, cryptingTime, StateViewModel.IsJsonOn);
            }

            // Puis traiter les fichiers non prioritaires de la même manière
            foreach (var file in nonPriorityFiles)
            {
                token.ThrowIfCancellationRequested();
                _pauseEvents[backup.NameSave].Wait(token);

                string destFile = Path.Combine(backupFolder, file.FileName);
                var stopwatch = Stopwatch.StartNew();
                long cryptingTime = 0;

                string extension = Path.GetExtension(file.FilePath).ToLower();
                bool shouldEncrypt = (StateViewModel.IsPdfEnabled && extension == ".pdf") ||
                                   (StateViewModel.IsTxtEnabled && extension == ".txt") ||
                                   (StateViewModel.IsPngEnabled && extension == ".png") ||
                                   (StateViewModel.IsJsonEnabled && extension == ".json") ||
                                   (StateViewModel.IsXmlEnabled && extension == ".xml") ||
                                   (StateViewModel.IsDocxEnabled && extension == ".docx") ||
                                   (StateViewModel.IsMkvEnabled && extension == ".mkv") ||
                                   (StateViewModel.IsJpgEnabled && extension == ".jpg");

                if (shouldEncrypt)
                {
                    try
                    {
                        string key = Environment.GetEnvironmentVariable("EASYSAVE_CRYPTO_KEY");
                        using (Process cryptoProcess = new Process())
                        {
                            cryptoProcess.StartInfo.FileName = "CryptoSoft.exe";
                            cryptoProcess.StartInfo.Arguments = $"\"{file.FilePath}\" \"{destFile}\" \"{key}\"";
                            cryptoProcess.StartInfo.UseShellExecute = true;
                            cryptoProcess.Start();
                            cryptoProcess.WaitForExit();
                        }
                        cryptingTime = stopwatch.ElapsedMilliseconds;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{LanguageManager.GetText("error_encryption")}: {ex.Message}");
                    }
                }
                else
                {
                    File.Copy(file.FilePath, destFile, true);
                }

                stopwatch.Stop();
                long transferTime = stopwatch.ElapsedMilliseconds;

                Logger logger = new Logger();
                logger.LogBackupOperation(backup.NameSave, file.FilePath, destFile,
                    new FileInfo(file.FilePath).Length, transferTime, cryptingTime, StateViewModel.IsJsonOn);
            }
        }

        private void ExecuteFullBackup(SaveInformation backup, CancellationToken token)
        {
            backup.RemainingSize = backup.TotalSize;
            backup.RemainingFiles = backup.NumberFile;

            string backupFolder = Path.Combine(backup.DestinationPath, backup.NameSave);
            Directory.CreateDirectory(backupFolder);

            var savedState = EtatSauvegarde.ReadState("../../../Logs/state.json");
            var savedBackup = savedState.FirstOrDefault(s => s.NameSave == backup.NameSave);

            if (savedBackup == null)
            {
                MessageBox.Show("Aucun enregistrement de sauvegarde trouvé.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedFiles = savedBackup.Files;
            if (selectedFiles == null || selectedFiles.Count == 0)
            {
                MessageBox.Show("Aucun fichier sélectionné pour la sauvegarde.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            long totalSize = selectedFiles.Sum(f => new FileInfo(f.FilePath).Length);
            long copiedSize = 0;

            // Mise à jour de l'état après la suppression du fichier
            EtatSauvegarde updateEtat = new EtatSauvegarde();

            foreach (var file in selectedFiles.ToList()) // Copie pour éviter les erreurs de modification de collection
            {
                token.ThrowIfCancellationRequested();
                _pauseEvents[backup.NameSave].Wait(token);

                string destFile = Path.Combine(backupFolder, file.FileName);
                long fileSize = new FileInfo(file.FilePath).Length;

                var stopwatch = Stopwatch.StartNew();
                CopyOrEncryptFile(file.FilePath, destFile);
                stopwatch.Stop();
                long transferTime = stopwatch.ElapsedMilliseconds;

                copiedSize += fileSize;
                backup.Progression = (int)((double)copiedSize / totalSize * 100);
                OnPropertyChanged(nameof(backup.Progression));

                // Décrémentation sécurisée
                if (backup.RemainingFiles > 0) backup.RemainingFiles--;
                if (backup.RemainingSize >= fileSize) backup.RemainingSize -= fileSize;
                else backup.RemainingSize = 0;

                // Vérification et mise à jour de l'état de sauvegarde
                updateEtat.UpdateSaveState(backup.NameSave, file.FilePath, backup.RemainingFiles, backup.RemainingSize);

                // Log de l'opération de sauvegarde
                Logger logger = new Logger();
                logger.LogBackupOperation(backup.NameSave, file.FilePath, destFile,
                    fileSize, transferTime, 0, StateViewModel.IsJsonOn);
            }

            updateEtat.UpdateActive(backup.NameSave);
        }


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

            EtatSauvegarde updateEtat = new EtatSauvegarde();

            foreach (var file in backup.Files)
            {
                token.ThrowIfCancellationRequested();
                _pauseEvents[backup.NameSave].Wait(token);

                string destFile = Path.Combine(backupFolder, file.FileName);
                long fileSize = new FileInfo(file.FilePath).Length;

                if (!File.Exists(destFile) || File.GetLastWriteTime(file.FilePath) > File.GetLastWriteTime(destFile))
                {
                // long fileSize = new FileInfo(file.FilePath).Length;
                
                if (fileSize > TailleLimiteKo)
                {
                    _largeFileSemaphore.Wait(token);
                }
                
                var stopwatch = Stopwatch.StartNew();
                
                try
                {
                    CopyOrEncryptFile(file.FilePath, destFile);
                }
                finally
                {
                    if (fileSize > TailleLimiteKo)
                    {
                        _largeFileSemaphore.Release();
                    }
                }
                
                stopwatch.Stop();
                long transferTime = stopwatch.ElapsedMilliseconds;
                
                copiedSize += fileSize;

                updateEtat.UpdateSaveState(backup.NameSave, file.FilePath, backup.RemainingFiles, backup.RemainingSize);

                // Log the backup operation
                Logger logger = new Logger();
                logger.LogBackupOperation(backup.NameSave, file.FilePath, destFile,
                    fileSize, transferTime, 0, StateViewModel.IsJsonOn);
                }

                backup.Progression = (int)((double)copiedSize / totalSize * 100);
                OnPropertyChanged(nameof(backup.Progression));
            }

            updateEtat.UpdateActive(backup.NameSave);
        }

        private void CopyOrEncryptFile(string sourceFile, string destinationFile)
        {
            string extension = Path.GetExtension(sourceFile).ToLower();
            bool shouldEncrypt = (StateViewModel.IsPdfEnabled && extension == ".pdf") ||
                                 (StateViewModel.IsTxtEnabled && extension == ".txt") ||
                                 (StateViewModel.IsPngEnabled && extension == ".png") ||
                                 (StateViewModel.IsJsonEnabled && extension == ".json") ||
                                 (StateViewModel.IsXmlEnabled && extension == ".xml") ||
                                 (StateViewModel.IsDocxEnabled && extension == ".docx") ||
                                 (StateViewModel.IsMkvEnabled && extension == ".mkv") ||
                                 (StateViewModel.IsJpgEnabled && extension == ".jpg");

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
                    MessageBox.Show($"{LanguageManager.GetText("error_encryption")}: {ex.Message}");
                }
            }
            else
            {
                File.Copy(sourceFile, destinationFile, true);
            }
        }

        // Methods for pause, resume, stop and delete functionality
        public void PauseBackup(SaveInformation backup)
        {
            if (_pauseEvents.ContainsKey(backup.NameSave))
            {
                _pauseEvents[backup.NameSave].Reset();
            }
            SaveManager.Instance.PauseBackup(backup);
        }

        public void ResumeBackup(SaveInformation backup)
        {
            if (_pauseEvents.ContainsKey(backup.NameSave))
            {
                _pauseEvents[backup.NameSave].Set();
            }
            SaveManager.Instance.ResumeBackup(backup);
        }

        public void StopBackup(SaveInformation backup)
        {
            if (_cancellationTokens.ContainsKey(backup.NameSave))
            {
                _cancellationTokens[backup.NameSave].Cancel();
            }
            SaveManager.Instance.StopBackup(backup);
        }

        public void DeleteBackup(SaveInformation backup)
        {
            SaveManager.Instance.DeleteBackup(backup);
            Backups.Remove(backup);
        }
    }
}
