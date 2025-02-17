namespace Livrable1.ViewModel
{
    public static class StateManager
    {
        // États des logiciels métier
        public static bool IsCalculatorEnabled { get; set; } = false;
        public static bool IsNotePadEnabled { get; set; } = false;

        // États des extensions
        public static bool IsPdfEnabled { get; set; } = false;
        public static bool IsPngEnabled { get; set; } = false;
        public static bool IsJsonEnabled { get; set; } = false;
        public static bool IsXmlEnabled { get; set; } = false;
        public static bool IsDocxEnabled { get; set; } = false;
        public static bool IsJsonOn { get; set; } = false;
        public static bool IsXmlOn { get; set; } = false;
    }
} 