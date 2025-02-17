using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;

namespace Livrable1.Model
{
    public class Backup : BaseViewModel
    {
        public string name { get; set; }
        public string type { get; set; }
        public string sourcePath { get; set; }
        public string destinationPath { get; set; }
        public DateTime creationDate { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public Backup() { }

        public Backup(string name, string sourcePath, string destinationPath)
        {
            this.name = name;
            this.sourcePath = sourcePath;
            this.destinationPath = destinationPath;
            this.creationDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{creationDate}] {type}: {name} - From {sourcePath} to {destinationPath}";
        }
    }
}