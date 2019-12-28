using Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace FilesServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Action MyAction = new Action();
            int BufferSize = 0;

            var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 3231);
            listener.Start();
            Console.WriteLine("Сервер запущен");

            while (true)
            {
                using (var client = listener.AcceptTcpClient())
                {
                    Console.WriteLine("Входящее соединение");
                    using (var stream = client.GetStream())
                    {
                        var resultText = string.Empty;
                        while (stream.DataAvailable)
                        {
                            var buffer = new byte[1024];
                            stream.Read(buffer, 0, buffer.Length);

                            resultText += System.Text.Encoding.UTF8.GetString(buffer);
                        }
                        Console.WriteLine($"Данные от клиента - {resultText}");
                        var myFiles = JsonConvert.DeserializeObject<List<MyFile>>(resultText);
                        MyAction.MyFiles = myFiles;
                        MyAction.AddToDB(MyAction.MyFiles);
                    }                   
                }
                Console.WriteLine("Cоединение закрыто");
                using (var client = listener.AcceptTcpClient())
                {
                    Console.WriteLine("Входящее соединение");
                    using (var stream = client.GetStream())
                    {
                        byte[] recv = new Byte[256];                       
                        int bytes = stream.Read(recv, 0, recv.Length);
                        byte[] a = new byte[bytes];
                        for (int i = 0; i < bytes; i++)
                        {
                            a[i] = recv[i];
                        }
                        byte[] b = a;
                    }
                }
                Console.WriteLine("Cоединение закрыто");
            }
        }
    }
}
