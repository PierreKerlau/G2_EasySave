using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livrable1.Modele
{
    public class Sauvegarde
    {
        public string Nom { get; set; }
        public string CheminSource { get; set; }
        public string CheminDestination { get; set; }
        public DateTime DateCreation { get; set; }

        public Sauvegarde(string nom, string cheminSource, string cheminDestination)
        {
            Nom = nom;
            CheminSource = cheminSource;
            CheminDestination = cheminDestination;
            DateCreation = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{DateCreation}] {Nom} - De {CheminSource} vers {CheminDestination}";
        }
    }
}
