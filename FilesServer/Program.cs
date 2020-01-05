using Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FilesServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Action MyAction = new Action();

            int BufferSize = 0; // перед отправкой файла передаю его размер и записываю тут, чтобы знать какого размера будет буффер на прием файла
            string fileName = string.Empty; // для записи имени файла

            var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 3231);
            listener.Start();
            Console.WriteLine("Сервер запущен");

            while (true)
            {
                //Action MyAction = new Action();
                using (var client = listener.AcceptTcpClient())
                {
                    //Console.WriteLine("Cоединение открыто");
                    using (var stream = client.GetStream())
                    {
                        var resultText = string.Empty;
                        while (stream.DataAvailable)
                        {
                            var buffer = new byte[1024];
                            stream.Read(buffer, 0, buffer.Length);

                            resultText += System.Text.Encoding.UTF8.GetString(buffer);
                        }
                        var myFile = JsonConvert.DeserializeObject<MyFile>(resultText);
                        myFile.FilePathToServer = MyAction.RepositoryPath + myFile.Name;
                        MyAction.MyFiles.Add(myFile);
                        MyAction.AddToDB(MyAction.MyFiles);
                        //BufferSize = int.Parse(myFile.Size);
                        fileName = myFile.Name;
                        //Console.WriteLine(BufferSize);
                    }                   
                }
                //Console.WriteLine("Cоединение закрыто");
                var myFiles = JsonConvert.SerializeObject(MyAction.MyFiles);
                using (var client = listener.AcceptTcpClient())
                {
                    //Console.WriteLine("Cоединение открыто");
                    using (var stream = client.GetStream())
                    {
                        byte[] buffer = new byte[BufferSize];                       
                        stream.Read(buffer, 0, buffer.Length);
                        MyAction.SaveToFileRepository(buffer, fileName);
                        var answerData = System.Text.Encoding.UTF8.GetBytes(myFiles);
                        stream.Write(answerData, 0, answerData.Length);
                    }
                }
            }
        }
    }
}
