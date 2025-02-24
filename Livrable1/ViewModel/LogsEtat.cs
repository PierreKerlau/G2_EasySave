using Livrable1.Model;
using System;
using System.Collections.Generic;
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

            // Vérifier si le fichier d'état existe
            if (!File.Exists(stateFilePath))
            {
                Console.WriteLine("Le fichier d'état n'existe pas.");
                return states; // Retourner une liste vide si le fichier n'existe pas
            }

            try
            {

                using (var stream = /*stateFile.OpenRead()*/ File.OpenRead(stateFilePath))
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
                                    //----------------------------------------------------------------------------------------//
                                    DateTime lastTimeStamp = element.GetProperty("lastTimeStamp").GetDateTime();
                                    //DateTime lastTimeStamp = element.TryGetProperty("date", out var dateProp) ? dateProp.GetDateTime() : DateTime.MinValue;
                                    bool isActive = element.GetProperty("Active").GetBoolean();
                                    long totalSize = element.GetProperty("totalSize").GetInt64();
                                    int remainingFiles = element.GetProperty("remainingFiles").GetInt32();
                                    long remainingSize = element.GetProperty("remainingSize").GetInt64();
                                    //----------------------------------------------------------------------------------------//

                                    if (saveName != null && !string.IsNullOrEmpty(sourcePath) && !string.IsNullOrEmpty(destinationPath))
                                    {
                                        SaveInformation save = new(
                                            saveName,
                                            sourcePath,
                                            destinationPath,
                                            numberFile,
                                            lastTimeStamp,
                                            isActive,
                                            totalSize,
                                            remainingFiles,
                                            remainingSize
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
                remainingSize = save.RemainingSize
            }).ToList();

            string json = JsonSerializer.Serialize(savesToStore, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(STATE_FILE_PATH, json);

        }


        public static void LoadSaves()
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

    }
}
