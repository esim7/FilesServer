using DataAccess;
using Domain;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FilesServer
{
    public class Action
    {
        public List<MyFile> MyFiles { get; set; }
        public Context FileContext;
        public string RepositoryPath = @"C:\FilesRepository\";
        public TcpListener Listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 3231);

        int BufferSize = 0;
        string fileName = string.Empty; // для записи имени файла

        public Action()
        {          
            MyFiles = new List<MyFile>();
            FileContext = new Context();
            using (var context = new Context())
            {
                MyFiles = FileContext.MyFiles.ToList();
            }
            Listener.Start();
        }

        public void AddToDB(List<MyFile> files)
        {
            using (var context = new Context())
            {
                foreach (var file in files)
                {
                    var item = context.MyFiles.AsNoTracking().Any(x => x.Id == file.Id);
                    if (item)
                    {
                        context.Update(file);
                        context.SaveChanges();
                        continue;
                    }
                    context.Add(file);
                    context.SaveChanges();
                }
            }
        }
        public void SaveToFileRepository(byte[] data, string fileName)
        {
            using (var fileStream = new FileStream(RepositoryPath + fileName, FileMode.Create, FileAccess.Write))
            {
                fileStream.Write(data, 0, data.Length);
            }
        }

        public void GetDataToFirstConnection()
        {
            using (var client = Listener.AcceptTcpClient())
            {
                using (var stream = client.GetStream())
                {
                    var AllFilesToRepository = JsonConvert.SerializeObject(MyFiles);
                    var data = System.Text.Encoding.UTF8.GetBytes(AllFilesToRepository);
                    stream.Write(data, 0, data.Length);
                }
            }
        }

        public void ReciveFiles()
        {
            using (var client = Listener.AcceptTcpClient())
            {
                Console.WriteLine("Cоединение открыто");
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
                    myFile.FilePathToServer = RepositoryPath + myFile.Name;
                    MyFiles.Add(myFile);
                    AddToDB(MyFiles);
                    BufferSize = int.Parse(myFile.Size);
                    fileName = myFile.Name;
                    Console.WriteLine(BufferSize);
                }
            }
            Console.WriteLine("Cоединение закрыто");
            var myFiles = JsonConvert.SerializeObject(MyFiles);
            using (var client = Listener.AcceptTcpClient())
            {
                Console.WriteLine("Cоединение открыто");
                using (var stream = client.GetStream())
                {
                    byte[] buffer = new byte[BufferSize];
                    stream.Read(buffer, 0, buffer.Length);
                    SaveToFileRepository(buffer, fileName);

                    var answerData = System.Text.Encoding.UTF8.GetBytes(myFiles);
                    stream.Write(answerData, 0, answerData.Length);
                }
            }
        }

        public void SendFiles()
        {
            using (var client = Listener.AcceptTcpClient())
            {
                Console.WriteLine("Cоединение открыто");
                using (var stream = client.GetStream())
                {
                    var resultText = string.Empty;
                    while (stream.DataAvailable)
                    {
                        var buffer = new byte[1024];
                        stream.Read(buffer, 0, buffer.Length);
                        resultText += System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                    }
                    Guid id = Guid.Parse(resultText);
                    var myFile = FileContext.MyFiles.FirstOrDefault(x => x.Id == id);
                    var FileData = File.ReadAllBytes(myFile.FilePathToServer);


                    stream.Write(FileData, 0, FileData.Length);
                }
            }
        }
    }
}
