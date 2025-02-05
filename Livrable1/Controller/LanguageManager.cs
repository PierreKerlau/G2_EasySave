using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Livrable1.Controller
{
    public static class LanguageManager
    {
        private static Dictionary<string, Dictionary<string, string>> translations = new();
        private static string currentLanguage = "en"; // Default language

        public static void LoadLanguages()
        {
            string filePath = "../../../Language.json";

            Console.WriteLine(Path.GetFullPath(filePath));

            if (!File.Exists(filePath))
            {
                Console.WriteLine("ERROR: Language file not found!");
                return;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                translations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json) ?? new();

                if (translations.Count == 0)
                {
                    Console.WriteLine("ERROR: Language file is empty or incorrectly formatted!");
                }
                else
                {
                    Console.WriteLine("Languages loaded successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to load language file: {ex.Message}");
            }
        }

        public static string GetText(string key)
        {
            if (translations.ContainsKey(currentLanguage) && translations[currentLanguage].ContainsKey(key))
            {
                return translations[currentLanguage][key];
            }

            Console.WriteLine($"WARNING: Key '{key}' not found for language '{currentLanguage}'");
            return key;
        }

        public static void SetLanguage(string lang)
        {
            if (translations.ContainsKey(lang))
            {
                currentLanguage = lang;
                Console.WriteLine($"Language changed to {lang}");
            }
            else
            {
                Console.WriteLine("ERROR: Language not available!");
            }
        }

        public static string GetCurrentLanguage()
        {
            return currentLanguage;
        }
    }
}
