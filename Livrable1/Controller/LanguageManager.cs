
//using Livrable1.View;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text.Json;

//namespace Livrable1.Controller
//{
//    public class LanguageManager
//    {
//        private static Dictionary<string, Dictionary<string, string>> languages = new();
//        private static string currentLanguage = "en"; // Langue par défaut

//        static LanguageManager()
//        {
//            LoadLanguages();
//        }

//        public void ChoiceLanguage()
//        {
//            Console.Clear();
//            ViewConsole.ShowLogo();
//            Console.WriteLine(LanguageManager.GetText("choose_language"));

//            ConsoleKeyInfo choice = Console.ReadKey();

//            if (choice.KeyChar == '1')
//            {
//                LanguageManager.SetLanguage("en");
//                Console.WriteLine("\n" + LanguageManager.GetText("language_changed") + " English.");
//            }
//            else if (choice.KeyChar == '2')
//            {
//                LanguageManager.SetLanguage("fr");
//                Console.WriteLine("\n" + LanguageManager.GetText("language_changed") + " Français.");
//            }
//            else
//            {
//                Console.WriteLine($"\n{LanguageManager.GetText("invalid_choice")}");
//            }

//        }

//        public static void LoadLanguages()
//        {
//            string filePath = "../../../Language.json"; 

//            if (File.Exists(filePath))
//            {
//                try
//                {
//                    string jsonContent = File.ReadAllText(filePath);
//                    languages = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(jsonContent);

//                    if (languages == null)
//                    {
//                        Console.WriteLine(LanguageManager.GetText("error_charging_data"));
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"{LanguageManager.GetText("error_charging_files")} '{ex.Message}'");
//                }
//            }
//            else
//            {
//                Console.WriteLine(LanguageManager.GetText("error_file_not_exist"));
//            }
//        }

//        public static string GetText(string key)
//        {
//            // Vérifie si la clé existe pour la langue actuelle
//            if (languages.ContainsKey(currentLanguage) && languages[currentLanguage].ContainsKey(key))
//            {
//                return languages[currentLanguage][key];
//            }
//            else
//            {
//                Console.WriteLine($"{LanguageManager.GetText("warning_key")} '{key}'. {LanguageManager.GetText("not_found_for_language")}'{currentLanguage}'");
//                return $"======== {key} ========"; // Retourne la clé brute si non trouvé
//            }
//        }

//        public static void SetLanguage(string language)
//        {
//            if (languages.ContainsKey(language))
//            {
//                currentLanguage = language;
//            }
//            else
//            {
//                Console.WriteLine($"{LanguageManager.GetText("language")} '{language}'. {LanguageManager.GetText("language_default")}");
//                currentLanguage = "en"; // Langue par défaut
//            }
//        }
//    }
//}
