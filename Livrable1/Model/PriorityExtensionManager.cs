using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Livrable1.Model;

public class PriorityExtensionManager
{
    private static PriorityExtensionManager _instance;
    private HashSet<string> _priorityExtensions = new HashSet<string>();

    public static PriorityExtensionManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new PriorityExtensionManager();
            return _instance;
        }
    }

    public HashSet<string> PriorityExtensions => _priorityExtensions;

    public void UpdatePriorityExtension(string extension, bool isPriority)
    {
        if (isPriority)
            _priorityExtensions.Add(extension.ToLower());
        else
            _priorityExtensions.Remove(extension.ToLower());
    }

    public bool HasPriorityFiles(string directoryPath)
    {
        if (!Directory.Exists(directoryPath) || !_priorityExtensions.Any())
            return false;

        return Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories)
            .Any(file => _priorityExtensions.Contains(Path.GetExtension(file).ToLower()));
    }

    public bool HasPendingPriorityFiles(List<SaveInformation> backups)
    {
        if (!_priorityExtensions.Any())
            return false;

        foreach (var backup in backups)
        {
            if (HasPriorityFiles(backup.SourcePath))
                return true;
        }
        return false;
    }
} 