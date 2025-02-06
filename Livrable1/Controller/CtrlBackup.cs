using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Livrable1.Model;
using Livrable1.View;

namespace Livrable1.Controller
{
    public class CtrlBackup
    {
        private List<Backup> saves = new();
        private const string FILE_PATH_SAVE = "save.json";

        private const string SOURCE_FOLDER = @"C:\Users\pierr\Documents\CESI\CESI_A3\Bloc_2_Genie_Logiciel\Clonage\Source";
        private const string DESTINATION_FOLDER = @"C:\Users\pierr\Documents\CESI\CESI_A3\Bloc_2_Genie_Logiciel\Clonage\Destination";

        private List<Backup> backupJobs = new();


        public CtrlBackup()
        {
            LoadData();
        }

        public void AddBackup()
        {
            ViewAddBackup.AddBackup();

            if (!Directory.Exists(SOURCE_FOLDER))
            {
                Console.WriteLine("ERROR: Source folder does not exist!");
                return;
            }

            string[] files = Directory.GetFiles(SOURCE_FOLDER);
            if (files.Length == 0)
            {
                Console.WriteLine("No files found in the source folder.");
                return;
            }

            Console.WriteLine("\nAvailable files:");
            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine($"[{i + 1}] {Path.GetFileName(files[i])}");
            }

            Console.Write("\nSelect a file number to add to backup (or 0 to cancel): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= files.Length)
            {
                string selectedFile = files[choice - 1];

                backupJobs.Add(new Backup(Path.GetFileName(selectedFile), selectedFile, DESTINATION_FOLDER));
                Console.WriteLine($"File added to backup queue: {Path.GetFileName(selectedFile)}");
            }
            else
            {
                Console.WriteLine("Operation canceled.");
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
                Console.WriteLine($"[{i + 1}] {(backupJobs[i].isDirectory ? "Folder" : "File")} {backupJobs[i].name}");
            }

            Console.Write("\nEnter the files to backup (Press ? to open help note): ");
            string input = Console.ReadLine();

            List<int> selectedIndexes = ParseSelection(input, backupJobs.Count);

            if (selectedIndexes.Count == 0)
            {
                Console.WriteLine("No valid selection. Operation canceled.");
                return;
            }

            Console.WriteLine("Starting backup...");

            foreach (int index in selectedIndexes)
            {
                Backup backup = backupJobs[index - 1];
                string destinationPath = Path.Combine(DESTINATION_FOLDER, backup.name);

                try
                {
                    if (backup.isDirectory)
                    {
                        CopyDirectory(backup.sourcePath, destinationPath);
                        Console.WriteLine($"Folder backed up: {backup.name}");
                    }
                    else
                    {
                        File.Copy(backup.sourcePath, destinationPath, true);
                        Console.WriteLine($"File backed up: {backup.name}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: Failed to backup {backup.name}: {ex.Message}");
                }
            }

            backupJobs = backupJobs.Where((_, i) => !selectedIndexes.Contains(i + 1)).ToList();
            Console.WriteLine("Backup process completed!");
        }

        private void CopyDirectory(string sourceDir, string destDir)
        {
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
                CopyDirectory(dir, destSubDir);
            }
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
            try
            {
                if (File.Exists(FILE_PATH_SAVE))
                {
                    string json = File.ReadAllText(FILE_PATH_SAVE);
                    saves = JsonSerializer.Deserialize<List<Backup>>(json) ?? new List<Backup>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }
    }
}