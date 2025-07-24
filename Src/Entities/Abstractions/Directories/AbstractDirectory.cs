
using System;

namespace MR.Tech.Storage.Src.Entities.Abstractions
{
    /// <summary>
    /// Onde será guardado os arquivos
    /// </summary>
    public class AbstractDirectory
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime Creation {  get; set; }
    }
}
