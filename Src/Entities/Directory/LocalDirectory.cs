using MR.Tech.Storage.Src.Entities.Abstractions;
using System;

namespace MR.Tech.Storage.Src.Entities.Archiver
{
    public class LocalDirectory : AbstractDirectory
    {
        public bool IsRoot { get; set; }
        public bool HasSubDirectories { get; set; }
        public float Size {  get; set; }

        public LocalDirectory(float size, bool isRoot, bool hasSubDirectories, string name, string path, DateTime creation)
        {
            Size = size;
            IsRoot = isRoot;
            HasSubDirectories = hasSubDirectories;
            Name = name;
            Path = path;
            Creation = creation;

        }
    }
}
