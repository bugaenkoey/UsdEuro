using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class ClientCurse
    {
        private static int count = 0;
        private int id;
        private IPAddress iPAddress;
        private int port;
        TcpClient client;

        NetworkStream networkStream;

        public ClientCurse(IPAddress iPAddress, int port)
        {
            this.iPAddress = iPAddress;
            this.port = port;
            id = count++;
            // client = ClientConnect();
        }

        public string Dialog(string str)
        {
            Console.WriteLine($"\t\t ID:{id}");
            // Console.WriteLine($"Клиент уже подключен? >>>{client.Connected}");

            //  client = ClientConnect();

            if (client != null)
            {
                Console.WriteLine($"Клиент {client.Client.LocalEndPoint.ToString()} уже подключен? >>>{client.Connected}");
                if (!client.Connected)
                {
                    Console.WriteLine($"Подключаем ~ ~~  ~~~   ~~~>{client.Client.LocalEndPoint.ToString()}");
                    client = new TcpClient();
                    client = ClientConnect();
                }
            }
            else
            {
                client = new TcpClient();
                client = ClientConnect();
            }

            try
            {
                //преобразование строки сообщения
                //в массив байт
                byte[] barray = Encoding.Unicode.GetBytes(str);
                //запись в сетевой поток всего массива
                networkStream.Write(barray, 0, barray.Length);

                // получаем ответ
                byte[] data = new byte[32]; // буфер для получаемых данных
                StringBuilder builder = new StringBuilder();
                int bytes = 0;

                do
                {
                    bytes = networkStream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (networkStream.DataAvailable);
                string message = builder.ToString();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Сервер: {message}");
                Console.ResetColor();

                return message;
            }
            catch (Exception Ex)
            {
                //  Console.WriteLine("Ошибка:" + Ex.Message);
                return "Ошибка:" + Ex.Message;
            }
        }

        public TcpClient ClientConnect()
        {
            try
            {
                //  client = new TcpClient();
                //установка соединения с использованием
                //данных IP и номера порта
                client.Connect(IPAddress.Loopback, 80);
                //получение сетевого потока
                networkStream = client.GetStream();
            }
            catch (SocketException sockEx)
            {
                Console.WriteLine("Ошибка сокета:" +
                sockEx.Message);
                // Disconnect();
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Ошибка:" +
                Ex.Message);
                // Disconnect();
            }
            finally
            {
                //  Disconnect();
            }
            return client;
        }

        void Disconnect()
        {
            if (networkStream != null)
                networkStream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
                               // Environment.Exit(0); //завершение процесса
        }
    }
}