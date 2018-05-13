using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    abstract class ConnectionManager
    {
        protected const string IP = "192.168.1.31";
        protected const int PORT = 30300;

        protected const string ALLOWED_RESPONSE = "ALLOWED";
        protected const string DENIED_RESPONSE = "DENIED";
        protected const string ACCEPTED_RESPONSE = "ACCEPTED";
    }
}
