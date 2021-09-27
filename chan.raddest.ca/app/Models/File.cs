using System;
using System.IO;

namespace app.Models
{
    public class File
    {
        public ulong Id {get; set;}
        public DateTime Created {get; set;}
        public byte[] Content {get; set;}

        public string FileName {get; set;}

        public string FileType {
            get
            {
                return Path.GetExtension(FileName);
            }
        }
    }
}