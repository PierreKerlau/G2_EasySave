﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Livrable1.View;

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
        }

        private void ButtonShowViewAddBackup_Click(object sender, RoutedEventArgs e)
        {
            // Ouverture de la vue ViewAddBackup et fermeture de la vue principale
            ViewAddBackup viewAddBackup = new ViewAddBackup();
            viewAddBackup.Show();
            this.Close();
        }

        private void ButtonShowViewExecuteBackup_Click(object sender, RoutedEventArgs e)
        {
            // Ouverture de la vue ViewExecuteBackup et fermeture de la vue principale
            ViewExecuteBackup viewExecuteBackup = new ViewExecuteBackup();
            viewExecuteBackup.Show();
            this.Close();
        }

        private void ButtonShowViewRecoverBackup_Click(object sender, RoutedEventArgs e)
        {
            // Ouverture de la vue ViewRecoverBackup et fermeture de la vue principale
            ViewRecoverBackup viewRecoverBackup = new ViewRecoverBackup();
            viewRecoverBackup.Show();
            this.Close();
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
    }
}