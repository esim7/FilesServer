using System;

namespace Domain
{
    public class MyFile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string FilePathToServer { get; set; }
    }
}
