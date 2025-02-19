using System;
using Livrable1.ViewModel;

//------------Model------------//
namespace Livrable1.Model
{
    //------------Class ProcessWatcher------------//
    public class ProcessWatcher
    {
        private static ProcessWatcher _instanceWatch; // Singleton instance
        private static readonly object _lock = new object(); // Lock object for thread safety

        // Indicates whether Cheat Engine should be blocked
        public bool BloquerCheatEngine { get; set; }

        // Indicates whether Wireshark should be blocked
        public bool BloquerWireshark { get; set; }

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
    //------------Class ProcessWatcher------------//

}
//------------Model------------//