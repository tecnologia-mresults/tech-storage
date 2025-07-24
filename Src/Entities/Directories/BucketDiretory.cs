using MR.Tech.Storage.Src.Entities.Abstractions;
using System;

namespace MR.Tech.Storage.Src.Entities.Directories
{
    public class BucketDiretory: AbstractDirectory
    {
        
        public BucketDiretory(string name, string path, DateTime creationDate) 
        { 
            Name = name;
            Path = path;
            Creation = creationDate;
        }
    }
}
