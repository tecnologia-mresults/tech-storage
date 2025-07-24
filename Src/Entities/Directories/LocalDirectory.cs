using MR.Tech.Storage.Src.Entities.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MR.Tech.Storage.Src.Entities.Directories
{
    public class LocalDirectory : AbstractDirectory
    {
        public bool IsRoot { get; set; }
        public bool HasSubDirectories { get; set; }
        public long Size {  get; set; }
        public IEnumerable<LocalDirectory> SubDirectories { get; set; }

        public LocalDirectory(bool isRoot, bool hasSubDirectories, string name, string path, DateTime creation)
        {
            IsRoot = isRoot;
            HasSubDirectories = hasSubDirectories;
            Name = name;
            Path = path;
            Creation = creation;
            Size = GetSizeOfLocalDirectory();
            SubDirectories = GetSubDirectories();

        }

        private long GetSizeOfLocalDirectory()
        {
            long size = 0;

           
            foreach (string file in Directory.GetFiles(Path, "*", SearchOption.AllDirectories))
            {
                FileInfo info = new FileInfo(file);
                size += info.Length;
            }

            return size;
        }

        private IEnumerable<LocalDirectory> GetSubDirectories()
        {
            List<LocalDirectory> mappedDirectories = new List<LocalDirectory>();
             
            foreach (var item in new DirectoryInfo(Path).GetDirectories())
            {
                bool hasSub = item.GetDirectories().Count() == 0 ? false : true;

                LocalDirectory localDirectory = new LocalDirectory(false, hasSub, item.Name, item.FullName, item.CreationTime);

                mappedDirectories.Add(localDirectory);
            }

            return mappedDirectories;
        }
    }
}
