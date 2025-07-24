using System;

namespace MR.Tech.Storage.Src.Services.S3.Minio.Exceções
{
    public class MinioStorageException : Exception
    {
        public MinioStorageException(string message): base(message)  
        { }
    }
}
