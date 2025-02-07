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
        private readonly object _lockFile = new object();

        public CtrlBackup()
        {
            LoadData();
            logger = new Logger();
        }

        public void AddBackup()
        {
            ViewAddBackup.AddBackup();

            // Choix du nom de la sauvegarde et vérification de la non-existence
            string backupName;
            do
            {
                Console.Write("\n" + LanguageManager.GetText("enter_backup_name"));
                backupName = Console.ReadLine().Trim();
                if (backupNames.Contains(backupName))
                {
                    Console.WriteLine(LanguageManager.GetText("backup_name_exists"));
                }
            } while (backupNames.Contains(backupName));

            backupNames.Add(backupName);

            // Vérification du chemin source
            string sourcePath;
            do
            {
                Console.Write("\n" + LanguageManager.GetText("enter_source_path"));
                sourcePath = Console.ReadLine().Trim();
            } while (!Directory.Exists(sourcePath));

            // Vérification du chemin destination
            string destinationPath;
            do
            {
                Console.Write("\n" + LanguageManager.GetText("enter_destination_path"));
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
                    Console.WriteLine(LanguageManager.GetText("no_files_found"));
                    break;
                }

                Console.WriteLine("\n" + LanguageManager.GetText("available_files"));
                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine($"[{i + 1}] {Path.GetFileName(files[i])}");
                }

                Console.Write("\n" + LanguageManager.GetText("select_file_number"));
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice <= files.Length)
                {
                    if (choice == 0) break;

                    string selectedFile = files[choice - 1];
                    selectedFiles.Add(selectedFile);
                    Console.WriteLine($"{LanguageManager.GetText("file_added")} {Path.GetFileName(selectedFile)}");

                    Console.Write("\n" + LanguageManager.GetText("add_another_file"));
                    addMoreFiles = Console.ReadLine().Trim().ToLower() == "o";
                }
                else
                {
                    Console.WriteLine(LanguageManager.GetText("invalid_file_choice"));
                }
            }

            // Si des fichiers ont été sélectionnés, crée une seule sauvegarde
            if (selectedFiles.Count > 0)
            {
                string combinedSource = string.Join(" | ", selectedFiles);
                backupJobs.Add(new Backup(backupName, combinedSource, destinationPath));
                Console.WriteLine($"{LanguageManager.GetText("backup_job_added")} '{backupName}' {LanguageManager.GetText("with_files")} {selectedFiles.Count} {LanguageManager.GetText("files_added_successfully")}");
            }
            else
            {
                Console.WriteLine(LanguageManager.GetText("no_files_selected"));
            }
        }

        public void ExecuteBackup()
        {
            ViewExecuteBackup.ExecuteBackup();
            logger.UpdateState("Backup Operation", "Starting backup");

            if (backupJobs.Count == 0)
            {
                Console.WriteLine(LanguageManager.GetText("no_backup_jobs"));
                return;
            }

            Console.WriteLine("\n" + LanguageManager.GetText("available_backup"));
            for (int i = 0; i < backupJobs.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {backupJobs[i].name}");
            }

            Console.Write("\n" + LanguageManager.GetText("backup_to_execute"));
            string input = Console.ReadLine();
            List<int> selectedIndexes = ParseSelection(input, backupJobs.Count);

            if (selectedIndexes.Count == 0)
            {
                Console.WriteLine(LanguageManager.GetText("invalid_backup_selection"));
                return;
            }

            Console.Write("\n" + LanguageManager.GetText("backup_type"));
            string backupType = Console.ReadLine().Trim().ToLower();
            bool isDifferential = backupType == "d";

            Console.WriteLine("\n" + LanguageManager.GetText("starting_backup"));

            foreach (int index in selectedIndexes)
            {
                Backup backup = backupJobs[index - 1];
                string destinationFolder = Path.Combine(backup.destinationPath, backup.name);
                logger.UpdateState(backup.name, $"Starting backup of {backup.name}");

                bool backupExists = Directory.Exists(destinationFolder);
                if (isDifferential && !backupExists)
                {
                    Console.WriteLine($"{LanguageManager.GetText("no_backup_found_for")} '{backup.name}'. {LanguageManager.GetText("performing_full_backup_instead")}");
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
                                    Console.WriteLine($"{LanguageManager.GetText("differential_file")} '{fileName}'. {LanguageManager.GetText("file_copied")}");
                                }
                                else
                                {
                                    Console.WriteLine($"{LanguageManager.GetText("differential_file")} '{fileName}'. {LanguageManager.GetText("is_unchanged")}");
                                }
                            }
                            else
                            {
                                File.Copy(file, destinationFile, true);
                                long transferTime = (long)(DateTime.Now - startTime).TotalMilliseconds;
                                logger.LogBackupOperation(backup.name, file, destinationFile, fileSize, transferTime);
                                Console.WriteLine($"{LanguageManager.GetText("full_file")} '{fileName}'. {LanguageManager.GetText("file_copied")}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{LanguageManager.GetText("error_file")} '{fileName}'. {LanguageManager.GetText("not_found")}");
                            logger.UpdateState(backup.name, $"Error: File {fileName} not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{LanguageManager.GetText("error_failed_backup")} '{fileName}': {ex.Message}");
                        logger.UpdateState(backup.name, $"Error: {ex.Message}");
                    }
                }
                
                logger.UpdateState(backup.name, LanguageManager.GetText("backup_completed"));
            }

            logger.UpdateState("Backup Operation", "All backups completed");
            Console.WriteLine("\n" + LanguageManager.GetText("backup_process_completed"));
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
                Console.WriteLine(LanguageManager.GetText("invalid_selection_format"));
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
                Console.Write(LanguageManager.GetText("backup_type"));
                string backupType = Console.ReadLine().Trim().ToLower();

                bool isDifferential = backupType == "d";

                Console.WriteLine("\n" + LanguageManager.GetText("available_backup"));
                foreach (var backup in backupJobs)
                {
                    Console.WriteLine($"- {backup.name}");
                }

                string backupName;
                Backup selectedBackup = null;
                do
                {
                    Console.Write("\n" + LanguageManager.GetText("enter_name_backup_recover"));
                    backupName = Console.ReadLine().Trim();
                    selectedBackup = backupJobs.FirstOrDefault(b => b.name.Equals(backupName, StringComparison.OrdinalIgnoreCase));

                    if (selectedBackup == null)
                    {
                        Console.WriteLine(LanguageManager.GetText("backup_name_not_found"));
                    }
                } while (selectedBackup == null);

                Console.WriteLine("\n" + LanguageManager.GetText("starting_recovery"));
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
                            Console.WriteLine($"{LanguageManager.GetText("file_recovery")} '{fileName}' {LanguageManager.GetText("file_restored")}");

                        }
                        else
                        {
                            Console.WriteLine($"{LanguageManager.GetText("error_file")} '{fileName}' {LanguageManager.GetText("not_found_in_backup")}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{LanguageManager.GetText("error_failed_recover")} '{fileName}': {ex.Message}");

                    }
                }

                Console.Write("\n" + LanguageManager.GetText("recovery_another"));
                continueRecovery = Console.ReadLine().Trim().ToLower() == "o";
            } while (continueRecovery);
        }

        private void LoadData()
        {
            // TODO
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void StartSauvegarde()
        {
            Timer timer = new Timer(GenererJson, null, 0, 200); // Mise à jour toutes les 5 sec
            //Console.WriteLine(LanguageManager.GetText("log_state_update"));
            //Console.ReadLine();
        }

        private void GenererJson(object? state)
        {
            string filePath = "state.json";
            List<EtatSauvegarde> sauvegardes = new();

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
                    Console.WriteLine(LanguageManager.GetText("log_state_error_json"));
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

            // Sérialisation et écriture dans le fichier JSON avec verrou
            try
            {
                string json = JsonSerializer.Serialize(sauvegardes, new JsonSerializerOptions { WriteIndented = true });
                
                lock (_lockFile)
                {
                    // Utilisation de FileShare.ReadWrite pour permettre la lecture pendant l'écriture
                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(json);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{LanguageManager.GetText("log_state_error_writting_json")} {ex.Message}");
            }
        }
        public void ShowLogs()
        {
            ViewLogs.ShowLogs();
        }
    }

}
