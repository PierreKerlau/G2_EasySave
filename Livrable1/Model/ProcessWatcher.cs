using System;
using Livrable1.ViewModel;

namespace Livrable1.Model
{
    public class ProcessWatcher
    {
        private static ProcessWatcher _instanceWatch; // Singleton instance
        private static readonly object _lock = new object(); // Lock object for thread safety

        // Indicates whether Cheat Engine should be blocked
        public bool BloquerNotepad { get; set; }

        // Indicates whether Wireshark should be blocked
        public bool BloquerCalculator { get; set; }

        // Private constructor to prevent external instantiation
        private ProcessWatcher() { }

        // Singleton instance accessor with thread safety
        public static ProcessWatcher Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instanceWatch == null)
                        _instanceWatch = new ProcessWatcher();
                    return _instanceWatch;
                }
            }
        }
    }
}
