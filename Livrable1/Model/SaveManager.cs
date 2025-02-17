using System.Collections.Generic;
using Livrable1.Model;

namespace Livrable1.ViewModel
{
    public class SaveManager
    {
        private static SaveManager instance;
        private List<SaveInformation> backups;

        private SaveManager()
        {
            backups = new List<SaveInformation>();
        }

        public static SaveManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SaveManager();
                }
                return instance;
            }
        }

        public void AddBackup(SaveInformation backup)
        {
            backups.Add(backup);
        }

        public List<SaveInformation> GetBackups()
        {
            return backups;
        }
    }
}
