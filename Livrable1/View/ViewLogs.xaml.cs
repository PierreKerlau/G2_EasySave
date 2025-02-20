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

//---------------------View---------------------//
namespace Livrable1.View
{
    /// <summary>
    /// Interaction logic for ViewLogs.xaml
    /// </summary>

    //------------Class ViewLogs------------//
    public partial class ViewLogs : Window
    {
        // Constructor for ViewLogs
        public ViewLogs()
        {
            InitializeComponent(); // Initialize UI components
        }

        // Event handler for the leave button click
        private void ButtonLeave_Click_1(object sender, RoutedEventArgs e)
        {
            // Open the main window and close the current window
            MainWindow viewMain = new MainWindow(); // Create a new instance of MainWindow
            viewMain.Show(); // Show the MainWindow
            this.Close(); // Close the current window
        }
    }
    //------------Class ViewLogs------------//
}
//---------------------View---------------------//
