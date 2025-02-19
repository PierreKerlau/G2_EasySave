using System;
using Livrable1.ViewModel;


namespace Livrable1.Model
{
    public class ProcessWatcher
    {
        // Instance unique du Singleton
        private static ProcessWatcher _instanceWatch;
        private static readonly object _lock = new object();

        // Propriétés pour stocker l'état des CheckBox
        public bool BloquerCheatEngine { get; set; }
        public bool BloquerWireshark { get; set; }

        // Constructeur privé pour empêcher l'instanciation directe
        private ProcessWatcher() { }

        // Méthode pour récupérer l'instance unique
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