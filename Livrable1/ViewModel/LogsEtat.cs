using Livrable1.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Livrable1.ViewModel
{
    public class EtatSauvegarde
    {
        // Path to the state file
        private string STATE_FILE_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Logs", "state.json").ToString();
        private FileInfo stateFile;

        public ObservableCollection<SaveInformation> Backups { get; set; }

        public EtatSauvegarde()
        {
            stateFile = new FileInfo(STATE_FILE_PATH);
            // Create the state file if it doesn't exist
            if (!stateFile.Exists || stateFile.Length == 0)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(STATE_FILE_PATH));
                File.WriteAllText(STATE_FILE_PATH, "[]");
            }
        }

        // Reads the state from the specified file path
        public static List<SaveInformation> ReadState(string stateFilePath)
        {
            var states = new List<SaveInformation>();

            if (!File.Exists(stateFilePath))
            {
                return states; // Return an empty list if the file does not exist
            }

            try
            {
                using (var stream = File.OpenRead(stateFilePath))
                using (var reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();

                    if (string.IsNullOrEmpty(json))
                    {
                        return new List<SaveInformation>(); // Return empty list if JSON is empty
                    }

                    using (var jsonDocument = JsonDocument.Parse(json))
                    {
                        var root = jsonDocument.RootElement;

                        if (root.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var element in root.EnumerateArray())
                            {
                                try
                                {
                                    // Extract properties from the JSON element
                                    string? saveName = element.GetProperty("name").GetString();
                                    string? sourcePath = element.GetProperty("realDirectoryPath").GetString();
                                    string? destinationPath = element.GetProperty("copyDirectoryPath").GetString();
                                    int numberFile = element.GetProperty("numberFile").GetInt32();
                                    DateTime lastTimeStamp = element.GetProperty("lastTimeStamp").GetDateTime();
                                    bool isActive = element.GetProperty("Active").GetBoolean();
                                    long totalSize = element.GetProperty("totalSize").GetInt64();
                                    int remainingFiles = element.GetProperty("remainingFiles").GetInt32();
                                    long remainingSize = element.GetProperty("remainingSize").GetInt64();

                                    // Validate extracted properties before creating SaveInformation object
                                    if (!string.IsNullOrEmpty(saveName) && !string.IsNullOrEmpty(sourcePath) && !string.IsNullOrEmpty(destinationPath))
                                    {
                                        var files = new List<FileInformation>(); // Initialize the list of FileInformation
                                        var filesArray = element.GetProperty("files").EnumerateArray(); // Assuming you have a "files" array in your JSON

                                        // Create FileInformation objects for each file in the files array
                                        foreach (var fileElement in filesArray)
                                        {
                                            string filePath = fileElement.GetString();
                                            files.Add(new FileInformation(filePath)); // Create FileInformation object for each file
                                        }

                                        // Create SaveInformation object and add it to the states list
                                        SaveInformation save = new(
                                            saveName,
                                            sourcePath,
                                            destinationPath,
                                            numberFile,
                                            lastTimeStamp,
                                            isActive,
                                            totalSize,
                                            remainingFiles,
                                            remainingSize,
                                            files // Pass the list of files
                                        );

                                        states.Add(save);
                                    }
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(e.Message); // Show error message if an exception occurs
                                }
                            }
                            return states; // Return the list of SaveInformation
                        }
                    }
                    return new List<SaveInformation>(); // Return empty list if root is not an array
                }
            }
            catch
            {
                return new List<SaveInformation>(); // Return empty list if an error occurs
            }
        }

        // Writes the current state to the state file
        public void WriteState(List<SaveInformation> saves)
        {

            try
            {
                // Load existing saves
                List<SaveInformation> existingSaves = ReadState(STATE_FILE_PATH);

                // Add new saves without duplicates
                foreach (var newSave in saves)
                {
                    if (!existingSaves.Any(s => s.NameSave == newSave.NameSave))
                    {
                        existingSaves.Add(newSave);
                    }
                }

                // Convert the list to JSON and save it
                var savesToStore = existingSaves.Select(save => new
                {
                    name = save.NameSave,
                    realDirectoryPath = save.SourcePath,
                    copyDirectoryPath = save.DestinationPath,
                    numberFile = save.NumberFile,
                    lastTimeStamp = save.Date,
                    Active = save.IsActive,
                    totalSize = save.TotalSize,
                    remainingFiles = save.RemainingFiles,
                    remainingSize = save.RemainingSize,
                    files = save.Files.Select(f => f.FilePath) // Serialize the list of files
                }).ToList();

                string json = JsonSerializer.Serialize(savesToStore, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(STATE_FILE_PATH, json); // Write the JSON to the state file
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing to file: {ex.Message}"); // Show error message if an exception occurs
            }
        }

        // Updates the save state for a specific save
        public void UpdateSaveState(string nameSave, string filePath, int remainingFiles, long remainingSize)
        {
            // Read the current state
            List<SaveInformation> saves = ReadState(STATE_FILE_PATH);

            // Find the save to update
            var saveToUpdate = saves.FirstOrDefault(s => s.NameSave == nameSave);

            if (saveToUpdate != null)
            {
                // Find the exact file to transfer
                var fileToTransfer = saveToUpdate.Files
                    .FirstOrDefault(f => Path.GetFullPath(f.FilePath).Equals(Path.GetFullPath(filePath), StringComparison.OrdinalIgnoreCase));

                if (fileToTransfer == null)
                {
                    MessageBox.Show($"File not found in the save: {filePath}");
                    return;
                }

                // Retrieve the size of the file
                long fileSize = 0;
                if (File.Exists(fileToTransfer.FilePath))
                {
                    fileSize = new FileInfo(fileToTransfer.FilePath).Length; // Get the size of the file
                }
                else if (fileToTransfer.Size > 0)
                {
                    fileSize = fileToTransfer.Size; // Use stored size if available
                }
                else
                {
                    MessageBox.Show($"Unable to retrieve file size: {fileToTransfer.FilePath}");
                }

                // Safely decrement the values
                int newRemainingFiles = saveToUpdate.RemainingFiles - 1;
                long newRemainingSize = saveToUpdate.RemainingSize - fileSize;

                // Ensure values are not negative
                saveToUpdate.RemainingFiles = Math.Max(newRemainingFiles, 0);
                saveToUpdate.RemainingSize = Math.Max(newRemainingSize, 0);
                saveToUpdate.IsActive = true; // Mark the save as active

                // Update the JSON file without removing files
                var savesToStore = saves.Select(save => new
                {
                    name = save.NameSave,
                    realDirectoryPath = save.SourcePath,
                    copyDirectoryPath = save.DestinationPath,
                    numberFile = save.NumberFile,
                    lastTimeStamp = save.Date,
                    Active = save.IsActive,
                    totalSize = save.TotalSize,
                    remainingFiles = save.RemainingFiles,
                    remainingSize = save.RemainingSize,
                    files = save.Files.Select(f => f.FilePath) // Keep all files
                }).ToList();

                string json = JsonSerializer.Serialize(savesToStore, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(STATE_FILE_PATH, json); // Write updated state to the file
            }
            else
            {
                MessageBox.Show($"No save found with the name: {nameSave}");
            }
        }

        // Updates the active status of a save
        public void UpdateActive(string nameSave)
        {
            // Read the current state
            List<SaveInformation> saves = ReadState(STATE_FILE_PATH);

            // Find the save to update
            var saveToUpdate = saves.FirstOrDefault(s => s.NameSave == nameSave);

            if (saveToUpdate != null)
            {
                saveToUpdate.IsActive = false; // Set the save as inactive

                // Update the JSON file without removing files
                var savesToStore = saves.Select(save => new
                {
                    name = save.NameSave,
                    realDirectoryPath = save.SourcePath,
                    copyDirectoryPath = save.DestinationPath,
                    numberFile = save.NumberFile,
                    lastTimeStamp = save.Date,
                    Active = save.IsActive,
                    totalSize = save.TotalSize,
                    remainingFiles = save.RemainingFiles,
                    remainingSize = save.RemainingSize,
                    files = save.Files.Select(f => f.FilePath) // Keep all files
                }).ToList();

                string json = JsonSerializer.Serialize(savesToStore, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(STATE_FILE_PATH, json); // Write updated state to the file
            }
            else
            {
                MessageBox.Show($"No save found with the name: {nameSave}");
            }
        }
    }
}
