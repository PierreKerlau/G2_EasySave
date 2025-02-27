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
        // Event to notify when a property has changed
        public event PropertyChangedEventHandler PropertyChanged;

        // Method to notify that a property has changed
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Private fields for the TcpClient and NetworkStream used to communicate with the server
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;

        // ObservableCollection to hold backup information and bind it to the UI
        public ObservableCollection<SaveInformation> Backups { get; set; } = new ObservableCollection<SaveInformation>();

        // Constructor of MainWindow
        public MainWindow()
        {
            InitializeComponent(); // Initialize components (UI)
            DataContext = this; // Set the data context for binding
            StartClient(); // Start the client connection to the server
        }

        // Method to start the client and connect to the server
        public async void StartClient()
        {
            try
            {
                // Attempt to connect to the server at localhost on port 8888
                _tcpClient = new TcpClient("127.0.0.1", 8888); // Connect to server at port 8888
                _networkStream = _tcpClient.GetStream(); // Get the network stream for communication
                MessageBox.Show("Connected to the server"); // Notify the user that the connection is established

                // Continuously read server messages in a separate thread
                Task.Run(() => ListenForProgressUpdates()); // Start listening for server messages in the background
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.ToString()}"); // Show error message if connection fails
            }
        }

        // Method to listen for progress updates from the server
        private async Task ListenForProgressUpdates()
        {
            byte[] buffer = new byte[1024]; // Buffer to store received data
            int bytesRead;

            try
            {
                // Continuously read from the network stream while the connection is open
                while ((bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    // Convert the received bytes to a string message
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string[] backupParts = message.Split('|'); // Split the message by '|'

                    List<SaveInformation> backups = new List<SaveInformation>(); // List to hold parsed backup information
                    foreach (string backupPart in backupParts) // Loop through each part of the message
                    {
                        string[] parts = backupPart.Split('*'); // Split the part by '*'

                        if (parts.Length == 4) // Ensure that the part has exactly 4 elements
                        {
                            string backupName = parts[0]; // Extract the backup name
                            string sourcePath = parts[1]; // Extract the source path
                            string destinationPath = parts[2]; // Extract the destination path
                            if (int.TryParse(parts[3].TrimEnd('%'), out int progress)) // Try to parse the progress as an integer
                            {
                                // Create a new SaveInformation object and populate it with the parsed data
                                SaveInformation Backup = new SaveInformation
                                {
                                    NameSave = backupName,
                                    SourcePath = sourcePath,
                                    DestinationPath = destinationPath,
                                    Progression = progress
                                };

                                backups.Add(Backup); // Add the backup information to the list
                            }
                        }
                    }

                    // Update the UI with the new backup data
                    Dispatcher.Invoke(() =>
                    {
                        UpdateProgress(backups); // Update the UI on the main thread
                    });
                }
            }
            catch (Exception ex)
            {
                // If an exception occurs (e.g., server disconnects), show an error message
                MessageBox.Show("Disconnected from server: " + ex.Message);
                _networkStream?.Close(); // Close the network stream
                _tcpClient.Close(); // Close the TCP connection
            }
        }

        // SaveInformation class to hold backup information, implements INotifyPropertyChanged for data binding
        public class SaveInformation : INotifyPropertyChanged
        {
            // Event for notifying property changes
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            // Properties for Save Information
            public string NameSave { get; set; } = ""; // Name of the save
            public string SourcePath { get; set; } = ""; // Source path for the backup
            public string DestinationPath { get; set; } = ""; // Destination path for the backup

            // Progression property, supports INotifyPropertyChanged for UI updates
            private double _progression;
            public double Progression
            {
                get { return _progression; }
                set
                {
                    if (_progression != value)
                    {
                        _progression = value; // Set the progress value
                        OnPropertyChanged(nameof(Progression)); // Notify that the Progression property has changed
                    }
                }
            }
        }

        // Method to update the UI with new backup progress data
        private void UpdateProgress(List<SaveInformation> NewBackups)
        {
            // Clear the existing backups in the collection
            Backups.Clear();
            foreach (var backup in NewBackups)
            {
                Backups.Add(backup); // Add the new backup information to the ObservableCollection
            }
        }

        // Override the OnClosed method to handle cleanup when the window is closed
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e); // Call the base class method

            // If the client is connected, close the connection properly
            if (_tcpClient != null && _tcpClient.Connected)
            {
                _networkStream?.Close(); // Close the network stream
                _tcpClient.Close(); // Close the TCP connection
                MessageBox.Show("Disconnected from the server."); // Notify the user that the client has disconnected
            }
        }
    }
}
