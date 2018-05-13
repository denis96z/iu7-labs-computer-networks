using System;
using System.Net.Sockets;

namespace Lab8Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var client = new Client();
                client.StayConnected(100);
            }
            catch (SocketException)
            {
                //NOP
            }
        }
    }
}
