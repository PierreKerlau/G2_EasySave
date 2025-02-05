using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livrable1.Model
{
    internal class Language
    {
        public string name { get; set; }
        public int languageId { get; set; }

        public Language(string name, int languageId)
        {
            this.name = name;
            this.languageId = languageId;
        }

        public override string ToString()
        {
            return $"Language : {name} - Id : {languageId}";
        }
    } 
}
