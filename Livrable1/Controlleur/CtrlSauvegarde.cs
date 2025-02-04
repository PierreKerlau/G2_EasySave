using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Livrable1.Vue;

namespace Livrable1.Controlleur
{
    public class CtrlSauvegarde
    {
        private List<CtrlSauvegarde> sauvegardes = new();
        private const string FilePathSauvegarde = "sauvegardes.json";
        public CtrlSauvegarde()
        {
            ChargerDonnees();
        }

        public void AjouterTravail()
        {
            AjouterTravail();
        }

        public void ExecuterSauvegarde()
        {
            Console.WriteLine("Test exécuter sauvegarde");
        }

        public void ChoisirLangue()
        {
            Console.WriteLine("Test choisir langue");
        }

        public void VoirLogs()
        {
            Console.WriteLine("Test voir logs");
        }

        public void ChargerDonnees()
        {
            try
            {
                if (File.Exists(FilePathSauvegarde))
                {
                    string json = File.ReadAllText(FilePathSauvegarde);
                    sauvegardes = JsonSerializer.Deserialize<List<CtrlSauvegarde>>(json) ?? new List<CtrlSauvegarde>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des données : {ex.Message}");
            }
        }
    }
}

