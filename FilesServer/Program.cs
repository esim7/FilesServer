using Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FilesServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Action MyAction = new Action();
            string ServerAction;

            //int BufferSize = 0; // перед отправкой файла передаю его размер и записываю тут, чтобы знать какого размера будет буффер на прием файла
            //string fileName = string.Empty; // для записи имени файла

            //var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 3231);
            //listener.Start();
            Console.WriteLine("Сервер запущен");
            MyAction.GetDataToFirstConnection(); // Передача актуального списка файлов на файловом сервере при первом подключении клиента
            while (true)
            {
                using (var client = MyAction.Listener.AcceptTcpClient())
                {
                    using (var stream = client.GetStream())
                    {
                        ServerAction = string.Empty;
                        while (stream.DataAvailable)
                        {
                            var buffer = new byte[128];
                            stream.Read(buffer, 0, buffer.Length);
                            ServerAction += System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                        }
                    }
                }
                if (ServerAction == "recive")
                {
                    MyAction.ReciveFiles();
                }
                else if (ServerAction == "send")
                {
                    MyAction.SendFiles();
                }
                else if (ServerAction == "delete")
                {

                }
            }
        }
    }
}
