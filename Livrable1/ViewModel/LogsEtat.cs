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
        private string STATE_FILE_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Logs", "state.json").ToString();
        private FileInfo stateFile;

        public ObservableCollection<SaveInformation> Backups { get; set; }

        public EtatSauvegarde()
        {
            stateFile = new FileInfo(STATE_FILE_PATH);
            if (!stateFile.Exists || stateFile.Length == 0)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(STATE_FILE_PATH));
                File.WriteAllText(STATE_FILE_PATH, "[]");
            }
        }

        public static List<SaveInformation> ReadState(string stateFilePath)
        {
            var states = new List<SaveInformation>();

            if (!File.Exists(stateFilePath))
            {
                return states; // Retourner une liste vide si le fichier n'existe pas
            }

            try
            {
                using (var stream = File.OpenRead(stateFilePath))
                using (var reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();

                    if (string.IsNullOrEmpty(json))
                    {
                        return new List<SaveInformation>();
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
                                    string? saveName = element.GetProperty("name").GetString();
                                    string? sourcePath = element.GetProperty("realDirectoryPath").GetString();
                                    string? destinationPath = element.GetProperty("copyDirectoryPath").GetString();
                                    int numberFile = element.GetProperty("numberFile").GetInt32();
                                    DateTime lastTimeStamp = element.GetProperty("lastTimeStamp").GetDateTime();
                                    bool isActive = element.GetProperty("Active").GetBoolean();
                                    long totalSize = element.GetProperty("totalSize").GetInt64();
                                    int remainingFiles = element.GetProperty("remainingFiles").GetInt32();
                                    long remainingSize = element.GetProperty("remainingSize").GetInt64();

                                    if (!string.IsNullOrEmpty(saveName) && !string.IsNullOrEmpty(sourcePath) && !string.IsNullOrEmpty(destinationPath))
                                    {
                                        var files = new List<FileInformation>(); // Initialize the list of FileInformation
                                        var filesArray = element.GetProperty("files").EnumerateArray(); // Assuming you have a "files" array in your JSON

                                        foreach (var fileElement in filesArray)
                                        {
                                            string filePath = fileElement.GetString();
                                            files.Add(new FileInformation(filePath)); // Create FileInformation object for each file
                                        }

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
                                    MessageBox.Show(e.Message);
                                }
                            }
                            return states;
                        }
                    }
                    return new List<SaveInformation>();
                }
            }
            catch
            {
                return new List<SaveInformation>();
            }
        }

        public void WriteState(List<SaveInformation> saves)
        {
            // Afficher le chemin pour débogage
            MessageBox.Show($"Écriture dans le fichier : {STATE_FILE_PATH}");

            try
            {
                // Charger les sauvegardes existantes
                List<SaveInformation> existingSaves = ReadState(STATE_FILE_PATH);
                
                // Ajouter les nouvelles sauvegardes sans dupliquer
                foreach (var newSave in saves)
                {
                    if (!existingSaves.Any(s => s.NameSave == newSave.NameSave))
                    {
                        existingSaves.Add(newSave);
                    }
                }

                // Convertir en JSON et sauvegarder
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

                // Vérifiez le JSON avant l'écriture
                MessageBox.Show($"JSON à écrire : {json}");
                File.WriteAllText(STATE_FILE_PATH, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'écriture dans le fichier : {ex.Message}");
            }
        }













        public void UpdateSaveState(string nameSave, string filePath, int remainingFiles, long remainingSize)
        {
            // Lire l'état actuel
            List<SaveInformation> saves = ReadState(STATE_FILE_PATH);

            // Trouver la sauvegarde à modifier
            var saveToUpdate = saves.FirstOrDefault(s => s.NameSave == nameSave);

            if (saveToUpdate != null)
            {
                // Recherche du fichier exact
                var fileToTransfer = saveToUpdate.Files
                    .FirstOrDefault(f => Path.GetFullPath(f.FilePath).Equals(Path.GetFullPath(filePath), StringComparison.OrdinalIgnoreCase));

                if (fileToTransfer == null)
                {
                    MessageBox.Show($"Fichier non trouvé dans la sauvegarde : {filePath}");
                    return;
                }

                // Récupération de la taille du fichier
                long fileSize = 0;
                if (File.Exists(fileToTransfer.FilePath))
                {
                    fileSize = new FileInfo(fileToTransfer.FilePath).Length;
                }
                else if (fileToTransfer.Size > 0)
                {
                    fileSize = fileToTransfer.Size; // Utiliser la taille stockée si elle est disponible
                }
                else
                {
                    MessageBox.Show($"Impossible d'obtenir la taille du fichier : {fileToTransfer.FilePath}");
                }

                // Décrémentation sécurisée des valeurs
                int newRemainingFiles = saveToUpdate.RemainingFiles - 1;
                long newRemainingSize = saveToUpdate.RemainingSize - new FileInfo(fileToTransfer.FilePath).Length;

                // Vérification pour éviter d'écraser avec une ancienne valeur
                if (newRemainingFiles > 0)
                {
                    saveToUpdate.RemainingFiles = newRemainingFiles;
                }
                else
                {
                    saveToUpdate.RemainingFiles = 0;
                }

                if (newRemainingSize < saveToUpdate.RemainingSize)
                {
                    if (newRemainingSize > 0)
                    {
                        saveToUpdate.RemainingSize = newRemainingSize;
                    }
                    else
                    {
                        saveToUpdate.RemainingSize = 0;
                    }
                }

                saveToUpdate.IsActive = true;




                // Afficher les nouvelles valeurs pour débogage
                MessageBox.Show($"Mise à jour : RemainingFiles = {saveToUpdate.RemainingFiles}, RemainingSize = {saveToUpdate.RemainingSize}");




                // Mise à jour du fichier JSON (sans supprimer les fichiers)
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
                    files = save.Files.Select(f => f.FilePath) // On garde tous les fichiers
                }).ToList();

                string json = JsonSerializer.Serialize(savesToStore, new JsonSerializerOptions { WriteIndented = true });

                // Vérifier le JSON avant l'écriture


                MessageBox.Show($"JSON à écrire : {json}");


                File.WriteAllText(STATE_FILE_PATH, json);
            }
            else
            {
                MessageBox.Show($"Aucune sauvegarde trouvée avec le nom : {nameSave}");
            }
        }








        public void UpdateActive(string nameSave)
        {
            // Lire l'état actuel
            List<SaveInformation> saves = ReadState(STATE_FILE_PATH);

            // Trouver la sauvegarde à modifier
            var saveToUpdate = saves.FirstOrDefault(s => s.NameSave == nameSave);

            if (saveToUpdate != null)
            {
                saveToUpdate.IsActive = false;

                // Afficher les nouvelles valeurs pour débogage
                MessageBox.Show($"Mise à jour : RemainingFiles = {saveToUpdate.RemainingFiles}, RemainingSize = {saveToUpdate.RemainingSize}");

                // Mise à jour du fichier JSON (sans supprimer les fichiers)
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
                    files = save.Files.Select(f => f.FilePath) // On garde tous les fichiers
                }).ToList();

                string json = JsonSerializer.Serialize(savesToStore, new JsonSerializerOptions { WriteIndented = true });

                // Vérifier le JSON avant l'écriture
                MessageBox.Show($"JSON à écrire : {json}");
                File.WriteAllText(STATE_FILE_PATH, json);
            }
            else
            {
                MessageBox.Show($"Aucune sauvegarde trouvée avec le nom : {nameSave}");
            }
        }




    }
}
