using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Livrable1.Model;

//---------------------Model---------------------//
namespace Livrable1.ViewModel
{
    //------------Class SaveManager------------//
    public class SaveManager
    {
        private static SaveManager instance;
        private List<SaveInformation> backups;
        private Dictionary<string, ManualResetEventSlim> _pauseEvents;
        private Dictionary<string, CancellationTokenSource> _cancellationTokens;

        // Private constructor to prevent instantiation from outside
        private SaveManager()
        {
            backups = new List<SaveInformation>();
            _pauseEvents = new Dictionary<string, ManualResetEventSlim>();
            _cancellationTokens = new Dictionary<string, CancellationTokenSource>();
        }

        // Public property to access the singleton instance of SaveManager
        public static SaveManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SaveManager(); // Create a new instance if it doesn't exist
                }
                return instance; // Return the singleton instance
            }
        }

        // Method to add a backup to the list
        public void AddBackup(SaveInformation backup)
        {
            backups.Add(backup); // Add the provided backup to the backups list
        }

        public void PauseBackup(SaveInformation backup)
        {
            if (!_pauseEvents.ContainsKey(backup.NameSave))
            {
                _pauseEvents[backup.NameSave] = new ManualResetEventSlim(true);
            }
            _pauseEvents[backup.NameSave].Reset();
        }

        public void ResumeBackup(SaveInformation backup)
        {
            if (!_pauseEvents.ContainsKey(backup.NameSave))
            {
                _pauseEvents[backup.NameSave] = new ManualResetEventSlim(true);
            }
            _pauseEvents[backup.NameSave].Set();
        }

        public void StopBackup(SaveInformation backup)
        {
            if (_cancellationTokens.ContainsKey(backup.NameSave))
            {
                _cancellationTokens[backup.NameSave].Cancel();
                _cancellationTokens.Remove(backup.NameSave);
            }
            if (_pauseEvents.ContainsKey(backup.NameSave))
            {
                _pauseEvents[backup.NameSave].Set(); // Release any waiting pause
            }
        }

        public void DeleteBackup(SaveInformation backup)
        {
            backups.Remove(backup);
        }
        public List<SaveInformation> GetBackups()
        {
            return backups; // Return the list of backups
        }
    }
}
