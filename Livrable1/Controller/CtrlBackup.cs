using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Livrable1.Model;
using Livrable1.View;

namespace Livrable1.Controller
{
    public class CtrlBackup
    {
        private List<Backup> backupJobs = new();
        private List<string> backupNames = new();

        public CtrlBackup()
        {
            LoadData();
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
                            if (isDifferential)
                            {
                                if (!File.Exists(destinationFile) || File.GetLastWriteTime(file) > File.GetLastWriteTime(destinationFile))
                                {
                                    File.Copy(file, destinationFile, true);
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
                                Console.WriteLine($"[FULL] {fileName} copied.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"ERROR: File '{fileName}' not found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ERROR: Failed to backup '{fileName}': {ex.Message}");
                    }
                }
            }

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
            ViewRecoverBackup.RecoverBackup();
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
    }
}
