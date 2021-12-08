using System;
using System.Net;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IPAddress iPAddress = IPAddress.Loopback;
            int port = 80;
            ServerCurse serverCurse = new ServerCurse(iPAddress, port);

        }
    }
}
