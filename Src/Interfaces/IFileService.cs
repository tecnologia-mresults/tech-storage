
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MR.Tech.Storage.Src.Interfaces
{
    public interface IFileService<T>
    {
        /// <summary>
        /// Faz upload de um arquivo
        /// </summary>
        /// <param name="fileName">Nome do arquivo</param>
        /// <param name="fileBytes">Bytes do arquivo</param>
        /// <param name="format">Formato do arquivo</param>
        /// <param name="directory">Diretorio destino do arquivo</param>
        /// <returns></returns>
        Task<T> UploadAsync(string fileName, byte[] fileBytes,string directory);

        /// <summary>
        /// Deleta um arquivo        
        /// </summary>
        /// <param name="fileName">Nome do arquivo</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string fileName, string dir);

        /// <summary>
        /// Pega um arquivo pelo nome
        /// </summary>
        /// <param name="fileName">Nome do arquivador</param>
        /// <returns></returns>
        Task<T> GetAsync(string fileName, string dirName);

        /// <summary>
        /// Lista todos os arquivos
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> ListAllAsync(string diretoryName);
    }
}
