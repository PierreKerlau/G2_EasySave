using Livrable1.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        
        public List<SaveInformation> ReadState()
        {
            
            using (var stream = stateFile.OpenRead())
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
                        var states = new List<SaveInformation>();

                        foreach (var element in root.EnumerateArray())
                        {
                            try
                            {
                                string? saveName = element.GetProperty("name").GetString();
                                string? sourcePath = element.GetProperty("realDirectoryPath").GetString();
                                string? destinationPath = element.GetProperty("copyDirectoryPath").GetString();
                                int numberFile = element.GetProperty("numberFile").GetInt32();
                                //----------------------------------------------------------------------------------------//
                                DateTime lastTimeStamp = element.GetProperty("dernierHorodatage").GetDateTime();
                                bool isActive = element.GetProperty("actif").GetBoolean();
                                long totalSize = element.GetProperty("tailleTotale").GetInt64();
                                int remainingFiles = element.GetProperty("fichiersRestants").GetInt32();
                                long remainingSize = element.GetProperty("tailleRestante").GetInt64();
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
                                Console.WriteLine(e.Message);
                            }
                        }
                        return states;
                    }
                }
                return new List<SaveInformation>();
            }
        }

        public void WriteState(List<SaveInformation> saves)
        {
            var savesToStore = saves.Select(save => new
            {
                name = save.NameSave,
                realDirectoryPath = save.SourcePath,
                copyDirectoryPath = save.DestinationPath,
                numberfile = save.NumberFile,
                date = save.Date,
                isActive = save.IsActive,
                totalSize = save.TotalSize,
                remainingFiles = save.RemainingFiles,
                remainingSize = save.RemainingSize
            }).ToList();

            string json = JsonSerializer.Serialize(savesToStore, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(STATE_FILE_PATH, json);

        }
    }
}
