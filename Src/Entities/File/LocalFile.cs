
using MR.Tech.Storage.Src.Entities.Abstractions;
using System;

namespace MR.Tech.Storage.Src.Entities.File
{
     public class LocalFile:AbstractFile
    {
        public string Path { get; set; }
        public float Size { get; set; }

        public LocalFile(string path, string name, byte[] fileBytes, float size, string mime, DateTime creation)
        {
            Path = path;
            Size = size;
            FileBytes = fileBytes;
            Name = name;
            Mime = mime;
            Creation = creation;
        }


    }
}
