using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Lab8Server
{
    class Server
    {
        private const int MAX_CLIENTS = 3;
        private const int SERVER_PORT = 3005;

        private bool[] clients = new bool[MAX_CLIENTS];
        private TcpListener tcpListener = new TcpListener(IPAddress.Any, SERVER_PORT);

        public void Start()
        {
            try
            {
                tcpListener.Start();

                while (true)
                {
                    var clientSocket = tcpListener.AcceptSocket();

                    int id = GetFreeID();
                    if (id == -1)
                    {
                        Console.WriteLine("Попытка соединения: нет свободных id.");

                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
                    }
                    else
                    {
                        Console.WriteLine("Новое соединение: id=" + id);

                        clients[id] = true;
                        new Thread(() =>
                        {
                            Poll(clientSocket);
                            clients[id] = false;

                            Console.WriteLine("Соединение закрыто: id=" + id);
                        }).Start();
                    }
                }
            }
            catch (SocketException)
            {
                //NOP
            }
        }

        private int GetFreeID()
        {
            for (int i = 0; i < MAX_CLIENTS; i++)
            {
                if (!clients[i])
                {
                    return i;
                }
            }
            return -1;
        }

        private void Poll(Socket clientSocket)
        {
            try
            {
                const string MSG = "ALIVE";
                string receivedMsg = null;

                var buffer = new byte[MSG.Length];
                clientSocket.ReceiveTimeout = 5000;

                do
                {
                    int length = clientSocket.Receive(buffer);
                    receivedMsg = Encoding.UTF8.GetString(buffer, 0, length);
                }
                while (receivedMsg == MSG);
            }
            catch (SocketException)
            {
                //NOP
            }
        }

        public void Stop()
        {
            tcpListener.Stop();
        }
    }
}
