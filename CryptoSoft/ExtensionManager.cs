using System.Collections.Generic;
using System.Windows;

namespace EasySave.Cryptography
{
    public static class ExtensionManager
    {
        private static HashSet<string> _encryptedExtensions = new HashSet<string>();

        public static void AddExtension(string extension)
        {
            if (!extension.StartsWith("."))
                extension = "." + extension;
            extension = extension.ToLower();
            _encryptedExtensions.Add(extension);
            MessageBox.Show($"Extensions actives après ajout : {string.Join(", ", _encryptedExtensions)}");
        }

        public static void RemoveExtension(string extension)
        {
            if (!extension.StartsWith("."))
                extension = "." + extension;
            _encryptedExtensions.Remove(extension.ToLower());
        }

        public static bool IsEncryptionEnabled(string extension)
        {
            if (!extension.StartsWith("."))
                extension = "." + extension;
            extension = extension.ToLower();
            bool result = _encryptedExtensions.Contains(extension);
            MessageBox.Show($"Vérification cryptage pour {extension}: {result}");
            return result;
        }

        public static IEnumerable<string> GetEncryptedExtensions()
        {
            return _encryptedExtensions;
        }
    }
} 