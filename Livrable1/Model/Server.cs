//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Threading;
//using System.Net.Sockets;
//using System.Net;
//using System.IO;
//using System.Runtime.CompilerServices;

//namespace Livrable1.Model
//{
//    public class Server
//    {
//        private static Server _instance;

//        private TcpListener _tcpListener;
//        private bool _isRunning;

//        // Private constructor to prevent direct instantiation
//        //private Server() { }

//        public static Server Instance
//        {
//            get
//            {
//                if (_instance == null)
//                {
//                    _instance = new Server();
//                }
//                return _instance;
//            }
//        }

//        // Method to start the server
//        public void StartServer(int port)
//        {
//            _tcpListener = new TcpListener(IPAddress.Any, port);
//            _tcpListener.Start();
//            _isRunning = true;
//            Console.WriteLine("Server strated on port " + port);

//            // Listens for incoming connections in a separate thread
//            Task.Run(() => ListenForClients());
//        }

//        private async Task ListenForClients()
//        {
//            while (_isRunning)
//            {
//                // Accepts new connection
//                TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync();

//                // Starts a new thread to manage communication with this client
//                Task.Run(() => HandleClient(tcpClient));
//            }
//        }

//        private async Task HandleClient(TcpClient client)
//        {
//            Console.WriteLine("Client connected");

//            // Obtains the client's data stream
//            NetworkStream stream = client.GetStream();

//            byte[] buffer = new byte[1024];
//            int bytesRead;

//            // Receives data and manages the communication
//            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
//            {
//                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
//                Console.WriteLine("Message from client: " + message);

//                // Sending of progression
//                await SendProgressUpdate(stream, "Backup1", 50); // EXEMPLE
//            }
//            client.Close();
//        }

//        private async Task SendProgressUpdate(NetworkStream stream, string backupName, int progress)
//        {
//            string message = $"{backupName}:{progress}%";
//            byte[] data = Encoding.UTF8.GetBytes(message);
//            await stream.WriteAsync(data, 0, data.Length);
//        }

//        public void StopServer()
//        {
//            _isRunning = false;
//            _tcpListener.Stop();
//            Console.WriteLine("Server stopped");
//        }
//    }
//}
