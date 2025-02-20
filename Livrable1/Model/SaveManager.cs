using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Livrable1.Model;

namespace Livrable1.ViewModel
{
    public class SaveManager
    {
        private static SaveManager instance;
        private List<SaveInformation> backups;
        private Dictionary<string, ManualResetEventSlim> _pauseEvents;
        private Dictionary<string, CancellationTokenSource> _cancellationTokens;

        private SaveManager()
        {
            backups = new List<SaveInformation>();
            _pauseEvents = new Dictionary<string, ManualResetEventSlim>();
            _cancellationTokens = new Dictionary<string, CancellationTokenSource>();
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
            return backups;
        }
    }
}