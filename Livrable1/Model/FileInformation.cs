using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//------------Model------------//
namespace Livrable1.Model
{
    //------------Class FileInformation------------//
    public class FileInformation
    {
        public string FilePath { get; set; } // Full file path
        public string FileName => Path.GetFileName(FilePath); // Extracts the file name from the file path
        public bool IsSelected { get; set; } // Indicates whether the file is selected

        public long Size => new FileInfo(FilePath).Length;

        // Constructor initializing the file path
        public FileInformation(string filePath)
        {
            FilePath = filePath;
        }

    }
    //------------Class FileInformation------------//
}
//------------Model------------//
