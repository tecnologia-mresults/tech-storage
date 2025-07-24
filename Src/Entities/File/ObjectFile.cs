using MR.Tech.Storage.Src.Entities.Abstractions;
using System;

namespace MR.Tech.Storage.Src.Entities.File
{
    public class ObjectFile : AbstractFile
    {
        public string ObjectName { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string ETag { get; set; }
        public bool IsLastest { get; set; }
        public bool IsDir { get; set; }

        public ObjectFile(string objectName, long size, DateTime lastModified, string eTag, bool isLastest, bool isDir, string mime, byte[] fileBytes, DateTime creation)
        {
            ObjectName = objectName;
            Size = size;
            LastModified = lastModified;
            ETag = eTag;
            IsLastest = isLastest;
            IsDir = isDir;
            Name = objectName;
            Mime = mime;
            FileBytes = fileBytes;
            Creation = creation;

        }
    }
}
