

using System;

namespace MR.Tech.Storage.Src.Entities.Abstractions
{
    /// <summary>
    /// Abstração padão de um arquivo
    /// </summary>
    public class AbstractFile
    {
        public string Name { get; set; }
        public byte[] FileBytes { get; set; }
        public string Format { get; set; }
        public DateTime Creation {  get; set; }
    }
}
