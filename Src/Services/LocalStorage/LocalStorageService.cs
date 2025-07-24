using MR.Tech.Storage.Src.Entities.Directories;
using MR.Tech.Storage.Src.Entities.File;
using MR.Tech.Storage.Src.Interfaces;
using MR.Tech.Storage.Src.Services.LocalStorage.Exceptions;
using MR.Tech.Storage.Src.Shared.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MR.Tech.Storage.Src.Services.LocalStorage
{
    public class LocalStorageService : IDirectoryService<LocalDirectory>, IFileService<LocalFile>
    {
        private DriveInfo _driveInfo;
        public void Configure(string volumeName)
        {
            DriveInfo driveInfo = new DriveInfo(volumeName);
            if (driveInfo.IsReady == false )
            {
                throw new LocalStorageException($"{nameof(LocalStorageExceptionEnum.Volume)}: Volume inválido. O volume informado não está disponivel ou não existe.");
            }
            _driveInfo = driveInfo;
        }

        public async Task<LocalDirectory> CreateAsync(string directoryName)
        {
            string normalizedPathToFindAnDirectory = Path.Combine(_driveInfo.Name, directoryName);

            if (Directory.Exists(normalizedPathToFindAnDirectory) == true)
            {
                throw new LocalStorageException($"{nameof(LocalStorageExceptionEnum.Directory)}: Diretório com o nome informado já existe.");
            }

            LocalDirectory createdDir = await Task.Run(() =>
           {
               Directory.CreateDirectory(normalizedPathToFindAnDirectory);

                   DirectoryInfo createdDirectory = new DirectoryInfo(normalizedPathToFindAnDirectory);

                   bool isRoot = createdDirectory.Parent.FullName == _driveInfo.Name ? true : false;

                   bool hasSubDirectories = createdDirectory.GetDirectories().Count() == 0 ? true : false;

                   LocalDirectory localDirectory = new LocalDirectory(isRoot, hasSubDirectories, createdDirectory.Name, createdDirectory.FullName, createdDirectory.CreationTime);

                   return localDirectory;
               

           });

            return createdDir;
        }

        public async Task<LocalDirectory> GetAsync(string dirName)
        {
            string pathToDirectory = Path.Combine(_driveInfo.Name, dirName);
            
            if (Directory.Exists(pathToDirectory) == false)
            {
                throw new LocalStorageException($"{nameof(LocalStorageExceptionEnum.Directory)}: Diretório com o nome informado não existe.");
            }

            DirectoryInfo directory = new DirectoryInfo(pathToDirectory);

            bool isRoot = directory.Parent.FullName == _driveInfo.Name ? true : false;

            bool hasSubDirectories = directory.GetDirectories().Count() == 0 ? false : true;

            LocalDirectory localDirectory = new LocalDirectory(isRoot, hasSubDirectories, directory.Name, directory.FullName, directory.CreationTime);

            return localDirectory;

        }

        public async Task<bool> DeleteAsync(string dirName)
        {
            string pathToDirectory = Path.Combine(_driveInfo.Name, dirName);

            if (Directory.Exists(pathToDirectory) == false)
            {
                throw new LocalStorageException($"{nameof(LocalStorageExceptionEnum.Directory)}: Diretório com o nome informado não existe.");
            }

            Directory.Delete(pathToDirectory, true);

            bool wasntDeleted = Directory.Exists(pathToDirectory);
            
            if (wasntDeleted == true) {
                
                throw new LocalStorageException($"{nameof(LocalStorageExceptionEnum.Directory)}: Falha ao apagar diretório.");
            }

            return wasntDeleted;

        }

        public async Task<IEnumerable<LocalDirectory>> ListAsync(string dirName)
        {
            List<LocalDirectory> directories = new List<LocalDirectory>();
            string pathToDirectory = Path.Combine(_driveInfo.Name, dirName);


            foreach (var directory in new DirectoryInfo(pathToDirectory).GetDirectories())
            {
                bool isRoot = directory.Parent.FullName == _driveInfo.Name ? true : false;

                bool hasSubDirectories = directory.GetDirectories().Count() == 0 ? false : true;

                LocalDirectory localDirectory = new LocalDirectory(isRoot, hasSubDirectories, directory.Name, directory.FullName, directory.CreationTime);

                directories.Add(localDirectory);
            }

            return directories;
        }

        public async Task<LocalFile> UploadAsync(string fileName, byte[] fileBytes, string directory)
        {
            string pathToFile = Path.Combine(_driveInfo.Name,directory, fileName);

            if(Directory.Exists(Path.Combine(_driveInfo.Name, directory)) == false)
            {
                throw new LocalStorageException($"{nameof(LocalStorageExceptionEnum.Directory)}: Diretório {Path.Combine(_driveInfo.Name, directory)} não existe.");
            }


            if (File.Exists(pathToFile) == true)
            {
                throw new LocalStorageException($"{nameof(LocalStorageExceptionEnum.File)}: Já existe um arquivo com esse nome.");
            }

            File.WriteAllBytes(pathToFile,fileBytes);
            
            if(File.Exists(pathToFile) == false)
            {
                throw new LocalStorageException($"{nameof(LocalStorageExceptionEnum.File)}: Falha ao criar arquivo {fileName} no diretório {directory}.");
            }

            FileInfo createdFile = new FileInfo(pathToFile);
            string mime = MimeHelper.MimeFinder(fileBytes);

            LocalFile file = await Task.Run(() =>
            {
                LocalFile localFile = new LocalFile(createdFile.FullName,createdFile.Name,fileBytes,createdFile.Length,mime,createdFile.LastWriteTime);

                return localFile;
            });

            return file;

        }

        public async Task<bool> DeleteAsync(string fileName, string directory)
        {
            string pathToFile = Path.Combine(_driveInfo.Name, directory, fileName);

            if (Directory.Exists(Path.Combine(_driveInfo.Name, directory)) == false)
            {
                throw new LocalStorageException($"{nameof(LocalStorageExceptionEnum.Directory)}: Diretório {Path.Combine(_driveInfo.Name, directory)} não existe.");
            }

            if (File.Exists(pathToFile) == false)
            {
                throw new LocalStorageException($"{nameof(LocalStorageExceptionEnum.File)}: Arquivo {pathToFile} não existe.");
            }

            await Task.Run(() =>
            {
              File.Delete(pathToFile);
            });
            

            return true;
        }

        public async Task<LocalFile> GetAsync(string fileName, string directory)
        {
            if (Directory.Exists(Path.Combine(_driveInfo.Name, directory)) == false)
            {
                throw new LocalStorageException($"{nameof(LocalStorageExceptionEnum.Directory)}: Diretório {Path.Combine(_driveInfo.Name, directory)} não existe.");
            }

            if(File.Exists(Path.Combine(_driveInfo.Name, directory,fileName)) == false)
            {
                throw new LocalStorageException($"{nameof(LocalStorageExceptionEnum.File)}: Arquivo {Path.Combine(_driveInfo.Name, directory, fileName)} não existe.");
            }

            LocalFile file = await Task.Run( () =>
            {
                FileInfo createdFile = new FileInfo(Path.Combine(_driveInfo.Name, directory, fileName));
                byte[] fileBytes = File.ReadAllBytes(Path.Combine(_driveInfo.Name, directory, fileName));
                
                string mime = MimeHelper.MimeFinder(fileBytes);

                LocalFile localFile = new LocalFile(createdFile.FullName, createdFile.Name, fileBytes, createdFile.Length, mime, createdFile.LastWriteTime);

                return localFile;

            });

            return file;

        }

        public async Task<IEnumerable<LocalFile>> ListAllAsync(string directory)
        {
            if (Directory.Exists(Path.Combine(_driveInfo.Name, directory)) == false)
            {
                throw new LocalStorageException($"{nameof(LocalStorageExceptionEnum.Directory)}: Diretório {Path.Combine(_driveInfo.Name, directory)} não existe.");
            }

            var listOfFile = await Task.Run(() =>
            {
              List<LocalFile> files = new List<LocalFile>();

              DirectoryInfo directoryInf = new DirectoryInfo(Path.Combine(_driveInfo.Name, directory));

              foreach(var createdFile in directoryInf.GetFiles())
              {

                byte[] fileBytes = File.ReadAllBytes(createdFile.FullName);

                string mime = MimeHelper.MimeFinder(fileBytes);

                LocalFile localFile = new LocalFile(createdFile.FullName, createdFile.Name, fileBytes, createdFile.Length, mime, createdFile.LastWriteTime);

                files.Add(localFile);
              }
           
              return files;

            });

          return listOfFile;
        }
    }

}



