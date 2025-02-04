using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livrable1.Modele
{
    internal class Langue
    {
        public string nom { get; set; }
        public int idLangue { get; set; }

        public Langue(string nom, int idLangue)
        {
            this.nom = nom;
            this.idLangue = idLangue;
        }

        public override string ToString()
        {
            return $"Langue : {nom} - Id : {idLangue}";
        }
    } 
}
