using System.Windows;

//---------------------ViewModel---------------------//
namespace Livrable1.ViewModel
{
    //------------Class StateViewModel------------//
    public static class StateViewModel
    {
        // Business application states
        public static bool IsCalculatorEnabled { get; set; } = false;
        public static bool IsNotePadEnabled { get; set; } = false;

        // File extension encryption states
        public static bool IsPdfEnabled { get; set; } = false;
        public static bool IsPngEnabled { get; set; } = false;
        public static bool IsJsonEnabled { get; set; } = false;
        public static bool IsXmlEnabled { get; set; } = false;
        public static bool IsDocxEnabled { get; set; } = false;
        public static bool IsTxtEnabled { get; set; } = false;

        // Additional state flags for JSON and XML (e.g., for logging or output options)
        public static bool IsJsonOn { get; set; } = true;
        public static bool IsXmlOn { get; set; } = false;

        // Method to update encryption settings for a specific extension
        public static void UpdateExtensionEncryption(string extension, bool isEnabled)
        {
            if (isEnabled)
            {
                MessageBox.Show($"Extension {extension} ajoutée pour le cryptage"); // Show message when the extension is enabled for encryption
            }
            else
            {
                MessageBox.Show($"Extension {extension} retirée du cryptage"); // Show message when the extension is disabled for encryption
            }
        }
    }
    //------------Class StateViewModel------------//
}
//---------------------ViewModel---------------------//