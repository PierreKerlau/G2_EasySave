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
using Livrable1.Controller;
using System.Diagnostics;
using EasySave.Cryptography;

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
                        // Code pour format JSON
                        break;
                    case "RadioButtonLogXML":
                        // Code pour format XML
                        break;
                }
            }
        }

        private void CheckBox_Checked_Calculator(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.IsChecked == true)
            {
                try
                {
                    Process.Start("calc.exe");
                    MessageBox.Show(LanguageManager.GetText("app_closing"), "EasySave", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error starting Calculator: {ex.Message}");
                }
            }
        }

        private void CheckBox_Checked_Notepad(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.IsChecked == true)
            {
                try
                {
                    Process.Start("notepad.exe");
                    MessageBox.Show(LanguageManager.GetText("app_closing"), "EasySave", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error starting Notepad: {ex.Message}");
                }
            }
        }

        private void CheckBox_Checked_PDF(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                try
                {
                    if (checkBox.IsChecked == true)
                    {
                        ExtensionManager.AddExtension(".pdf");
                        MessageBox.Show("PDF encryption enabled");
                    }
                    else
                    {
                        ExtensionManager.RemoveExtension(".pdf");
                        MessageBox.Show("PDF encryption disabled");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void CheckBox_Checked_PNG(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                try
                {
                    if (checkBox.IsChecked == true)
                    {
                        ExtensionManager.AddExtension(".png");
                        MessageBox.Show("PNG encryption enabled");
                    }
                    else
                    {
                        ExtensionManager.RemoveExtension(".png");
                        MessageBox.Show("PNG encryption disabled");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void CheckBox_Checked_JSON(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                try
                {
                    if (checkBox.IsChecked == true)
                    {
                        ExtensionManager.AddExtension(".json");
                        MessageBox.Show("JSON encryption enabled");
                    }
                    else
                    {
                        ExtensionManager.RemoveExtension(".json");
                        MessageBox.Show("JSON encryption disabled");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void CheckBox_Checked_XML(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                try
                {
                    if (checkBox.IsChecked == true)
                    {
                        ExtensionManager.AddExtension(".xml");
                        MessageBox.Show("XML encryption enabled");
                    }
                    else
                    {
                        ExtensionManager.RemoveExtension(".xml");
                        MessageBox.Show("XML encryption disabled");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void CheckBox_Checked_DOCX(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                try
                {
                    if (checkBox.IsChecked == true)
                    {
                        ExtensionManager.AddExtension(".docx");
                        MessageBox.Show("DOCX encryption enabled");
                    }
                    else
                    {
                        ExtensionManager.RemoveExtension(".docx");
                        MessageBox.Show("DOCX encryption disabled");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
    }
}
