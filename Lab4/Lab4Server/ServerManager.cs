using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ConnectionManager = Lab4.ConnectionManager;

namespace Lab4Server
{
    class ServerManager : ConnectionManager
    {
        private Socket mainSocket = null;

        private const int MAX_QUEUE_LENGTH = 10;
        private const string FILES_PATH = @"received\";

        private const int FILE_NAME_BUFFER_LENGTH = 50;
        private const int FILE_BUFFER_LENGTH = 200;

        public ServerManager()
        {
            mainSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            mainSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
        }

        public void StartServer()
        {
            mainSocket.Listen(MAX_QUEUE_LENGTH);
            Task mainTask = new Task(() => HandleRequests());
            mainTask.Start();
            WriteLog("Сервер запущен.");
        }

        public void HandleRequests()
        {
            try
            {
                while (true)
                {
                    Socket clientSocket = mainSocket.Accept();
                    Task handlerTask = new Task(() => HandleRequest(clientSocket));
                    handlerTask.Start(); 
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        public void HandleRequest(Socket clientSocket)
        {
            try
            {
                WriteLog("Установлено соединение.");

                byte[] buffer = new byte[FILE_NAME_BUFFER_LENGTH];
                StringBuilder fileName = new StringBuilder();
                do
                {
                    int length = clientSocket.Receive(buffer);
                    fileName.Append(Encoding.Unicode.GetString(buffer, 0, length));
                }
                while (clientSocket.Available > 0);

                WriteLog("Запрос на передачу файла \"" + fileName + "\".");

                string response = String.Empty;
                string fullFileName = FILES_PATH + fileName;

                if (File.Exists(fullFileName))
                {
                    response = DENIED_RESPONSE;
                    buffer = Encoding.Unicode.GetBytes(response);

                    clientSocket.Send(buffer);
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();

                    WriteLog("Файл \"" + fileName + "\" существует. В передаче отказано.");
                    return;
                }

                response = ALLOWED_RESPONSE;
                buffer = Encoding.Unicode.GetBytes(response);
                clientSocket.Send(buffer);

                using (BinaryWriter writer = new BinaryWriter(
                    File.Open(fullFileName, FileMode.Create)))
                {
                    WriteLog("Начат прием файла \"" + fileName + "\".");

                    buffer = new byte[FILE_BUFFER_LENGTH];
                    do
                    {
                        int length = clientSocket.Receive(buffer);
                        foreach (byte b in buffer)
                        {
                            writer.Write(b);
                        }
                    }
                    while (clientSocket.Available > 0);

                    WriteLog("Окончен прием файла \"" + fileName + "\".");
                    writer.Close();
                }

                response = ACCEPTED_RESPONSE;
                buffer = Encoding.Unicode.GetBytes(response);

                clientSocket.Send(buffer);
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }

        private Mutex consoleMutex = new Mutex();

        private void HandleException(Exception exception)
        {
            WriteLog(exception.Message);
            StartServer();
        }

        private void WriteLog(string message)
        {
            consoleMutex.WaitOne();
            Console.WriteLine(DateTime.Now + ": " + message);
            consoleMutex.ReleaseMutex();
        }
    }
}
