using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Livrable1.Controller
{
    public static class BusinessSoftwareManager
    {
        private static readonly HashSet<string> _monitoredProcesses = new HashSet<string>
        {
            "notepad",
            "calc"
        };

        private static readonly HashSet<string> _activeProcesses = new HashSet<string>();

        public static bool CanStartBackup()
        {
            UpdateActiveProcesses();
            return _activeProcesses.Count == 0;
        }

        public static void UpdateActiveProcesses()
        {
            _activeProcesses.Clear();
            Process[] processes = Process.GetProcesses();
            
            foreach (Process process in processes)
            {
                if (_monitoredProcesses.Contains(process.ProcessName.ToLower()))
                {
                    _activeProcesses.Add(process.ProcessName.ToLower());
                }
            }
        }

        public static string GetBlockingProcesses()
        {
            return string.Join(", ", _activeProcesses);
        }
    }
} 