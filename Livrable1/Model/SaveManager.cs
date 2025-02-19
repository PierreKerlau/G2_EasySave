using System.Collections.Generic;
using Livrable1.Model;

//---------------------Model---------------------//
namespace Livrable1.ViewModel
{
    //------------Class SaveManager------------//
    public class SaveManager
    {
        private static SaveManager instance; // Singleton instance of SaveManager
        private List<SaveInformation> backups; // List to hold backup information

        // Private constructor to prevent instantiation from outside
        private SaveManager()
        {
            backups = new List<SaveInformation>(); // Initialize the backups list
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

        // Method to get the list of backups
        public List<SaveInformation> GetBackups()
        {
            return backups; // Return the list of backups
        }
    }
    //------------Class SaveManager------------//
}
//---------------------Model---------------------//