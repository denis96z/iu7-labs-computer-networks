using System;
using System.Windows;

namespace Lab8Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server();
            server.Start();
        }
    }
}
