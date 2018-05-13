using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Lab5Server
{
    class HttpServer : Lab5.HttpComponent
    {
        private Socket serverSocket = null;

        private bool serverActive = false;
        private Mutex serverActiveMutex = new Mutex();

        private Mutex logMutex = new Mutex();

        private const string ROOT_PATH = @"C:\University\LabsСN\Lab5\Lab5Server\bin\Debug\html\";
        private const int MAX_CONNECTIONS = 20;

        public HttpServer()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(IP), PORT));
            serverSocket.Listen(MAX_CONNECTIONS);
        }

        ~HttpServer()
        {
            serverSocket.Close();
        }

        public void Start()
        {
            serverActiveMutex.WaitOne();
            if (serverActive) return;
            serverActive = true;
            serverActiveMutex.ReleaseMutex();

            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        serverActiveMutex.WaitOne();
                        if (!serverActive) break;
                        serverActiveMutex.ReleaseMutex();

                        Socket clientSocket = serverSocket.Accept();
                        WriteLog("Accepted connection...");
                        new Thread(() => HandleRequest(clientSocket)).Start();
                    }
                    catch (Exception e)
                    {
                        WriteLog(e.Message);
                    }
                }
            }).Start();
        }

        public void Stop()
        {
            serverActiveMutex.WaitOne();
            serverActive = false;
            serverActiveMutex.ReleaseMutex();
        }

        private void HandleRequest(Socket clientSocket)
        {
            try
            {
                byte[] buffer = new byte[5];

                StringBuilder requestStrBuilder = new StringBuilder();
                do
                {
                    int length = clientSocket.Receive(buffer);
                    requestStrBuilder.Append(Encoding.UTF8.GetString(buffer, 0, length));
                }
                while (clientSocket.Available > 0);
                string request = requestStrBuilder.ToString();

                WriteLog("Received request:\r\n" + request);

                string response = "HTTP/1.1 ";
                if (request.Length < 16)
                {
                    response += BAD_REQUEST + "\r\n";
                }
                else if (request.Substring(0, 3) != "GET")
                {
                    response += METHOD_NOT_ALLOWED + "\r\n";
                }
                else if (request[3] != ' ' || request[4] != '/')
                {
                    response += BAD_REQUEST + "\r\n";
                }
                else
                {
                    request = request.Substring(5, request.Length - 5);

                    int index = 0;
                    for (; index < request.Length && request[index] != ' '; index++) ;
                    if (index == request.Length)
                    {
                        response += BAD_REQUEST + "\r\n";
                    }
                    else
                    {
                        string filePath = request.Substring(0, index).Replace('/', '\\');

                        if (request.Substring(index + 1, 10) != "HTTP/1.1\r\n")
                        {
                            response += BAD_REQUEST + "\r\n";
                        }
                        else
                        {
                            string fileName = ROOT_PATH + (filePath == String.Empty ?
                                "index.html" : filePath + ".html");

                            if (File.Exists(fileName))
                            {
                                response += OK + "\r\nContent-Type: text/html; charset=UTF-8\r\n\r\n" +
                                    File.ReadAllText(fileName);
                            }
                            else
                            {
                                response += NOT_FOUND + "\r\n";
                            }
                        }
                    }
                }

                buffer = Encoding.UTF8.GetBytes(response);
                clientSocket.Send(buffer);

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();

                WriteLog("Sent response:\r\n" + response.ToString());
            }
            catch (Exception e)
            {
                WriteLog(e.Message);
            }
        }

        private void WriteLog(string log)
        {
            logMutex.WaitOne();
            Console.WriteLine(DateTime.Now + ": " + log);
            logMutex.ReleaseMutex();
        }
    }
}
