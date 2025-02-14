using System.Collections.Generic;

namespace EasySave.Cryptography
{
    public static class ExtensionManager
    {
        private static readonly HashSet<string> _encryptedExtensions = new HashSet<string>();

        public static void AddExtension(string extension)
        {
            if (!extension.StartsWith("."))
                extension = "." + extension;
            _encryptedExtensions.Add(extension.ToLower());
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
            return _encryptedExtensions.Contains(extension.ToLower());
        }

        public static IEnumerable<string> GetEncryptedExtensions()
        {
            return _encryptedExtensions;
        }
    }
} 