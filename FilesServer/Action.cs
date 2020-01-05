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
        TcpListener Listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 3231);

        public Action()
        {
            MyFiles = new List<MyFile>();
            FileContext = new Context();
            using (var context = new Context())
            {
                MyFiles = FileContext.MyFiles.ToList();
            }
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
    }
}
