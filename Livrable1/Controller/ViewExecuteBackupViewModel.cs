using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Livrable1.Model;
using Livrable1.View;
using Livrable1.Controller;

namespace Livrable1.Controller
{    

    public class ViewExecuteBackupViewModel : BaseViewModel
    {
        private ObservableCollection<Backup> _save;

        public ObservableCollection<Backup> Save
        {
            get => _save;
            set
            {
                _save = value;
                OnPropertyChanged(nameof(Save));
            }
        }

        public ViewExecuteBackupViewModel()
        {
            // Exemple de données de Save
                Save = new ObservableCollection<Backup>
            {
                new Backup { name = "Save 1", sourcePath = "C:\\ProjetEasySave\\path\\source", destinationPath = "C:\\ProjetEasySave\\path\\destination", creationDate = DateTime.Now},
                new Backup { name = "Save 2", sourcePath = "C:\\ProjetEasySave\\path\\source", destinationPath = "C:\\ProjetEasySave\\path\\destination", creationDate = DateTime.Now.AddDays(-1) }
            };
        }
        //------------------Start Methods for Delete------------------//
        public void RemoveSelectBackup()
        {
            foreach (var backup in Save)
            {
                backup.IsSelected = false;
            }
        }
        //-------------------End Methods for Delete-------------------//
    }
}
