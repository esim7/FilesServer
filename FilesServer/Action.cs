using DataAccess;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilesServer
{
    public class Action
    {
        public List<MyFile> MyFiles { get; set; }
        public Context FileContext;
        public string RepositoryPath = @"C://FilesRepository";

        public Action()
        {
            MyFiles = new List<MyFile>();
            FileContext = new Context();
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
        public void SaveToFileRepository()
        {

        }
    }
}
