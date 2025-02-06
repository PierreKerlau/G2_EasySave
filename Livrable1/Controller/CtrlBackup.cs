using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Livrable1.Model;
using Livrable1.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Livrable1.Controller
{
    public class CtrlBackup
    {
        private List<Backup> backupJobs = new();
        private List<string> backupNames = new();
        private Logger logger;

        public CtrlBackup()
        {
            LoadData();
            logger = new Logger();
        }

        public void AddBackup()
        {
            ViewAddBackup.AddBackup();

            // Vérification du nom de sauvegarde
            string backupName;
            do
            {
                Console.Write("\nEnter a name for the backup: ");
                backupName = Console.ReadLine().Trim();
                if (backupNames.Contains(backupName))
                {
                    Console.WriteLine("ERROR: Backup name already exists. Please choose a different name.");
                }
            } while (backupNames.Contains(backupName));

            backupNames.Add(backupName); // Ajouter le nom à la liste une fois validé

            // Vérification du chemin source
            string sourcePath;
            do
            {
                Console.Write("\nEnter the source path: ");
                sourcePath = Console.ReadLine().Trim();
            } while (!Directory.Exists(sourcePath));

            // Vérification du chemin destination
            string destinationPath;
            do
            {
                Console.Write("\nEnter the destination path: ");
                destinationPath = Console.ReadLine().Trim();
            } while (!Directory.Exists(destinationPath));

            // Ajout des fichiers sélectionnés
            List<string> selectedFiles = new();
            bool addMoreFiles = true;

            while (addMoreFiles)
            {
                string[] files = Directory.GetFiles(sourcePath);

                if (files.Length == 0)
                {
                    Console.WriteLine("No files found in the source folder.");
                    break;
                }

                Console.WriteLine("\nAvailable files:");
                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine($"[{i + 1}] {Path.GetFileName(files[i])}");
                }

                Console.Write("\nSelect a file number to add to backup (or 0 to finish): ");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice <= files.Length)
                {
                    if (choice == 0) break;

                    string selectedFile = files[choice - 1];
                    selectedFiles.Add(selectedFile);
                    Console.WriteLine($"File added to backup queue: {Path.GetFileName(selectedFile)}");

                    Console.Write("\nDo you want to add another file? (o/N): ");
                    addMoreFiles = Console.ReadLine().Trim().ToLower() == "o";
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please select a valid file number.");
                }
            }

            // Si des fichiers ont été sélectionnés, crée une seule sauvegarde
            if (selectedFiles.Count > 0)
            {
                string combinedSource = string.Join(" | ", selectedFiles);
                backupJobs.Add(new Backup(backupName, combinedSource, destinationPath));
                Console.WriteLine($"Backup job '{backupName}' with {selectedFiles.Count} files added successfully!");
            }
            else
            {
                Console.WriteLine("No files selected for backup.");
            }
        }

        public void ExecuteBackup()
        {
            ViewExecuteBackup.ExecuteBackup();
            logger.UpdateState("Backup Operation", "Starting backup");

            if (backupJobs.Count == 0)
            {
                Console.WriteLine("No backup jobs in queue.");
                return;
            }

            Console.WriteLine("\nAvailable backup jobs:");
            for (int i = 0; i < backupJobs.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {backupJobs[i].name}");
            }

            Console.Write("\nEnter the backup(s) to execute (Press ? for help): ");
            string input = Console.ReadLine();
            List<int> selectedIndexes = ParseSelection(input, backupJobs.Count);

            if (selectedIndexes.Count == 0)
            {
                Console.WriteLine("No valid selection. Operation canceled.");
                return;
            }

            Console.Write("\nSelect backup type - Full (F) / Differential (D): ");
            string backupType = Console.ReadLine().Trim().ToLower();
            bool isDifferential = backupType == "d";

            Console.WriteLine("\nStarting backup...");

            foreach (int index in selectedIndexes)
            {
                Backup backup = backupJobs[index - 1];
                string destinationFolder = Path.Combine(backup.destinationPath, backup.name);
                logger.UpdateState(backup.name, $"Starting backup of {backup.name}");

                bool backupExists = Directory.Exists(destinationFolder);
                if (isDifferential && !backupExists)
                {
                    Console.WriteLine($"No previous backup found for '{backup.name}'. Performing full backup instead.");
                    isDifferential = false;
                }

                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                string[] files = backup.sourcePath.Split(" | ");

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string destinationFile = Path.Combine(destinationFolder, fileName);

                    try
                    {
                        if (File.Exists(file))
                        {
                            var startTime = DateTime.Now;
                            long fileSize = new FileInfo(file).Length;

                            if (isDifferential)
                            {
                                if (!File.Exists(destinationFile) || File.GetLastWriteTime(file) > File.GetLastWriteTime(destinationFile))
                                {
                                    File.Copy(file, destinationFile, true);
                                    long transferTime = (long)(DateTime.Now - startTime).TotalMilliseconds;
                                    logger.LogBackupOperation(backup.name, file, destinationFile, fileSize, transferTime);
                                    Console.WriteLine($"[DIFFERENTIAL] {fileName} copied.");
                                }
                                else
                                {
                                    Console.WriteLine($"[DIFFERENTIAL] {fileName} is unchanged. Skipping...");
                                }
                            }
                            else
                            {
                                File.Copy(file, destinationFile, true);
                                long transferTime = (long)(DateTime.Now - startTime).TotalMilliseconds;
                                logger.LogBackupOperation(backup.name, file, destinationFile, fileSize, transferTime);
                                Console.WriteLine($"[FULL] {fileName} copied.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"ERROR: File '{fileName}' not found.");
                            logger.UpdateState(backup.name, $"Error: File {fileName} not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ERROR: Failed to backup '{fileName}': {ex.Message}");
                        logger.UpdateState(backup.name, $"Error: {ex.Message}");
                    }
                }
                
                logger.UpdateState(backup.name, "Backup completed successfully");
            }

            logger.UpdateState("Backup Operation", "All backups completed");
            Console.WriteLine("\nBackup process completed!");
        }

        private List<int> ParseSelection(string input, int maxIndex)
        {
            List<int> selectedIndexes = new();
            if (string.IsNullOrWhiteSpace(input)) return selectedIndexes;

            try
            {
                var parts = input.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in parts)
                {
                    if (part.Contains("-"))
                    {
                        var rangeParts = part.Split('-');
                        if (rangeParts.Length == 2 &&
                            int.TryParse(rangeParts[0], out int start) &&
                            int.TryParse(rangeParts[1], out int end) &&
                            start >= 1 && end <= maxIndex && start <= end)
                        {
                            selectedIndexes.AddRange(Enumerable.Range(start, end - start + 1));
                        }
                    }
                    else if (int.TryParse(part, out int singleIndex) && singleIndex >= 1 && singleIndex <= maxIndex)
                    {
                        selectedIndexes.Add(singleIndex);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Invalid selection format. Please use: x, x-y, or x;y");
            }

            return selectedIndexes.Distinct().OrderBy(n => n).ToList();
        }

        public void RecoverBackup()
        {
            bool continueRecovery;
            do
            {
                Console.Clear();
                ViewRecoverBackup.RecoverBackup();
                Console.Write("Select backup type - Complete (C) / Differential (D): ");
                string backupType = Console.ReadLine().Trim().ToLower();

                bool isDifferential = backupType == "d";

                Console.WriteLine("\nAvailable backups:");
                foreach (var backup in backupJobs)
                {
                    Console.WriteLine($"- {backup.name}");
                }

                string backupName;
                Backup selectedBackup = null;
                do
                {
                    Console.Write("\nEnter the name of the backup to recover: ");
                    backupName = Console.ReadLine().Trim();
                    selectedBackup = backupJobs.FirstOrDefault(b => b.name.Equals(backupName, StringComparison.OrdinalIgnoreCase));

                    if (selectedBackup == null)
                    {
                        Console.WriteLine("ERROR: Backup name not found. Please try again.");
                    }
                } while (selectedBackup == null);

                Console.WriteLine("\nStarting recovery...");
                string destinationFolder = selectedBackup.destinationPath;
                string[] files = selectedBackup.sourcePath.Split(" | ");

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string sourceFile = Path.Combine(destinationFolder, selectedBackup.name, fileName);
                    string recoverPath = Path.Combine(destinationFolder, fileName);

                    try
                    {
                        if (File.Exists(sourceFile))
                        {
                            File.Copy(sourceFile, recoverPath, true);
                            Console.WriteLine($"[RECOVERY] {fileName} restored.");
                        }
                        else
                        {
                            Console.WriteLine($"ERROR: File '{fileName}' not found in backup.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ERROR: Failed to recover '{fileName}': {ex.Message}");
                    }
                }

                Console.Write("\nDo you want to recover another backup? (o/N): ");
                continueRecovery = Console.ReadLine().Trim().ToLower() == "o";
            } while (continueRecovery);
        }

        public void ChoiceLanguage()
        {
            Console.Clear();
            Console.WriteLine(LanguageManager.GetText("choose_language"));
            ConsoleKeyInfo choice = Console.ReadKey();

            if (choice.KeyChar == '1') LanguageManager.SetLanguage("en");
            else if (choice.KeyChar == '2') LanguageManager.SetLanguage("fr");
            else Console.WriteLine($"\n{LanguageManager.GetText("invalid_choice")}");
        }

        private void LoadData()
        {
            // TODO
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void StartSauvegarde()
        {
            Timer timer = new Timer(GenererJson, null, 0, 200); // Mise à jour toutes les 5 sec
            Console.WriteLine("Mise à jour des états en temps réel... Appuyez sur Entrée pour quitter.");
            Console.ReadLine();
        }

        private void GenererJson(object? state)
        {
            string filePath = "state.json";
            List<EtatSauvegarde> sauvegardes;

            // Vérifie si le fichier JSON existe déjà
            if (File.Exists(filePath))
            {
                try
                {
                    // Lecture du fichier JSON existant
                    string existingJson = File.ReadAllText(filePath);
                    sauvegardes = JsonSerializer.Deserialize<List<EtatSauvegarde>>(existingJson) ?? new List<EtatSauvegarde>();
                }
                catch (Exception)
                {
                    Console.WriteLine("Erreur de lecture du JSON, création d'un nouveau fichier.");
                    sauvegardes = new List<EtatSauvegarde>();
                }
            }
            else
            {
                sauvegardes = new List<EtatSauvegarde>();
            }

            // Simulation de plusieurs sauvegardes en cours
            Random rnd = new Random();
            for (int i = 1; i <= 3; i++) // Simuler 3 sauvegardes
            {
                string nomSauvegarde = "Sauvegarde_" + i;

                // Rechercher si la sauvegarde existe déjà
                var etat = sauvegardes.Find(s => s.Appellation == nomSauvegarde);
                if (etat == null)
                {
                    // Création d'une nouvelle sauvegarde
                    etat = new EtatSauvegarde
                    {
                        Appellation = nomSauvegarde,
                        HorodatageDerniereAction = DateTime.Now,
                        Etat = "Actif",
                        NombreTotalFichiers = 100,
                        TailleTotaleFichiersMB = 5000,
                        Progression = 0
                    };
                    sauvegardes.Add(etat);
                }

                // Mise à jour des valeurs existantes
                etat.HorodatageDerniereAction = DateTime.Now;
                etat.Progression = Math.Min(etat.Progression + rnd.Next(1, 10), 100);
                etat.NombreFichiersRestants = etat.NombreTotalFichiers * (100 - etat.Progression) / 100;
                etat.TailleFichiersRestantsMB = etat.TailleTotaleFichiersMB * (100 - etat.Progression) / 100;
                etat.FichierSource = "/mnt/data/source/file" + rnd.Next(1, 100) + ".txt";
                etat.FichierDestination = "/mnt/data/destination/file" + rnd.Next(1, 100) + ".txt";

                // Si progression à 100%, marquer comme terminé
                if (etat.Progression == 100)
                    etat.Etat = "END";
            }

            // Sérialisation et écriture dans le même fichier JSON
            string json = JsonSerializer.Serialize(sauvegardes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);

        }
        public void ShowLogs()
        {
            ViewLogs.ShowLogs();
        }
    }

}
