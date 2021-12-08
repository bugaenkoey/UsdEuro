using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    internal class ServerCurse
    {
        private IPAddress iPAddress;
        private int port;

        private TcpListener tcpListener;
        public int MaxClient { get; set; }
        public ServerCurse(IPAddress iPAddress, int port)
        {
            this.iPAddress = iPAddress;
            this.port = port;
            MaxClient = 2;
            Start();

        }
        private void Start()
        {
            try
            {
                tcpListener = new TcpListener(iPAddress, port);
                tcpListener.Start(MaxClient);

                while (true)
                {
                    //сервер сообщает клиенту о готовности
                    //к соединению
                    TcpClient client = tcpListener.AcceptTcpClient();


                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"\t NEW CONNECT");
                    Console.ResetColor();

                    ConnectedClient connectedClient = new ConnectedClient(client);

                    Thread clientThread = new Thread(new ThreadStart(connectedClient.Communication));
                    clientThread.Start();
                }
            }

            catch (SocketException sockEx)
            {
                Console.WriteLine("Ошибка сокета:" + sockEx.Message);
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Ошибка:" + Ex.Message);
            }
        }
    }


    internal class ConnectedClient
    {


        LogConnect logConnect;

        private static int count = 0;
        private static decimal curse = GetCourse();
        private static byte roundCurse = 2;


        private TcpClient client;

        public int maxClientCount = 5;

        public ConnectedClient(TcpClient client)
        {
            this.client = client;
            ++count;

            logConnect = new LogConnect();
            logConnect.socket = client.Client.RemoteEndPoint.ToString();
            logConnect.BeginDateTime = DateTime.Now;
            logConnect.curse = curse;

        }

        private static decimal GetCourse()
        {
            try
            {


                HttpWebRequest reqw = (HttpWebRequest)HttpWebRequest.
                    Create("https://v2.api.forex/historics/2018-10-24.json?to=EUR&key=demo");
                HttpWebResponse resp = (HttpWebResponse)reqw.
                GetResponse(); //создаем объект отклика
                StreamReader sr = new StreamReader(
                    resp.GetResponseStream(), Encoding.Default);

                //создаем поток для чтения отклика
                string response = sr.ReadToEnd();

                //извлекаем курс валют из JSON строки
                string[] stringeS = response.Split(new char[] { ':', '}' });
                response = stringeS[stringeS.Length - 4];

                decimal.TryParse(response, out decimal newCurse);


                sr.Close();
                return newCurse;
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Ошибка Curse:" +
                Ex.Message);

                return -1;
            }
        }



        internal void Communication()
        {
            //чтение данных из сети в формате
            //Unicode
            NetworkStream networkStream = null;
            string request, message;


            try
            {
                do
                {
                    networkStream = client.GetStream();
                    request = GetMessage();


                    if (count >= maxClientCount)
                    {
                        message = $"\t count:{count}--> Max {maxClientCount}\n Please wait!!!" +
                            $" сервер сейчас находится под максимальной нагрузкой и " +
                            $"необходимо попробовать подключиться через какое - то время";

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        SendMessage(message);
                        Console.ResetColor();

                        Disconnect();
                        //  continue;
                        return;
                    }


                    //при получении сообщения EXIT
                    //завершить приложение

                    if (request.ToUpper() == "USD EURO")
                    {
                        message = $"Curse:{request} = {Math.Round(curse, roundCurse)}";
                        SendMessage(message);
                    }
                    if (request.ToUpper() == "EURO USD")
                    {
                        message = $"Curse: {request} = {Math.Round(1 / curse, roundCurse)}";
                        SendMessage(message);
                    }
                    if (request.ToUpper() == "EXIT")
                    {
                        //   message = $"{request}";
                        Console.ForegroundColor = ConsoleColor.Red;
                        SendMessage(request);
                        Console.ResetColor();

                        Disconnect();
                        return;
                    }




                    //  client.Close();
                } while (true);

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }

            finally
            {

                //   Disconnect();
            }

            void Disconnect()
            {
                count--;
                if (networkStream != null)
                    networkStream.Close();//отключение потока
                if (client != null)
                    client.Close();//отключение клиента
                                   // this.Close();
                                   // Environment.Exit(0); //завершение процесса
                logConnect.EndDateTime = DateTime.Now;
                logConnect.Save();
            }


            void SendMessage(string message)
            {
                byte[] byContinue = Encoding.Unicode.GetBytes($"{logConnect.socket} | count:{count}--> {message}\n");
                networkStream.Write(byContinue, 0, byContinue.Length);

                Console.WriteLine(message);
            }

            string GetMessage()
            {
                // получаем 
                byte[] data = new byte[32]; // буфер для получаемых данных
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = networkStream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (networkStream.DataAvailable);

                return builder.ToString();
            }
        }
    }

    public class LogConnect
    {
        public string socket;
        public DateTime BeginDateTime;
        public decimal curse;
        public DateTime EndDateTime;

        public LogConnect() { }
        public void Save()
        {
            string log = $"{socket}|{BeginDateTime.ToString("O")}|{curse}|{EndDateTime.ToString("O")}";
            Console.WriteLine(log);
            //C:\Users\asus\source\repos\UsdEuro\Server
            string writePath = @"C:\Users\asus\source\repos\UsdEuro\Server\Log.txt";

            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, true, Encoding.Default))
                {
                    sw.WriteLine(log);
                }
                Console.WriteLine("Запись выполнена");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}