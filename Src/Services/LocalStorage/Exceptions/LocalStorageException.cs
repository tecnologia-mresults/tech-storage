

using System;

namespace MR.Tech.Storage.Src.Services.LocalStorage.Exceptions
{
    public class LocalStorageException: Exception
    {
        public LocalStorageException(string message) : base(message) { }
    }
}
