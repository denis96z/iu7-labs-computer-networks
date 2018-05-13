using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5
{
    abstract class HttpComponent
    {
        protected const string IP = "127.0.0.1";
        protected const int PORT = 8080;

        protected const string OK = "200 OK";

        protected const string BAD_REQUEST = "400 Bad Request";
        protected const string NOT_FOUND = "404 Not Found";
        protected const string METHOD_NOT_ALLOWED = "405 Method Not Allowed";
    }
}
