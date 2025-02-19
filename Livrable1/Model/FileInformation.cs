using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Livrable1.Model
{
    public class FileInformation
    {
        //---------Variable---------//
        public string FilePath { get; set; }
        public string FileName => Path.GetFileName(FilePath);
        public bool IsSelected { get; set; } // Pour suivre la sélection
        //---------Variable---------//

        //---------Constructeur---------//
        public FileInformation(string filePath)
        {
            FilePath = filePath;
        }
        //---------Constructeur---------//

    }
}
