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

namespace Livrable1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateUILanguageMainWindows();
        }

        //------------------------------------------------------------------------------------//
        private bool IsProcessRunning(string processName)
        {
            return System.Diagnostics.Process.GetProcessesByName(processName).Any();
        }
        //------------------------------------------------------------------------------------//

        private void ButtonShowViewAddBackup_Click(object sender, RoutedEventArgs e)
        {
            //--------------------------------------------------------------------------------//
            if (ProcessWatcher.Instance.BloquerCheatEngine && IsProcessRunning("Notepad") ||
                ProcessWatcher.Instance.BloquerWireshark && IsProcessRunning("CalculatorApp"))
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
                //--------------------------------------------------------------------------------//
                ViewAddBackup viewAddBackup = new ViewAddBackup();
                viewAddBackup.Show();
                this.Close();
                //--------------------------------------------------------------------------------//
            }
            //--------------------------------------------------------------------------------//
        }

        private void ButtonShowViewExecuteBackup_Click(object sender, RoutedEventArgs e)
        {
            //--------------------------------------------------------------------------------//
            if (ProcessWatcher.Instance.BloquerCheatEngine && IsProcessRunning("Notepad") ||
                ProcessWatcher.Instance.BloquerWireshark && IsProcessRunning("CalculatorApp"))
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
                //--------------------------------------------------------------------------------//
                ViewExecuteBackup viewExecuteBackup = new ViewExecuteBackup();
                viewExecuteBackup.Show();
                this.Close();
                //--------------------------------------------------------------------------------//
            }
            //--------------------------------------------------------------------------------//
            
        }

        private void ButtonShowViewRecoverBackup_Click(object sender, RoutedEventArgs e)
        {
            //--------------------------------------------------------------------------------//
            if (ProcessWatcher.Instance.BloquerCheatEngine && IsProcessRunning("Notepad") ||
                ProcessWatcher.Instance.BloquerWireshark && IsProcessRunning("CalculatorApp"))
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
                //--------------------------------------------------------------------------------//
                ViewRecoverBackup viewRecoverBackup = new ViewRecoverBackup();
                viewRecoverBackup.Show();
                this.Close();
                //--------------------------------------------------------------------------------//
            }
            //--------------------------------------------------------------------------------//

        }

        private void ButtonShowViewLogs_Click(object sender, RoutedEventArgs e)
        {
            // Ouverture de la vue ViewLogs et fermeture de la vue principale
            ViewLogs viewLogs = new ViewLogs();
            viewLogs.Show();
            this.Close();
        }

        private void ButtonShowViewParameter_Click(object sender, RoutedEventArgs e)
        {
            // Ouverture de la vue ViewParameter et fermeture de la vue principale
            ViewParameter viewParameter = new ViewParameter();
            viewParameter.Show();
            this.Close();
        }

        private void UpdateUILanguageMainWindows()
        {
            ButtonShowViewAddBackup.Content = LanguageManager.GetText("button_add_backup");
            ButtonShowViewParameter.Content = LanguageManager.GetText("button_parameter");
            ButtonShowViewExecuteBackup.Content = LanguageManager.GetText("button_execute_backup");
            ButtonShowViewRecoverBackup.Content = LanguageManager.GetText("button_recover_backup");
        }
    }
}
