using System.Windows;
using Livrable1.Model;
using System.Text.Json;

namespace Livrable1.ViewModel
{
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
        public static bool IsMkvEnabled { get; set; } = false;
        public static bool IsJpgEnabled { get; set; } = false;

        // Additional state flags for JSON and XML (e.g., for logging or output options)
        public static bool IsJsonOn { get; set; } = true;
        public static bool IsXmlOn { get; set; } = false;

        // Method to update encryption settings for a specific extension
        public static void UpdateExtensionEncryption(string extension, bool isEnabled)
        {
            if (isEnabled)
            {
                MessageBox.Show($"{LanguageManager.GetText("extension_name")} '{extension}' {LanguageManager.GetText("add_for_encryption")}"); // Show message when the extension is enabled for encryption
            }
            else
            {
                MessageBox.Show($"{LanguageManager.GetText("extension_name")} '{extension}' {LanguageManager.GetText("supp_for_encryption")}"); // Show message when the extension is disabled for encryption
            }
        }

        public static void UpdatePriorityExtension(string extension, bool isPriority)
        {
            PriorityExtensionManager.Instance.UpdatePriorityExtension(extension, isPriority);
        }

        public static bool IsPriorityExtension(string extension)
        {
            return PriorityExtensionManager.Instance.PriorityExtensions.Contains(extension.ToLower());
        }
    }
}