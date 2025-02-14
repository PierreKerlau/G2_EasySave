using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EasySave.Cryptography
{
    public class CryptoManager
    {
        private static readonly string _key = Environment.GetEnvironmentVariable("EASYSAVE_CRYPTO_KEY") 
            ?? throw new InvalidOperationException("Encryption key not found in environment variables");
        public static void EncryptFile(string sourceFile, string destinationFile)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] key = Encoding.UTF8.GetBytes(_key);
                    Array.Resize(ref key, 32); // AES-256
                    aes.Key = key;
                    aes.GenerateIV();

                    using (FileStream destinationStream = File.Create(destinationFile))
                    {
                        // Écrire l'IV au début du fichier
                        destinationStream.Write(aes.IV, 0, aes.IV.Length);

                        using (CryptoStream cryptoStream = new CryptoStream(
                            destinationStream,
                            aes.CreateEncryptor(),
                            CryptoStreamMode.Write))
                        {
                            using (FileStream sourceStream = File.OpenRead(sourceFile))
                            {
                                sourceStream.CopyTo(cryptoStream);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Encryption failed: {ex.Message}", ex);
            }
        }

        public static void DecryptFile(string sourceFile, string destinationFile)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    byte[] key = Encoding.UTF8.GetBytes(_key);
                    Array.Resize(ref key, 32); // AES-256
                    aes.Key = key;

                    using (FileStream sourceStream = File.OpenRead(sourceFile))
                    {
                        byte[] iv = new byte[aes.IV.Length];
                        sourceStream.Read(iv, 0, iv.Length);
                        aes.IV = iv;

                        using (CryptoStream cryptoStream = new CryptoStream(
                            sourceStream,
                            aes.CreateDecryptor(),
                            CryptoStreamMode.Read))
                        {
                            using (FileStream destinationStream = File.Create(destinationFile))
                            {
                                cryptoStream.CopyTo(destinationStream);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Decryption failed: {ex.Message}", ex);
            }
        }

        public static bool ShouldEncrypt(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return ExtensionManager.IsEncryptionEnabled(extension);
        }
    }
}
