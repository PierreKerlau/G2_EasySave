using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livrable1.Modele
{
    public class Sauvegarde
    {
        public string nom { get; set; }
        public string type { get; set; }
        public string cheminSource { get; set; }
        public string cheminDestination { get; set; }
        public DateTime dateCreation { get; set; }

        public Sauvegarde(string nom, string cheminSource, string cheminDestination)
        {
            this.nom = nom;
            this.cheminSource = cheminSource;
            this.cheminDestination = cheminDestination;
            this.dateCreation = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{dateCreation}] {nom} - De {cheminSource} vers {cheminDestination}";
        }
    }
}
