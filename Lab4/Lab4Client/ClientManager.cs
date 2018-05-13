using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

using ConnectionManager = Lab4.ConnectionManager;

namespace Lab4Client
{
    class ClientManager : ConnectionManager
    {
        private Socket socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

        private const int RESPONSE_BUFFER_LENGTH = 10;
        private const int FILE_BUFFER_LENGTH = 1000;

        public delegate void OnProgress(long numBytes, long sentBytes);

        public void SendFile(string fileName, OnProgress callBack)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Файл не найден.");
            }

            socket.Connect(new IPEndPoint(IPAddress.Parse(IP), PORT));

            var fileInfo = new FileInfo(fileName);
            string shortFileName = fileInfo.Name;
            long fileLength = fileInfo.Length;

            byte[] buffer = Encoding.Unicode.GetBytes(shortFileName);
            socket.Send(buffer);

            buffer = new byte[RESPONSE_BUFFER_LENGTH];
            StringBuilder response = new StringBuilder();
            do
            {
                int length = socket.Receive(buffer);
                response.Append(Encoding.Unicode.GetString(buffer, 0, length));
            }
            while (socket.Available > 0);

            if (response.ToString() == DENIED_RESPONSE)
            {
                throw new Exception("В передаче файла отказано.");
            }

            buffer = File.ReadAllBytes(fileName);
            socket.Send(buffer);
            callBack(fileLength, fileLength);

            buffer = new byte[RESPONSE_BUFFER_LENGTH];
            response = new StringBuilder();
            do
            {
                int length = socket.Receive(buffer);
                response.Append(Encoding.Unicode.GetString(buffer, 0, length));
            }
            while (socket.Available > 0);

            if (response.ToString() != ACCEPTED_RESPONSE)
            {
                throw new Exception("Не удалось передать файл.");
            }
        }
    }
}
