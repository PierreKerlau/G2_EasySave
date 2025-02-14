using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Livrable1.Controller
{
    public static class LanguageManager
    {
        private static Dictionary<string, Dictionary<string, string>> _translations;
        private static string _currentLanguage;
        public static event EventHandler LanguageChanged;

        static LanguageManager()
        {
            _translations = new Dictionary<string, Dictionary<string, string>>();
            _currentLanguage = "en";
            LoadTranslations();
        }

        private static void LoadTranslations()
        {
            try
            {
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Language.json");
                if (File.Exists(jsonPath))
                {
                    string jsonContent = File.ReadAllText(jsonPath);
                    var loadedTranslations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(jsonContent);
                    if (loadedTranslations != null)
                    {
                        _translations = loadedTranslations;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading translations: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void SetLanguage(string languageCode)
        {
            if (_translations.ContainsKey(languageCode))
            {
                _currentLanguage = languageCode;
                OnLanguageChanged();
            }
        }

        public static string GetText(string key)
        {
            try
            {
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
                
                return $"[{key}]";
            }
            catch
            {
                return $"[{key}]";
            }
        }

        private static void OnLanguageChanged()
        {
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }

        public static string CurrentLanguage => _currentLanguage;

        public static bool IsLanguageAvailable(string languageCode)
        {
            return _translations.ContainsKey(languageCode);
        }
    }
}
