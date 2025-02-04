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
        private const string FILE_PATH_SAUVEGARDE = "sauvegardes.json";
        public CtrlSauvegarde()
        {
            ChargerDonnees();
        }

        public void AjouterTravail()
        {
            VueAjouterTravail.AjouterTravail();
            Console.WriteLine("Saisir une valeur");
            var valeur = Console.ReadLine();
            Console.WriteLine(valeur);
        }

        public void ExecuterSauvegarde()
        {
            VueExecuterSauvegarde.ExecuterSauvegarde();
        }

        public void RecupererSauvegarde()
        {
            VueRecupererSauvegarde.RecupererSauvegarde();
        }

        public void ChoisirLangue()
        {
            VueChoisirLangue.ChoisirLangue();
        }

        public void VoirLogs()
        {
            VueChoisirLangue.ChoisirLangue();
        }

        public void ChargerDonnees()
        {
            try
            {
                if (File.Exists(FILE_PATH_SAUVEGARDE))
                {
                    string json = File.ReadAllText(FILE_PATH_SAUVEGARDE);
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

