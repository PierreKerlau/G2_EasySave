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

namespace Livrable1.Model
{
    public class Server
    {
        private static Server _instance;

        private TcpListener _tcpListener;

        private bool _isRunning;

        private List<TcpClient> tcpClients = new List<TcpClient>();

        public static Server Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Server();
                }
                return _instance;
            }
        }

        // Method to start the server
        public void StartServer(int port)
        {
            if (_isRunning)
            {
                return; // Server is not restarted if it is already started
            }

            try
            {
                _tcpListener = new TcpListener(IPAddress.Loopback, port);
                _tcpListener.Start();
                _isRunning = true;
                MessageBox.Show("Server strated on port " + port);

                // Listens for incoming connections in a separate thread
                Task.Run(() => ListenForClients());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting server: " + ex.Message);
            }
        }

        private async Task ListenForClients()
        {
            while (_isRunning)
            {
                try
                {
                    // Wait for a client to connect
                    TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync();
                    //MessageBox.Show("Client connected");
                    lock (tcpClients)
                    {
                        tcpClients.Add(tcpClient);
                    }

                    // Start handling the client in a new task
                    Task.Run(() => HandleClient(tcpClient));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error accepting client: {ex.Message}");
                }
            }
        }

        private async Task HandleClient(TcpClient tcpClient)
        {
            try
            {
                NetworkStream networkStream = tcpClient.GetStream();
                byte[] buffer = new byte[1024];

                Broadcast("test");

                //Loop to listen for messages from the client
                while (true)
                {
                    int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Message from client: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                tcpClient.Close();
            }
        }

        public void SendProgressUpdate(List<SaveInformation> Backups)
        { 

            string message = "";
            foreach (var backup in Backups)
            {
                string backupName = backup.NameSave;
                string sourcePath = backup.SourcePath;
                string destinationPath = backup.DestinationPath;
                int progress = (int)backup.Progression;

                if (message.Length == 0)
                {
                    message = $"{backupName}*{sourcePath}*{destinationPath}*{progress}%";
                }
                else
                {
                    message = message + "|" + $"{backupName}*{sourcePath}*{destinationPath}*{progress}%";
                }
                
            }
            Broadcast(message); // Send the data to the connected clients
        }

        public async void Broadcast(string message)
        {
            foreach (var client in tcpClients)
            {
                NetworkStream networkStream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(message);
                await networkStream.WriteAsync(data, 0, data.Length);
            }
        }
        public void StopServer()
        {
            _isRunning = false;
            _tcpListener.Stop();
            Console.WriteLine("Server stopped");
        }
    }
}
