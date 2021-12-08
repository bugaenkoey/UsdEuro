using System;
using System.Net;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IPAddress iPAddress = IPAddress.Loopback;
            int port = 80;
            ClientCurse clientCurse = new ClientCurse(iPAddress, port);
            Console.WriteLine(clientCurse.Dialog("USD EURO"));


            string[] request = { "EXIT", "USD EURO", "EURO USD" };

            ClientCurse[] clients =  {
               new ClientCurse(iPAddress, port),
               new ClientCurse(iPAddress, port),
               new ClientCurse(iPAddress, port),
               new ClientCurse(iPAddress, port),
               new ClientCurse(iPAddress, port),
               new ClientCurse(iPAddress, port),
               new ClientCurse(iPAddress, port),
               new ClientCurse(iPAddress, port),
               new ClientCurse(iPAddress, port),
               new ClientCurse(iPAddress, port)
            };

            //  Console.ReadLine();

            string strRequest;
            Random rnd = new Random();
            int n;
            for (int i = 0; i < 50; i++)
            {
                strRequest = request[rnd.Next(0, 3)];
                n = rnd.Next(0, clients.Length);
                Console.WriteLine("_____________________________________________________________________");
                Console.Write($"{n} => {strRequest}");
                Console.WriteLine(clients[n].Dialog(strRequest));

            }


        }
    }
}
