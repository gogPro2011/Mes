using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mes
{
    class Program
    {
        const int PORT = 8888;
        const string adress = "127.0.0.1";

        IPAddress ip = IPAddress.Parse(adress);

        List<NetworkStream> all_clients = new List<NetworkStream>();

        TcpClient client = null;
        TcpListener server = null;

        static void Main()
        {
            var mc = new Program();
            var t = new Task(mc.StartServer);
            t.Start();

            Console.WriteLine("====Start programm====");

            string h;
            

            while (true){ 
                h = Console.ReadLine();
                switch (h) {
                    
                    case ("Send msg"):
                        break;
                    case ("Stop"):
                        mc.server = null;
                        //mc.stream.Close();
                        mc.client.Close();
                        break;
                    default:
                        Console.WriteLine("Error");
                        break;
                }
               
            }

        }
        private void hearingMsg(TcpClient client) {
            
            NetworkStream stream = client.GetStream();
            while (server != null || client != null)
            {
                if (stream.DataAvailable)
                {
                    Byte[] readingData = new Byte[256];
                    String responseData = String.Empty;
                    StringBuilder completeMessage = new StringBuilder();
                    int numberOfBytesRead = 0;
                    do
                    {
                        numberOfBytesRead = stream.Read(readingData, 0, readingData.Length);
                        completeMessage.AppendFormat("{0}", Encoding.UTF8.GetString(readingData, 0, numberOfBytesRead));
                    }
                    while (stream.DataAvailable);
                    responseData = completeMessage.ToString();
                    Console.WriteLine(responseData);
                }
                Thread.Sleep(100);
            }
            stream.Close();
            client.Close();
        }
        //старт в Task
        public void StartServer() {
            try
            {
                server = new TcpListener(ip, PORT);
                //Task task = new Task(hearingMsg);
                server.Start();
                Console.WriteLine("Ожидание подключений... ");
                while (true)
                {
                    client = server.AcceptTcpClient();

                    Console.WriteLine("Подключен клиент. Выполнение запроса...");

                    Task t1 = Task.Run(() => hearingMsg(client));
                    

                    /*// сообщение для отправки клиенту
                    string response = "Привет мир";
                    // преобразуем сообщение в массив байтов
                    byte[] data = Encoding.UTF8.GetBytes(response);

                    // отправка сообщения
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("Отправлено сообщение: {0}", response);
                    // закрываем поток
                    stream.Close();
                    // закрываем подключение
                    client.Close();*/
                    if (server == null) {
                        server.Stop();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            /*finally
            {
                if (server == null)
                    server.Stop();
            }*/


        }
    }
}
