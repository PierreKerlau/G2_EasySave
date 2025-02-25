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
            CheckBoxCalculator.IsChecked = ProcessWatcher.Instance.BloquerCalculator;
            CheckBoxNotepad.IsChecked = ProcessWatcher.Instance.BloquerNotepad;
        }
        
        public ViewParameter()
        {
            InitializeComponent();
            UpdateUILanguageParameter(); // Update language

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

            InitializeCheckBoxStates();

            if (LanguageManager.CurrentLanguage == "fr")
            {
                RadioButtonFrench.IsChecked = true;
            }
            else
            {
                RadioButtonEnglish.IsChecked = true;
            }

            LanguageManager.LanguageChanged += OnLanguageChanged;
            UpdateUILanguage();
            InitializePriorityExtensionsState();
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
            var checkBoxMKV = this.FindName("CheckBoxMKV") as CheckBox;
            var checkBoxJPG = this.FindName("CheckBoxJPG") as CheckBox;

            var checkBoxPrioPDF = this.FindName("CheckBoxPrioPDF") as CheckBox;
            var checkBoxPrioPNG = this.FindName("CheckBoxPrioPNG") as CheckBox;
            var checkBoxPrioTXT = this.FindName("CheckBoxPrioTXT") as CheckBox;
            var checkBoxPrioJSON = this.FindName("CheckBoxPrioJSON") as CheckBox;
            var checkBoxPrioXML = this.FindName("CheckBoxPrioXML") as CheckBox;
            var checkBoxPrioDOCX = this.FindName("CheckBoxPrioDOCX") as CheckBox;
            var checkBoxPrioMKV = this.FindName("CheckBoxPrioMKV") as CheckBox;
            var checkBoxPrioJPG = this.FindName("CheckBoxPrioJPG") as CheckBox;

            if (checkBoxCalculator != null) checkBoxCalculator.IsChecked = StateViewModel.IsCalculatorEnabled;
            if (checkBoxNotepad != null) checkBoxNotepad.IsChecked = StateViewModel.IsNotePadEnabled;
            if (checkBoxPDF != null) checkBoxPDF.IsChecked = StateViewModel.IsPdfEnabled;
            if (checkBoxPNG != null) checkBoxPNG.IsChecked = StateViewModel.IsPngEnabled;
            if (checkBoxJSON != null) checkBoxJSON.IsChecked = StateViewModel.IsJsonEnabled;
            if (checkBoxXML != null) checkBoxXML.IsChecked = StateViewModel.IsXmlEnabled;
            if (checkBoxDOCX != null) checkBoxDOCX.IsChecked = StateViewModel.IsDocxEnabled;
            if (checkBoxTXT != null) checkBoxTXT.IsChecked = StateViewModel.IsTxtEnabled;
            if (checkBoxMKV != null) checkBoxMKV.IsChecked = StateViewModel.IsMkvEnabled;
            if (checkBoxJPG != null) checkBoxJPG.IsChecked = StateViewModel.IsJpgEnabled;

            if (checkBoxPrioPDF != null) checkBoxPrioPDF.IsChecked = StateViewModel.IsPdfEnabled;
            if (checkBoxPrioPNG != null) checkBoxPrioPNG.IsChecked = StateViewModel.IsPngEnabled;
            if (checkBoxPrioTXT != null) checkBoxPrioTXT.IsChecked = StateViewModel.IsTxtEnabled;
            if (checkBoxPrioJSON != null) checkBoxPrioJSON.IsChecked = StateViewModel.IsJsonEnabled;
            if (checkBoxPrioXML != null) checkBoxPrioXML.IsChecked = StateViewModel.IsXmlEnabled;
            if (checkBoxPrioDOCX != null) checkBoxPrioDOCX.IsChecked = StateViewModel.IsDocxEnabled;
            if (checkBoxPrioMKV != null) checkBoxPrioMKV.IsChecked = StateViewModel.IsMkvEnabled;
            if (checkBoxPrioJPG != null) checkBoxPrioJPG.IsChecked = StateViewModel.IsJpgEnabled;

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

            TextBlockPriorityExtension.Text = LanguageManager.GetText("Priority_Extension");

            ButtonLeave.Content = LanguageManager.GetText("menu_leave");
        }

        private void RadioButton_Language_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

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
            ProcessWatcher.Instance.BloquerCalculator = CheckBoxCalculator.IsChecked == true;
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsCalculatorEnabled = checkBox.IsChecked ?? false;
            }
        }

        private void CheckBox_Unchecked_Calculator(object sender, RoutedEventArgs e)
        {
            ProcessWatcher.Instance.BloquerCalculator = CheckBoxCalculator.IsChecked == false;
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsCalculatorEnabled = false;
            }
        }

        private void CheckBox_Checked_Notepad(object sender, RoutedEventArgs e)
        {
            ProcessWatcher.Instance.BloquerNotepad = CheckBoxNotepad.IsChecked == true;
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsNotePadEnabled = checkBox.IsChecked ?? false;
            }
        }

        private void CheckBox_Unchecked_Notepad(object sender, RoutedEventArgs e)
        {
            ProcessWatcher.Instance.BloquerNotepad = CheckBoxNotepad.IsChecked == false;
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

        private void CheckBox_Checked_MKV(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsMkvEnabled = checkBox.IsChecked ?? false;
                StateViewModel.UpdateExtensionEncryption(".mkv", checkBox.IsChecked ?? false);
            }
        }

        private void CheckBox_Unchecked_MKV(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsMkvEnabled = false;
                StateViewModel.UpdateExtensionEncryption(".mkv", false);
            }
        }

        private void CheckBox_Checked_JPG(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsJpgEnabled = checkBox.IsChecked ?? false;
                StateViewModel.UpdateExtensionEncryption(".jpg", checkBox.IsChecked ?? false);
            }
        }

        private void CheckBox_Unchecked_JPG(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateViewModel.IsJpgEnabled = false;
                StateViewModel.UpdateExtensionEncryption(".jpg", false);
            }
        }

        private void CheckBox_Checked_Prio_PDF(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".pdf", true);
        }

        private void CheckBox_Unchecked_Prio_PDF(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".pdf", false);
        }

        private void CheckBox_Checked_Prio_PNG(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".png", true);
        }

        private void CheckBox_Unchecked_Prio_PNG(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".png", false);
        }

        private void CheckBox_Checked_Prio_TXT(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".txt", true);
        }

        private void CheckBox_Unchecked_Prio_TXT(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".txt", false);
        }

        private void CheckBox_Checked_Prio_JSON(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".json", true);
        }

        private void CheckBox_Unchecked_Prio_JSON(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".json", false);
        }

        private void CheckBox_Checked_Prio_XML(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".xml", true);
        }

        private void CheckBox_Unchecked_Prio_XML(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".xml", false);
        }

        private void CheckBox_Checked_Prio_DOCX(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".docx", true);
        }

        private void CheckBox_Unchecked_Prio_DOCX(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".docx", false);
        }

        private void CheckBox_Checked_Prio_MKV(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".mkv", true);
        }

        private void CheckBox_Unchecked_Prio_MKV(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".mkv", false);
        }

        private void CheckBox_Checked_Prio_JPG(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".jpg", true);
        }

        private void CheckBox_Unchecked_Prio_JPG(object sender, RoutedEventArgs e)
        {
            StateViewModel.UpdatePriorityExtension(".jpg", false);
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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Checked_2(object sender, RoutedEventArgs e)
        {

        }

        private void InitializePriorityExtensionsState()
        {
            CheckBoxPrioPDF.IsChecked = StateViewModel.IsPriorityExtension(".pdf");
            CheckBoxPrioPNG.IsChecked = StateViewModel.IsPriorityExtension(".png");
            CheckBoxPrioTXT.IsChecked = StateViewModel.IsPriorityExtension(".txt");
            CheckBoxPrioJSON.IsChecked = StateViewModel.IsPriorityExtension(".json");
            CheckBoxPrioXML.IsChecked = StateViewModel.IsPriorityExtension(".xml");
            CheckBoxPrioDOCX.IsChecked = StateViewModel.IsPriorityExtension(".docx");
            CheckBoxPrioMKV.IsChecked = StateViewModel.IsPriorityExtension(".mkv");
            CheckBoxPrioJPG.IsChecked = StateViewModel.IsPriorityExtension(".jpg");
        }
    }
}
//---------------------View---------------------//