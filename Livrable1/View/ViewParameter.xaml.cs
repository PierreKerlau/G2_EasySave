﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Livrable1.ViewModel;
using Livrable1.Model;
using System.Diagnostics;
using System.ComponentModel;

//---------------------View---------------------//
namespace Livrable1.View
{
    /// <summary>
    /// Logique d'interaction pour ViewParameter.xaml
    /// </summary>
    public partial class ViewParameter : Window
    {
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBoxCalculator.IsChecked = ProcessWatcher.Instance.BloquerCheatEngine;
            CheckBoxNotepad.IsChecked = ProcessWatcher.Instance.BloquerWireshark;
        }
        
        public ViewParameter()
        {
            InitializeComponent();
            UpdateUILanguageParameter(); // Update language

            // Lire la clé depuis le fichier .env
            try
            {
                string projectRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\.."));
                string envPath = System.IO.Path.Combine(projectRoot, ".env");
                
                string[] lines = System.IO.File.ReadAllLines(envPath);
                foreach (string line in lines)
                {
                    if (line.StartsWith("EASYSAVE_CRYPTO_KEY="))
                    {
                        string key = line.Substring("EASYSAVE_CRYPTO_KEY=".Length);
                        Environment.SetEnvironmentVariable("EASYSAVE_CRYPTO_KEY", key);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageManager.GetText("error_reading_env")} {ex.Message}");
            }

            // Initialiser l'état des CheckBox
            InitializeCheckBoxStates();

            // Définir l'état initial des boutons radio selon la langue actuelle
            if (LanguageManager.CurrentLanguage == "fr")
            {
                RadioButtonFrench.IsChecked = true;
            }
            else
            {
                RadioButtonEnglish.IsChecked = true;
            }

            // S'abonner à l'événement de changement de langue
            LanguageManager.LanguageChanged += OnLanguageChanged;

            // Mettre à jour l'interface avec la langue actuelle
            UpdateUILanguage();

        }

        private void InitializeCheckBoxStates()
        {
            var checkBoxCalculator = this.FindName("CheckBoxCalculator") as CheckBox;
            var checkBoxNotepad = this.FindName("CheckBoxNotepad") as CheckBox;
            var checkBoxPDF = this.FindName("CheckBoxPDF") as CheckBox;
            var checkBoxPNG = this.FindName("CheckBoxPNG") as CheckBox;
            var checkBoxJSON = this.FindName("CheckBoxJSON") as CheckBox;
            var checkBoxXML = this.FindName("CheckBoxXML") as CheckBox;
            var checkBoxDOCX = this.FindName("CheckBoxDOCX") as CheckBox;
            var checkBoxTXT = this.FindName("CheckBoxTXT") as CheckBox;

            if (checkBoxCalculator != null) checkBoxCalculator.IsChecked = StateViewModel.IsCalculatorEnabled;
            if (checkBoxNotepad != null) checkBoxNotepad.IsChecked = StateViewModel.IsNotePadEnabled;
            if (checkBoxPDF != null) checkBoxPDF.IsChecked = StateViewModel.IsPdfEnabled;
            if (checkBoxPNG != null) checkBoxPNG.IsChecked = StateViewModel.IsPngEnabled;
            if (checkBoxJSON != null) checkBoxJSON.IsChecked = StateViewModel.IsJsonEnabled;
            if (checkBoxXML != null) checkBoxXML.IsChecked = StateViewModel.IsXmlEnabled;
            if (checkBoxDOCX != null) checkBoxDOCX.IsChecked = StateViewModel.IsDocxEnabled;
            if (checkBoxTXT != null) checkBoxTXT.IsChecked = StateViewModel.IsTxtEnabled;
            // Initialiser l'état des RadioButtons pour le format de log
            var radioButtonJSON = this.FindName("RadioButtonLogJSON") as RadioButton;
            var radioButtonXML = this.FindName("RadioButtonLogXML") as RadioButton;

            if (radioButtonJSON != null) radioButtonJSON.IsChecked = StateViewModel.IsJsonOn;
            if (radioButtonXML != null) radioButtonXML.IsChecked = StateViewModel.IsXmlOn;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            UpdateUILanguage();
        }

        private void UpdateUILanguage()
        {
            TextBlockLanguage.Text = LanguageManager.GetText("menu_language");
            RadioButtonFrench.Content = LanguageManager.GetText("french");
            RadioButtonEnglish.Content = LanguageManager.GetText("english");

            TextBlockSoftware.Text = LanguageManager.GetText("menu_software");
            CheckBoxCalculator.Content = LanguageManager.GetText("calculator");
            CheckBoxNotepad.Content = LanguageManager.GetText("notepad");

            TextBlockExtensions.Text = LanguageManager.GetText("encrypt_extensions");

            TextBlockLogFormat.Text = LanguageManager.GetText("log_format");
            RadioButtonLogJSON.Content = LanguageManager.GetText("format_json");
            RadioButtonLogXML.Content = LanguageManager.GetText("format_xml");

            ButtonLeave.Content = LanguageManager.GetText("menu_leave");
        }

        private void RadioButton_Language_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return; // Éviter l'exécution pendant l'initialisation

            if (sender is RadioButton radioButton)
            {
                string newLanguage = radioButton.Name == "RadioButtonFrench" ? "fr" : "en";
                LanguageManager.SetLanguage(newLanguage);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            LanguageManager.LanguageChanged -= OnLanguageChanged;
        }

        private void ButtonLeave_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void RadioButton_LogFormat_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                switch (radioButton.Name)
                {
                    case "RadioButtonLogJSON":
                        if (radioButton.IsChecked == true)
                        {
                            StateViewModel.IsJsonOn = true;
                            StateViewModel.IsXmlOn = false;
                        }
                        break;
                    case "RadioButtonLogXML":
                        if (radioButton.IsChecked == true)
                        {
                            StateViewModel.IsXmlOn = true;
                            StateViewModel.IsJsonOn = false;
                        }
                        break;
                }
            }
        }

        private void CheckBox_Checked_Calculator(object sender, RoutedEventArgs e)
        {
            ProcessWatcher.Instance.BloquerCheatEngine = CheckBoxCalculator.IsChecked == true;
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsCalculatorEnabled = checkBox.IsChecked ?? false;
            }
        }

        private void CheckBox_Unchecked_Calculator(object sender, RoutedEventArgs e)
        {
            ProcessWatcher.Instance.BloquerCheatEngine = CheckBoxCalculator.IsChecked == false;
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsCalculatorEnabled = false;
            }
        }

        private void CheckBox_Checked_Notepad(object sender, RoutedEventArgs e)
        {
            ProcessWatcher.Instance.BloquerWireshark = CheckBoxNotepad.IsChecked == true;
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsNotePadEnabled = checkBox.IsChecked ?? false;
            }
        }

        private void CheckBox_Unchecked_Notepad(object sender, RoutedEventArgs e)
        {
            ProcessWatcher.Instance.BloquerWireshark = CheckBoxNotepad.IsChecked == false;
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsNotePadEnabled = false;
            }
        }

        private void CheckBox_Checked_PDF(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsPdfEnabled = checkBox.IsChecked ?? false;
                StateViewModel.UpdateExtensionEncryption(".pdf", checkBox.IsChecked ?? false);
            }
        }

        private void CheckBox_Unchecked_PDF(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsPdfEnabled = false;
                StateViewModel.UpdateExtensionEncryption(".pdf", false);
            }
        }

        private void CheckBox_Checked_PNG(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsPngEnabled = checkBox.IsChecked ?? false;
                StateViewModel.UpdateExtensionEncryption(".png", checkBox.IsChecked ?? false);
            }
        }

        private void CheckBox_Unchecked_PNG(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsPngEnabled = false;
                StateViewModel.UpdateExtensionEncryption(".png", false);
            }
        }

        private void CheckBox_Checked_JSON(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsJsonEnabled = checkBox.IsChecked ?? false;
                StateViewModel.UpdateExtensionEncryption(".json", checkBox.IsChecked ?? false);
            }
        }

        private void CheckBox_Unchecked_JSON(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsJsonEnabled = false;
                StateViewModel.UpdateExtensionEncryption(".json", false);
            }
        }

        private void CheckBox_Checked_XML(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsXmlEnabled = checkBox.IsChecked ?? false;
                StateViewModel.UpdateExtensionEncryption(".xml", checkBox.IsChecked ?? false);
            }
        }

        private void CheckBox_Unchecked_XML(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsXmlEnabled = false;
                StateViewModel.UpdateExtensionEncryption(".xml", false);
            }
        }

        private void CheckBox_Checked_DOCX(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsDocxEnabled = checkBox.IsChecked ?? false;
                StateViewModel.UpdateExtensionEncryption(".docx", checkBox.IsChecked ?? false);
            }
        }

        private void CheckBox_Unchecked_DOCX(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsDocxEnabled = false;
                StateViewModel.UpdateExtensionEncryption(".docx", false);
            }
        }

        private void CheckBox_Checked_TXT(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsTxtEnabled = checkBox.IsChecked ?? false;
                StateViewModel.UpdateExtensionEncryption(".txt", checkBox.IsChecked ?? false);
            }
        }

        private void CheckBox_Unchecked_TXT(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsTxtEnabled = false;
                StateViewModel.UpdateExtensionEncryption(".txt", false);
            }
        }

        // Method to update UI elements with language-specific texts
        private void UpdateUILanguageParameter()
        {
            TextBlockLanguage.Text = LanguageManager.GetText("text_block_choose_language");
            TextBlockSoftware.Text = LanguageManager.GetText("text_block_choose_software");
            CheckBoxCalculator.Content = LanguageManager.GetText("checkbox_calculator");
            TextBlockExtensions.Text = LanguageManager.GetText("text_block_choose_extension");
            TextBlockLogFormat.Text = LanguageManager.GetText("text_block_choose_format_log");
            ButtonLeave.Content = LanguageManager.GetText("menu_leave");
        }
    }
}
//---------------------View---------------------//