using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Lab8Client
{
    class Client
    {
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 3005;

        private TcpClient tcpClient = new TcpClient();

        public void StayConnected(int msgPeriod)
        {
            tcpClient.Connect(IPAddress.Parse(SERVER_IP), SERVER_PORT);

            var msg = Encoding.UTF8.GetBytes("ALIVE");
            while (true)
            {
                tcpClient.Client.Send(msg);
                Thread.Sleep(msgPeriod);
            }
        }
    }
}
