using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RSD {
    class SocketListener {
        private Server server;
        private int port;
        private Thread listenerThread;

        public delegate void SocketMessageReceivedHandler(SocketListener socketListener, TcpClient tcpClient, string message);
        public event SocketMessageReceivedHandler SocketMessageReceived;

        public SocketListener(Server server, int port) {
            this.server = server;
            this.port = port;

            server.Form.Output("Found the following servers:");
            foreach (GameServer gameServer in Server.GameServers) {
                server.Form.Output("- " + gameServer);
            }

            listenerThread = new Thread(new ThreadStart(Listener));
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }

        private void Listener() {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            server.Form.Output("Started listener");

            while (true) {
                TcpClient client = listener.AcceptTcpClient();
                
                string request = GetRequest(client);

                // Debug
                server.Form.Output("Received message");

                // Call the event
                OnSocketMessageReceived(client, request);            
            }
        }

        public void Finalize(TcpClient client, string response) {
            NetworkStream stream = client.GetStream();
            // Finalize the socket message
            try {
                byte[] msg = Encoding.ASCII.GetBytes(response);
                stream.Write(msg, 0, msg.Length);
                client.Close();

                server.Form.Output("Finalize: " + response);
            }
            catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Socket failed to write: " + ex);
                Console.ResetColor();
            }
        }

        private string GetRequest(TcpClient client) {
            NetworkStream stream = client.GetStream();
            byte[] data = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(data, 0, Convert.ToInt32(client.ReceiveBufferSize));
            string request = Encoding.ASCII.GetString(data, 0, bytesRead);

            return request;
        }

        public void OnSocketMessageReceived(TcpClient tcpClient, string message) {
            SocketMessageReceivedHandler handler = SocketMessageReceived;

            // We have subscribers
            if(handler != null) {
                handler(this, tcpClient, message);
            }
        }
    }
}
