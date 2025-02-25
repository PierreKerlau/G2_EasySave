using System;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Livrable1.Model;
using Livrable1.View;
using Livrable1.ViewModel;
using System.Text.Json;

//---------------------View---------------------//
namespace Livrable1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    //------------Class MainWindow------------//
    public partial class MainWindow : Window
    {
        // Constructor for MainWindow
        public MainWindow()
        {
            InitializeComponent(); // Initialize UI components
            UpdateUILanguageMainWindows(); // Update UI elements with language-specific texts
        }

        // Method to check if a process with the given name is running
        private bool IsProcessRunning(string processName)
        {
            return System.Diagnostics.Process.GetProcessesByName(processName).Any();
        }

        // Event handler for the "Add Backup" button click
        private void ButtonShowViewAddBackup_Click(object sender, RoutedEventArgs e)
        {
            // Check if any forbidden process is running based on settings in ProcessWatcher
            if (ProcessWatcher.Instance.BloquerNotepad && IsProcessRunning("Notepad") ||
                ProcessWatcher.Instance.BloquerCalculator && IsProcessRunning("CalculatorApp"))
            {
                MessageBox.Show(
                    $"{LanguageManager.GetText("action_blocked_software")}",
                    LanguageManager.GetText("alert_software"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }
            else
            {
                // Open the ViewAddBackup window and close the current window
                ViewAddBackup viewAddBackup = new ViewAddBackup();
                viewAddBackup.Show();
                this.Close();
            }
        }

        // Event handler for the "Execute Backup" button click
        private void ButtonShowViewExecuteBackup_Click(object sender, RoutedEventArgs e)
        {
            // Check if any forbidden process is running
            if (ProcessWatcher.Instance.BloquerNotepad && IsProcessRunning("Notepad") ||
                ProcessWatcher.Instance.BloquerCalculator && IsProcessRunning("CalculatorApp"))
            {
                MessageBox.Show(
                    $"{LanguageManager.GetText("action_blocked_software")}",
                    LanguageManager.GetText("alert_software"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }
            else
            {
                // Open the ViewExecuteBackup window and close the current window
                ViewExecuteBackup viewExecuteBackup = new ViewExecuteBackup();
                viewExecuteBackup.Show();
                this.Close();
            } 
        }

        // Event handler for the "Recover Backup" button click
        private void ButtonShowViewRecoverBackup_Click(object sender, RoutedEventArgs e)
        {
            // Check if any forbidden process is running
            if (ProcessWatcher.Instance.BloquerNotepad && IsProcessRunning("Notepad") ||
                ProcessWatcher.Instance.BloquerCalculator && IsProcessRunning("CalculatorApp"))
            {
                MessageBox.Show(
                    $"{LanguageManager.GetText("action_blocked_software")}",
                    LanguageManager.GetText("alert_software"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }
            else
            {
                // Open the ViewRecoverBackup window and close the current window
                ViewRecoverBackup viewRecoverBackup = new ViewRecoverBackup();
                viewRecoverBackup.Show();
                this.Close();
                
            }
        }

        // Event handler for the "Logs" button click
        private void ButtonShowViewLogs_Click(object sender, RoutedEventArgs e)
        {
            // Open the ViewLogs window and close the current window
            ViewLogs viewLogs = new ViewLogs();
            viewLogs.Show();
            this.Close();
        }

        // Event handler for the "Parameters" button click
        private void ButtonShowViewParameter_Click(object sender, RoutedEventArgs e)
        {
            // Open the ViewParameter window and close the current window
            ViewParameter viewParameter = new ViewParameter();
            viewParameter.Show();
            this.Close();
        }

        // Method to update UI elements of the main window with language-specific texts
        private void UpdateUILanguageMainWindows()
        {
            ButtonShowViewAddBackup.Content = LanguageManager.GetText("button_add_backup");
            ButtonShowViewParameter.Content = LanguageManager.GetText("button_parameter");
            ButtonShowViewExecuteBackup.Content = LanguageManager.GetText("button_execute_backup");
            ButtonShowViewRecoverBackup.Content = LanguageManager.GetText("button_recover_backup");
        }
    }
    //------------Class MainWindow------------//
}
//---------------------View---------------------//