using System;
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
using System.Diagnostics;
// using EasySave.Cryptography;

namespace Livrable1.View
{
    /// <summary>
    /// Logique d'interaction pour ViewParameter.xaml
    /// </summary>
    public partial class ViewParameter : Window
    {
        public ViewParameter()
        {
            InitializeComponent();

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

            if (checkBoxCalculator != null) checkBoxCalculator.IsChecked = StateManager.IsCalculatorEnabled;
            if (checkBoxNotepad != null) checkBoxNotepad.IsChecked = StateManager.IsNotePadEnabled;
            if (checkBoxPDF != null) checkBoxPDF.IsChecked = StateManager.IsPdfEnabled;
            if (checkBoxPNG != null) checkBoxPNG.IsChecked = StateManager.IsPngEnabled;
            if (checkBoxJSON != null) checkBoxJSON.IsChecked = StateManager.IsJsonEnabled;
            if (checkBoxXML != null) checkBoxXML.IsChecked = StateManager.IsXmlEnabled;
            if (checkBoxDOCX != null) checkBoxDOCX.IsChecked = StateManager.IsDocxEnabled;
            // Initialiser l'état des RadioButtons pour le format de log
            var radioButtonJSON = this.FindName("RadioButtonLogJSON") as RadioButton;
            var radioButtonXML = this.FindName("RadioButtonLogXML") as RadioButton;

            if (radioButtonJSON != null) radioButtonJSON.IsChecked = StateManager.IsJsonOn;
            if (radioButtonXML != null) radioButtonXML.IsChecked = StateManager.IsXmlOn;
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
                            StateManager.IsJsonOn = true;
                            StateManager.IsXmlOn = false;
                        }
                        break;
                    case "RadioButtonLogXML":
                        if (radioButton.IsChecked == true)
                        {
                            StateManager.IsXmlOn = true;
                            StateManager.IsJsonOn = false;
                        }
                        break;
                }
            }
        }

        private void CheckBox_Checked_Calculator(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateManager.IsCalculatorEnabled = checkBox.IsChecked ?? false;
            }
        }

        private void CheckBox_Checked_Notepad(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateManager.IsNotePadEnabled = checkBox.IsChecked ?? false;
            }
        }

        private void CheckBox_Checked_PDF(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateManager.IsPdfEnabled = checkBox.IsChecked ?? false;
            }
        }

        private void CheckBox_Checked_PNG(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateManager.IsPngEnabled = checkBox.IsChecked ?? false;
            }
        }

        private void CheckBox_Checked_JSON(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateManager.IsJsonEnabled = checkBox.IsChecked ?? false;
            }
        }

        private void CheckBox_Checked_XML(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateManager.IsXmlEnabled = checkBox.IsChecked ?? false;
            }
        }

        private void CheckBox_Checked_DOCX(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                StateManager.IsDocxEnabled = checkBox.IsChecked ?? false;
            }
        }
    }
}
