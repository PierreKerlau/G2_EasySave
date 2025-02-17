using System.Windows;
using EasySave.Cryptography;

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

        // États des formats de log
        public static bool IsJsonOn { get; set; } = false;
        public static bool IsXmlOn { get; set; } = false;

        public static void UpdateExtensionEncryption(string extension, bool isEnabled)
        {
            if (isEnabled)
            {
                ExtensionManager.AddExtension(extension);
                MessageBox.Show($"Extension {extension} ajoutée pour le cryptage");
            }
            else
            {
                ExtensionManager.RemoveExtension(extension);
                MessageBox.Show($"Extension {extension} retirée du cryptage");
            }
        }
    }
} 