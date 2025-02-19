using System.Windows;

namespace Livrable1.ViewModel
{
    public static class StateViewModel
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
        public static bool IsTxtEnabled { get; set; } = false;
        public static bool IsJsonOn { get; set; } = true;
        public static bool IsXmlOn { get; set; } = false;

        public static void UpdateExtensionEncryption(string extension, bool isEnabled)
        {
            if (isEnabled)
            {
                MessageBox.Show($"{LanguageManager.GetText("extension_name")} '{extension}' {LanguageManager.GetText("add_for_encryption")}");
            }
            else
            {
                MessageBox.Show($"{LanguageManager.GetText("extension_name")} '{extension}' {LanguageManager.GetText("supp_for_encryption")}");
            }
        }
    }
} 