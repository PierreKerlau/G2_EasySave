using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livrable1.Model
{
    public class EtatSauvegarde
    {
        public string Appellation { get; set; } = "";
        public DateTime HorodatageDerniereAction { get; set; }
        public string Etat { get; set; } = "Non Actif"; // Actif / Terminé
        public int NombreTotalFichiers { get; set; } = 0;
        public int TailleTotaleFichiersMB { get; set; } = 0;
        public int Progression { get; set; } = 0;
        public int NombreFichiersRestants { get; set; } = 0;
        public int TailleFichiersRestantsMB { get; set; } = 0;
        public string FichierSource { get; set; } = "";
        public string FichierDestination { get; set; } = "";
    }
}
