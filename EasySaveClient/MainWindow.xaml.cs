using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Packaging;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EasySaveClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Notifie le changement de propriété
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private TcpClient _tcpClient;
        private NetworkStream _networkStream;

        public ObservableCollection<SaveInformation> Backups { get; set; } = new ObservableCollection<SaveInformation>();


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            StartClient();
        }

        public async void StartClient()
        {
            try
            {
                _tcpClient = new TcpClient("127.0.0.1", 8888); // Connect to the server on port 8080
                _networkStream = _tcpClient.GetStream();
                MessageBox.Show("Connected to the server");

                // Continuous reading of server messages in a separate thread
                Task.Run(() => ListenForProgressUpdates());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.ToString()}");
            }
        }

        private async Task ListenForProgressUpdates()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;
            
            while ((bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead); // récupère un message string
                string[] backupParts = message.Split('|'); // découpe le message au niveau des |

                List<SaveInformation> backups = new List<SaveInformation>(); // créer une liste vide 
                foreach (string backupPart in backupParts) // parcourt le tableau backupParts
                {
                    string[] parts = backupPart.Split('*'); // découpe le message au niveau des :
                    
                    if (parts.Length == 4)
                    {
                        string backupName = parts[0]; // récupère le nom de la sauvegarde
                        string sourcePath = parts[1]; // récupère le chemin source
                        string destinationPath = parts[2]; // récupère le chemin destination
                        if (int.TryParse(parts[3].TrimEnd('%'), out int progress)) // récupère la progression sous forme de int dans progress en vérifiant que la converison a fonctionné
                        {
                            SaveInformation Backup = new SaveInformation();
                            Backup.NameSave = backupName;
                            Backup.SourcePath = sourcePath;
                            Backup.DestinationPath = destinationPath;
                            Backup.Progression = progress;

                            backups.Add(Backup); // crée une nouvelle sauvegarde
                        }
                    }
                }
                // Updates the graphical interface with the progress
                Dispatcher.Invoke(() =>
                {
                    UpdateProgress(backups);
                });
            }
        }

        public class SaveInformation : INotifyPropertyChanged
        {
            // Property change notification
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            // Properties for Save Information
            public string NameSave { get; set; } = "";
            public string SourcePath { get; set; } = "";
            public string DestinationPath { get; set; } = "";

            // Progression tracking property with INotifyPropertyChanged support
            private double _progression;
            public double Progression
            {
                get { return _progression; }
                set
                {
                    if (_progression != value)
                    {
                        _progression = value;
                        OnPropertyChanged(nameof(Progression));
                    }
                }
            }
        }

        private void UpdateProgress(List<SaveInformation> NewBackups)
        {
            // Updates progress in the interface (for example, via a DataGrid or ProgressBar)
            Backups.Clear();
            foreach (var backup in NewBackups)
            {
                Backups.Add(backup);
            }
        }
    }
}
