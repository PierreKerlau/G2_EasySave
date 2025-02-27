using System.Collections.Generic;
using System.Threading;
using Livrable1.Model;

namespace Livrable1.ViewModel
{
    public class SaveManager
    {
        private static SaveManager instance; // Singleton instance of SaveManager
        private List<SaveInformation> backups; // List to hold backup information
        private Dictionary<string, ManualResetEventSlim> _pauseEvents; // Pause control for each backup
        private Dictionary<string, CancellationTokenSource> _cancellationTokens; // Cancellation control for each backup

        // Private constructor to prevent instantiation from outside
        private SaveManager()
        {
            backups = new List<SaveInformation>(); // Initialize the backups list
            _pauseEvents = new Dictionary<string, ManualResetEventSlim>(); // Initialize pause events dictionary
            _cancellationTokens = new Dictionary<string, CancellationTokenSource>(); // Initialize cancellation tokens dictionary
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

        // Method to pause a specific backup
        public void PauseBackup(SaveInformation backup)
        {
            if (!_pauseEvents.ContainsKey(backup.NameSave))
            {
                _pauseEvents[backup.NameSave] = new ManualResetEventSlim(true); // Initialize a new event for pause
            }
            _pauseEvents[backup.NameSave].Reset(); // Reset the event to pause the backup
        }

        // Method to resume a paused backup
        public void ResumeBackup(SaveInformation backup)
        {
            if (!_pauseEvents.ContainsKey(backup.NameSave))
            {
                _pauseEvents[backup.NameSave] = new ManualResetEventSlim(true); // Initialize if not exists
            }
            _pauseEvents[backup.NameSave].Set(); // Set the event to resume the backup
        }

        // Method to stop a specific backup
        public void StopBackup(SaveInformation backup)
        {
            // Cancel the backup if it's running
            if (_cancellationTokens.ContainsKey(backup.NameSave))
            {
                _cancellationTokens[backup.NameSave].Cancel(); // Cancel the operation
                _cancellationTokens.Remove(backup.NameSave); // Remove the token
            }
            // Ensure the backup is resumed (if paused)
            if (_pauseEvents.ContainsKey(backup.NameSave))
            {
                _pauseEvents[backup.NameSave].Set(); // Resume the event if paused
            }
        }

        // Method to delete a specific backup
        public void DeleteBackup(SaveInformation backup)
        {
            backups.Remove(backup); // Remove the backup from the list
        }

        // Method to get the list of backups
        public List<SaveInformation> GetBackups()
        {
            return backups; // Return the list of backups
        }
    }
}
