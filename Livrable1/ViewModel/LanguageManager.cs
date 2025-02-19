using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;

//---------------------ViewModel---------------------//
namespace Livrable1.ViewModel
{
    //------------Class LangageManager------------//
    public static class LanguageManager
    {
        private static Dictionary<string, Dictionary<string, string>> _translations; // Dictionary to store translations for different languages.
        private static string _currentLanguage; // The currently selected language code.
        public static event EventHandler LanguageChanged; // Event raised when the language is changed.

        // Static constructor to initialize translations and set the default language.
        static LanguageManager()
        {
            _translations = new Dictionary<string, Dictionary<string, string>>();
            _currentLanguage = "en"; // Default language set to English.
            LoadTranslations(); // Load translations from the JSON file.
        }

        // Method to load translations from a JSON file.
        private static void LoadTranslations()
        {
            try
            {
                // Build the path to the Language.json file.
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Language.json");

                // Check if the file exists.
                if (File.Exists(jsonPath))
                {
                    string jsonContent = File.ReadAllText(jsonPath); // Read the entire JSON content.
                    var loadedTranslations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(jsonContent); // Deserialize the JSON content into the translations dictionary.

                    // If deserialization was successful, update the translations dictionary.
                    if (loadedTranslations != null)
                    {
                        _translations = loadedTranslations;
                    }
                }
            }
            catch (Exception ex)
            {
                // Display an error message if there was a problem loading translations.
                MessageBox.Show($"Error loading translations: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to set the current language.
        public static void SetLanguage(string languageCode)
        {
            // Check if the provided language code is available in the translations.
            if (_translations.ContainsKey(languageCode))
            {
                _currentLanguage = languageCode; // Update the current language.
                OnLanguageChanged(); // Notify subscribers that the language has changed.
            }
        }

        // Method to get the translated text for a given key.
        public static string GetText(string key)
        {
            try
            {
                // If the current language has a translation for the key, return it.
                if (_translations.ContainsKey(_currentLanguage) &&
                    _translations[_currentLanguage].ContainsKey(key))
                {
                    return _translations[_currentLanguage][key];
                }

                // Fallback to English if key not found in current language
                if (_translations["en"].ContainsKey(key))
                {
                    return _translations["en"][key];
                }

                return $"[{key}]"; // If the key is not found at all, return the key wrapped in brackets.
            }
            catch
            {
                return $"[{key}]"; // In case of an exception, return the key wrapped in brackets.
            }
        }

        // Method to raise the LanguageChanged event.
        private static void OnLanguageChanged()
        {
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }

        // Public property to get the current language.
        public static string CurrentLanguage => _currentLanguage;

        // Method to check if a given language is available.
        public static bool IsLanguageAvailable(string languageCode)
        {
            return _translations.ContainsKey(languageCode);
        }
    }
    //------------Class LangageManager------------//
}
//---------------------ViewModel---------------------//
