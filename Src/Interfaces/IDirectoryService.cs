using System.Collections.Generic;
using System.Threading.Tasks;

namespace MR.Tech.Storage.Src.Interfaces
{
    public interface IDirectoryService<T>
    {
        /// <summary>
        /// Cria um diretorio
        /// </summary>
        /// <param name="diretoryName">Nome do diretorio</param>
        /// <returns></returns>
        Task<T> CreateDirectoryAsync(string diretoryName);

        /// <summary>
        /// Deleta um diretorio
        /// </summary>
        /// <param name="diretoryName">Nome do diretorio</param>
        /// <returns></returns>
        Task<T> DeleteDirectoryAsync(string diretoryName);

        /// <summary>
        /// Pega um diretorio pelo nome
        /// </summary>
        /// <param name="diretoryName">Nome do arquivador</param>
        /// <returns></returns>
        Task<T> GetDirectoryAsync(string diretoryName);

        /// <summary>
        /// Lista todos os diretórios
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> ListDirectory();
    }
}
