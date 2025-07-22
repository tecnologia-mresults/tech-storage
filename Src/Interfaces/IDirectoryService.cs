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
        Task<T> CreateAsync(string diretoryName);

        /// <summary>
        /// Deleta um diretorio
        /// </summary>
        /// <param name="diretoryName">Nome do diretorio</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string diretoryName);

        /// <summary>
        /// Pega um diretorio pelo nome
        /// </summary>
        /// <param name="diretoryName">Nome do arquivador</param>
        /// <returns></returns>
        Task<T> GetAsync(string diretoryName);

        /// <summary>
        /// Lista todos os diretórios
        /// </summary>
        /// <param name="dirName">Nome do diretório</param>
        /// <returns></returns>
        Task<IEnumerable<T>> ListAsync( string dirName);
    }
}
