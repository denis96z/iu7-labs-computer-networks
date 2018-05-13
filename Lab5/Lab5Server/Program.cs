using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5Server
{
    class Program
    {
        static void Main(string[] args)
        {
            new HttpServer().Start();
            while (true) ;
        }
    }
}
