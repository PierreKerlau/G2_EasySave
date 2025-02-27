using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Net.Http;
using System.Windows.Interop;
using Livrable1.ViewModel;

namespace Livrable1.Model
{
    // Server class is responsible for managing TCP client connections and broadcasting updates to connected clients
    public class Server
    {
        private static Server _instance; // Singleton instance of Server
        private TcpListener _tcpListener; // Listener for incoming TCP connections
        private bool _isRunning; // Indicates if the server is running
        private List<TcpClient> tcpClients = new List<TcpClient>(); // List of connected clients

        public event EventHandler ServerDisconnected; // Event to notify when the server gets disconnected

        // Singleton instance of the Server
        public static Server Instance
        {
            get
            {
                // Create a new instance if one doesn't exist
                if (_instance == null)
                {
                    _instance = new Server(); // Create new instance if it doesn't exist
                }
                return _instance; // Return singleton instance
            }
        }

        // Method to start the server on a specified port
        public void StartServer(int port)
        {
            // Prevent restarting the server if it's already running
            if (_isRunning)
            {
                return;
            }

            try
            {
                _tcpListener = new TcpListener(IPAddress.Loopback, port); // Initialize TCP listener
                _tcpListener.Start(); // Start listening for connections
                _isRunning = true; // Set running state to true
                //MessageBox.Show("Server started on port " + port);

                // Start listening for incoming client connections in a separate task (thread)
                Task.Run(() => ListenForClients());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageManager.GetText("error_starting_server")}: {ex.Message}");
            }
        }

        // Asynchronous method to listen for incoming client connections
        private async Task ListenForClients()
        {
            while (_isRunning)
            {
                try
                {
                    // Accept an incoming client connection asynchronously
                    TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync();
                  
                    // Lock the tcpClients list to ensure thread-safety when adding a new client
                    lock (tcpClients)
                    {
                        tcpClients.Add(tcpClient); // Add the new client to the list
                    }

                    // Start handling the connected client in a new task
                    Task.Run(() => HandleClient(tcpClient));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{LanguageManager.GetText("error_accepting_client")}: {ex.Message}");
                }
            }
        }

        // Asynchronous method to handle communication with a specific client
        private async Task HandleClient(TcpClient tcpClient)
        {
            try
            {
                // Get the network stream for reading/writing data with the client
                NetworkStream networkStream = tcpClient.GetStream();
                byte[] buffer = new byte[1024]; // Buffer to hold incoming data

                // Loop to listen for incoming messages from the client
                while (true)
                {
                    // Read data from the client asynchronously
                    int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break; // Exit the loop if no data was read (client disconnected)
                    }

                    // Convert the received byte data to a string
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageManager.GetText("error_handling_client")}: {ex.Message}");
            }
            finally
            {
                tcpClient.Close(); // Close the client connection
            }
        }

        // Method to send progress updates to the connected clients
        public void SendProgressUpdate(List<SaveInformation> Backups)
        {
            string message = "";

            foreach (var backup in Backups)
            {
                string backupName = backup.NameSave;
                string sourcePath = backup.SourcePath;
                string destinationPath = backup.DestinationPath;
                int progress = (int)backup.Progression;

                // Construct message for each backup
                if (message.Length == 0)
                {
                    message = $"{backupName}*{sourcePath}*{destinationPath}*{progress}%";
                }
                else
                {
                    message += "|" + $"{backupName}*{sourcePath}*{destinationPath}*{progress}%";
                }
            }

            // Broadcast the constructed message to all connected clients
            Broadcast(message);
        }

        // Asynchronous method to broadcast a message to all connected clients
        public async void Broadcast(string message)
        {
            // Loop through all connected clients and send the message to each
            foreach (var client in tcpClients)
            {
                NetworkStream networkStream = client.GetStream(); // Get the network stream for the client
                byte[] data = Encoding.UTF8.GetBytes(message); // Convert message to bytes
                await networkStream.WriteAsync(data, 0, data.Length); // Send data asynchronously
            }
        }
    }
}
