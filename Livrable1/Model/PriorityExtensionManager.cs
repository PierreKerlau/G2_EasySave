using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Livrable1.Model;

public class PriorityExtensionManager
{
    private static PriorityExtensionManager _instance; // Singleton instance
    private HashSet<string> _priorityExtensions = new HashSet<string>(); // Set of priority file extensions

    // Singleton instance accessor
    public static PriorityExtensionManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new PriorityExtensionManager();
            return _instance;
        }
    }

    // Public accessor for the priority extensions set
    public HashSet<string> PriorityExtensions => _priorityExtensions;

    // Adds or removes an extension from the priority list based on the isPriority flag
    public void UpdatePriorityExtension(string extension, bool isPriority)
    {
        if (isPriority)
            _priorityExtensions.Add(extension.ToLower()); // Add the extension to the set
        else
            _priorityExtensions.Remove(extension.ToLower()); // Remove the extension from the set
    }

    // Checks if a directory contains any files with priority extensions
    public bool HasPriorityFiles(string directoryPath)
    {
        // Return false if the directory does not exist or if there are no priority extensions defined
        if (!Directory.Exists(directoryPath) || !_priorityExtensions.Any())
            return false;

        // Check if any file in the directory (including subdirectories) has a priority extension
        return Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories)
            .Any(file => _priorityExtensions.Contains(Path.GetExtension(file).ToLower()));
    }

    // Checks if any backup contains files with priority extensions
    public bool HasPendingPriorityFiles(List<SaveInformation> backups)
    {
        // Return false if there are no priority extensions defined
        if (!_priorityExtensions.Any())
            return false;

        // Iterate through backups and check if any source path contains priority files
        foreach (var backup in backups)
        {
            if (HasPriorityFiles(backup.SourcePath))
                return true;
        }
        return false;
    }
}
