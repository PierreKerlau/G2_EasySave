//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows;

//namespace EasySaveClient
//{
//    /// <summary>
//    /// Interaction logic for MainWindow.xaml
//    /// </summary>
//    public partial class MainWindow : Window, INotifyPropertyChanged
//    {
//        public event PropertyChangedEventHandler PropertyChanged;

//        // Notifie le changement de propriété
//        protected void OnPropertyChanged(string propertyName)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }

//        private TcpClient _tcpClient;
//        private NetworkStream _networkStream;

//        public ObservableCollection<SaveInformation> Backups { get; set; } = new ObservableCollection<SaveInformation>();


//        public MainWindow()
//        {
//            InitializeComponent(); 
//            DataContext = this;

//            StartClient();
//        }

//        private async void StartClient()
//        {
//            try
//            {
//                _tcpClient = new TcpClient("127.0.0.1", 8080); // Connect to the server on port 8080
//                _networkStream = _tcpClient.GetStream();

//                // Continuous reading of server messages in a separate thread
//                Task.Run(() => ListenForProgressUpdates());
//            }
//            catch (Exception ex) 
//            {
//                MessageBox.Show($"Error: {ex.Message}");
//            }
//        }

//        private async Task ListenForProgressUpdates()
//        {
//            byte[] buffer = new byte[1024];
//            int bytesRead;

//            while ((bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
//            {
//                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
//                string[] parts = message.Split(':');
//                if (parts.Length == 2)
//                {
//                    string backupName = parts[0];
//                    if (int.TryParse(parts[1].TrimEnd('%'), out int progress))
//                    {
//                        // Updates the graphical interface with the progress
//                        Dispatcher.Invoke(() =>
//                        {
//                            UpdateProgress(backupName, progress);
//                        });
//                    }
//                }
//            }
//        }

//        public class SaveInformation : INotifyPropertyChanged
//        {
//            // Property change notification
//            public event PropertyChangedEventHandler PropertyChanged;
//            protected void OnPropertyChanged(string propertyName)
//            {
//                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//            }

//            // Properties for Save Information
//            public string NameSave { get; set; } = "";
//            public string SourcePath { get; set; } = "";
//            public string DestinationPath { get; set; } = "";

//            // Progression tracking property with INotifyPropertyChanged support
//            private double _progression;
//            public double Progression
//            {
//                get { return _progression; }
//                set
//                {
//                    if (_progression != value)
//                    {
//                        _progression = value;
//                        OnPropertyChanged(nameof(Progression));
//                    }
//                }
//            }
//        }

//        private void UpdateProgress(string backupName, int progress)
//        {
//            // Updates progress in the interface (for example, via a DataGrid or ProgressBar)
//            var backup = Backups.FirstOrDefault(b => b.NameSave == backupName);
//            if (backup != null)
//            {
//                backup.Progression = progress;
//                OnPropertyChanged(nameof(Backups));
//            }
//        }
//    }
//}
