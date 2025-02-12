//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Livrable1.Model
//{
//    public class Backup
//    {
//        public string name { get; set; }
//        public string type { get; set; }
//        public string sourcePath { get; set; }
//        public string destinationPath { get; set; }
//        public DateTime creationDate { get; set; }
//        public bool isDirectory { get; set; }

//        public Backup(string name, string sourcePath, string destinationPath)
//        {
//            this.name = name;
//            this.sourcePath = sourcePath;
//            this.destinationPath = destinationPath;
//            this.creationDate = DateTime.Now;
//            this.isDirectory = Directory.Exists(sourcePath);
//        }

//        public override string ToString()
//        {
//            string type = isDirectory ? "Folder" : "File";
//            return $"[{creationDate}] {type}: {name} - From {sourcePath} to {destinationPath}";
//        }
//    }
//}
