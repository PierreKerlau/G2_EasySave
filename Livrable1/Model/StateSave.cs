using Livrable1.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



namespace Livrable1.Model
{

    public class Save
    {
        public string Appellation { get; set; } = "";
        public string FichierSource { get; set; } = "";
        public string FichierDestination { get; set; } = "";
        public int nbrFichier { get; set; } = 0;

        public Save(string Name, string FichierS, string FichierD, int nbrF) 
        {

            Appellation = Name;
            FichierSource = FichierS;
            FichierDestination = FichierD;
            nbrFichier = nbrF;

        }

        
    }

    //--------------------------------------------------------------------------------------------------------------------------
    public class SaveManager
    {
        public List<Save> Saves = new();
        public EtatSauvegarde EtatSauvegarde;

        public SaveManager(EtatSauvegarde EtatSauvegarde)
        {
            this.EtatSauvegarde = EtatSauvegarde;
        }

        public bool CreateSave(int saveId)
        {
            throw new NotImplementedException("This feature hasn't been implemented yet");
        }

        public bool LoadSave(int saveId)
        {
            throw new NotImplementedException("This feature hasn't been implemented yet");
        }

        public void SaveState()
        {
            EtatSauvegarde.WriteState(Saves);
        }

    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public class EtatSauvegarde
    {
        // Définition du chemin du fichier de sauvegarde de l'état
        // Il pointe vers EasySave/State/state.json dans le répertoire du projet
        private string STATE_FILE_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Logs", "state.json").ToString();

        // Création d'un objet FileInfo pour gérer les opérations sur le fichier
        private FileInfo stateFile;

        // Référence au contrôleur principal du programme
        public CtrlBackup Controller;
        
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Constructeur de la classe
        public EtatSauvegarde(CtrlBackup controller)
        {
            this.Controller = controller; // Initialisation du contrôleur

            stateFile = new FileInfo(STATE_FILE_PATH); // Création de l'objet FileInfo associé au fichier JSON

            // Vérifie si le fichier JSON existe ou s'il est vide
            if (!stateFile.Exists || stateFile.Length == 0)
            {
                string directoryPath = @"../../../Logs";
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                // Si le fichier n'existe pas ou est vide, on le crée avec une liste vide
                using (var stream = stateFile.Create()) // Crée un nouveau fichier
                using (var writer = new StreamWriter(stream)) // Écrit dans le fichier
                {
                    writer.Write("[]"); // Initialise le fichier avec un tableau JSON vide "[]"
                }
            }
        }
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Méthode pour lire l'état des sauvegardes depuis le fichier JSON
        public List<Save> ReadState()
        {
            // Ouvre le fichier en mode lecture
            using (var stream = stateFile.OpenRead())
            using (var reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd(); // Lit tout le contenu du fichier JSON

                // Vérifie si le fichier est vide
                if (string.IsNullOrEmpty(json))
                {
                    return new List<Save>(); // Retourne une liste vide si aucun contenu n'est trouvé
                }

                // Analyse le fichier JSON
                using (var jsonDocument = JsonDocument.Parse(json))
                {
                    var root = jsonDocument.RootElement; // Récupère la racine du document JSON

                    // Vérifie si la racine est bien un tableau JSON
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        var states = new List<Save>(); // Liste pour stocker les sauvegardes

                        // Parcourt chaque élément du tableau JSON
                        foreach (var element in root.EnumerateArray())
                        {
                            try
                            {
                                // Récupération des autres propriétés de la sauvegarde
                                string? saveName = element.GetProperty("name").GetString();
                                string? sourcePath = element.GetProperty("realDirectoryPath").GetString();
                                string? destinationPath = element.GetProperty("copyDirectoryPath").GetString();
                                int fileNumber = element.GetProperty("fileNumber").GetInt32();

                                // Vérifie que les propriétés essentielles ne sont pas nulles ou vides
                                if (saveName != null && !string.IsNullOrEmpty(sourcePath) && !string.IsNullOrEmpty(destinationPath))
                                {
                                    // Création d'une instance de sauvegarde
                                    Save save = new(
                                        saveName,
                                        sourcePath,
                                        destinationPath,
                                        fileNumber
                                        );

                                    states.Add(save); // Ajoute l'objet sauvegarde à la liste
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                        return states; // Retourne la liste des sauvegardes lues depuis le fichier JSON
                    }
                }
                return new List<Save>(); // Retourne une liste vide si le fichier JSON ne contient pas de tableau
            }
        }
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Méthode pour écrire l'état des sauvegardes dans le fichier JSON
        public void WriteState(List<Save> saves)
        {
            var savesToStore = saves.Select(save => new
            {
                name = save.Appellation,
                realDirectoryPath = save.FichierSource,
                copyDirectoryPath = save.FichierDestination,
                fileNumber = save.nbrFichier
            }).ToList();

            string json = JsonSerializer.Serialize(savesToStore, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(STATE_FILE_PATH, json);

        }
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}

