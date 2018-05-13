using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new ServerManager().StartServer();
                while (true)
                {
                    string command = Console.ReadLine();
                    if (command == "SHUTDOWN") break;
                }
            }
            catch
            {
                Console.WriteLine("Не удалось запустить сервер.");
                Console.WriteLine("Нажмите любую клавишу для завершения...");
                Console.ReadKey();
            }
        }
    }
}
