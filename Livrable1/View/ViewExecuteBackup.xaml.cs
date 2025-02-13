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

namespace Livrable1.View
{
    /// <summary>
    /// Logique d'interaction pour ViewExecuteBackup.xaml
    /// </summary>
    public partial class ViewExecuteBackup : Window
    {
        public ViewExecuteBackup()
        {
            InitializeComponent();
        }

        private void ButtonLeave_Click_1(object sender, RoutedEventArgs e)
        {
            // Ouverture de la fenêtre principale et fermeture de la fenêtre actuelle
            MainWindow viewMain = new MainWindow();
            viewMain.Show();
            this.Close();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
