
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Livrable1.Controller
{
    public class LanguageManager
    {
        private static Dictionary<string, Dictionary<string, string>> languages = new();
        private static string currentLanguage = "en"; // Langue par défaut

        static LanguageManager()
        {
            LoadLanguages();
        }

        public void ChoiceLanguage()
        {
            Console.Clear();
            Console.WriteLine(LanguageManager.GetText("choose_language"));
            Console.WriteLine("1. " + LanguageManager.GetText("english"));
            Console.WriteLine("2. " + LanguageManager.GetText("french"));

            ConsoleKeyInfo choice = Console.ReadKey();

            if (choice.KeyChar == '1')
            {
                LanguageManager.SetLanguage("en");
                Console.WriteLine("\n" + LanguageManager.GetText("language_changed") + " English.");
            }
            else if (choice.KeyChar == '2')
            {
                LanguageManager.SetLanguage("fr");
                Console.WriteLine("\n" + LanguageManager.GetText("language_changed") + " Français.");
            }
            else
            {
                Console.WriteLine($"\n{LanguageManager.GetText("invalid_choice")}");
            }

            Console.WriteLine("\n" + LanguageManager.GetText("press_any_key"));
            Console.ReadKey(); // Attend que l'utilisateur appuie sur une touche pour revenir au menu.
        }

        public static void LoadLanguages()
        {
            string filePath = "../../../Language.json"; // Assure-toi que le fichier JSON est à cet emplacement

            if (File.Exists(filePath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(filePath);
                    languages = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(jsonContent);

                    if (languages == null)
                    {
                        Console.WriteLine("Erreur: Impossible de charger les données des langues.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du chargement du fichier de langue: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Erreur: Le fichier de langue n'existe pas.");
            }
        }

        public static string GetText(string key)
        {
            // Vérifie si la clé existe pour la langue actuelle
            if (languages.ContainsKey(currentLanguage) && languages[currentLanguage].ContainsKey(key))
            {
                return languages[currentLanguage][key];
            }
            else
            {
                Console.WriteLine($"WARNING: Key '{key}' not found for language '{currentLanguage}'");
                return $"======== {key} ========"; // Retourne la clé brute si non trouvé
            }
        }

        public static void SetLanguage(string language)
        {
            if (languages.ContainsKey(language))
            {
                currentLanguage = language;
            }
            else
            {
                Console.WriteLine($"Langue '{language}' non trouvée, utilisant la langue par défaut.");
                currentLanguage = "en"; // Langue par défaut
            }
        }
    }
}
