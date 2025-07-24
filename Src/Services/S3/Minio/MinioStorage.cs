using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel;
using Minio.DataModel.Args;
using MR.Tech.Storage.Src.Entities.Directories;
using MR.Tech.Storage.Src.Entities.File;
using MR.Tech.Storage.Src.Interfaces;
using MR.Tech.Storage.Src.Services.S3.Minio.Exceções;
using MR.Tech.Storage.Src.Services.S3.Minio.Exceções.Enum;
using MR.Tech.Storage.Src.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


namespace MR.Tech.Storage.Src.Services.S3.Minio
{
    public class MinioStorage : IDirectoryService<BucketDiretory>, IFileService<ObjectFile>
    {
        private IMinioClient _minioClient;
        private string _endpoint;

        public void Configure(string endpoint, string accessKey, string secretKey, bool hasSsl)
        {

                _minioClient = new MinioClient()
                 .WithCredentials(accessKey, secretKey)
                 .WithEndpoint(endpoint)
                 .WithSSL(hasSsl)
                 .Build();

            _endpoint = endpoint;
        }

        public async Task<BucketDiretory> CreateAsync(string bucketName)
        {
            if(bucketName == string.Empty)
            {
                throw new ArgumentNullException(nameof(bucketName).ToUpper());
            }

            var args = new BucketExistsArgs()
              .WithBucket(bucketName.ToLower());

            bool found = await _minioClient.BucketExistsAsync(args).ConfigureAwait(false);
            
            if(found == true)
            {
                throw new MinioStorageException($"{ nameof(MinioStorageExceptionEnum.Bucket)}: Já existe um bucket com o nome {bucketName}");
            }
            
            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs()
                   .WithBucket(bucketName)
                  .WithLocation("")
            ).ConfigureAwait(false);
            
            string path = _endpoint + "/" + bucketName;
                BucketDiretory bucketDiretory = new BucketDiretory(bucketName,path,DateTime.Now);

            return bucketDiretory;

        }

        public async Task<bool> DeleteAsync(string bucketName)
        {
            if (bucketName == string.Empty)
            {
                throw new ArgumentNullException(nameof(bucketName).ToUpper());
            }

            var args = new BucketExistsArgs()
              .WithBucket(bucketName);

            bool found = await _minioClient.BucketExistsAsync(args).ConfigureAwait(false);

            if (found == false)
            {
                throw new MinioStorageException($"{nameof(MinioStorageExceptionEnum.Bucket)}: Não existe um bucket com o nome {bucketName}");
            }

            await _minioClient.RemoveBucketAsync(
                  new RemoveBucketArgs()
                      .WithBucket(bucketName)
              ).ConfigureAwait(false);

            return true;

        }

        public async Task<IEnumerable<BucketDiretory>> ListAsync(string bucketName ="")
        {
            var list = await _minioClient.ListBucketsAsync().ConfigureAwait(false);
            
            List<BucketDiretory> listOfBuckets = new List<BucketDiretory>();

            foreach (var bucket in list.Buckets)
            {
                string path = _endpoint +"/"+bucket.Name;

                BucketDiretory bucketDiretory = new BucketDiretory(bucket.Name,path,DateTime.Parse(bucket.CreationDate));

                listOfBuckets.Add(bucketDiretory);
            }

            return listOfBuckets;
        }

        public async Task<BucketDiretory> GetAsync(string bucketName)
        {
            if (bucketName == string.Empty)
            {
                throw new ArgumentNullException(nameof(bucketName).ToUpper());
            }

            var args = new BucketExistsArgs()
             .WithBucket(bucketName);

            bool found = await _minioClient.BucketExistsAsync(args).ConfigureAwait(false);

            if (found == false)
            {
                throw new MinioStorageException($"{nameof(MinioStorageExceptionEnum.Bucket)}: Não existe um bucket com o nome {bucketName}");
            }


            var list = await _minioClient.ListBucketsAsync().ConfigureAwait(false);

            List<BucketDiretory> listOfBuckets = new List<BucketDiretory>();

            Bucket bucket = list.Buckets.Where(x => x.Name == bucketName).FirstOrDefault();

            string path = _endpoint + "/" + bucket.Name;

            BucketDiretory bucketDiretory = new BucketDiretory(bucket.Name, path, DateTime.Parse(bucket.CreationDate));

            return bucketDiretory;
        }

        public async Task<ObjectFile> GetAsync(string objectName, string bucketName)
        {
            if (bucketName == string.Empty)
            {
                throw new ArgumentNullException(nameof(bucketName).ToUpper());
            }

            var args = new BucketExistsArgs()
             .WithBucket(bucketName);

            bool found = await _minioClient.BucketExistsAsync(args).ConfigureAwait(false);

            if (found == false)
            {
                throw new MinioStorageException($"{nameof(MinioStorageExceptionEnum.Bucket)}: Não existe um bucket com o nome {bucketName}.");
            }

            if(objectName == string.Empty)
            {
                throw new ArgumentNullException(nameof(objectName).ToUpper());
            }

            var memoryStream = new MemoryStream();

            var argsToFindObject = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream));

            var stat = await _minioClient.GetObjectAsync(argsToFindObject).ConfigureAwait(false);

            if(stat == null)
            {
                throw new MinioStorageException($"{nameof(MinioStorageExceptionEnum.Object)}: Documento informado não existe.");
            }

            byte[] bytes = memoryStream.ToArray();

            ObjectFile objectFile = new ObjectFile(stat.ObjectName,stat.Size,stat.LastModified,stat.ETag,false,false,stat.ContentType,bytes,stat.LastModified);
            
           return objectFile; 
        }

        public async Task<ObjectFile> UploadAsync(string objectName, byte[] objectBytes, string bucketName)
        {
            if(objectName == string.Empty)
            {
                throw new ArgumentException(nameof(objectName).ToUpper());
            }

            if(objectBytes.Length == 0)
            {
                throw new ArgumentException(nameof(objectBytes).ToUpper());
            }

            if (bucketName == string.Empty)
            {
                throw new ArgumentException(nameof(bucketName).ToUpper());
            }

            string contentType = MimeHelper.MimeFinder(objectBytes);

            var stream = new MemoryStream(objectBytes);
            long size = objectBytes.Length;

            await _minioClient.PutObjectAsync(new PutObjectArgs()
                   .WithBucket(bucketName)
                   .WithObject(objectName)
                   .WithStreamData(stream)
                   .WithObjectSize(size)
                   .WithContentType(contentType));

            ObjectFile objectFile = await GetAsync(objectName,bucketName);

            return objectFile;
        }

        public async Task<bool> DeleteAsync(string bucketName, string objectName)
        {
            if (objectName == string.Empty)
            {
                throw new ArgumentException(nameof(objectName).ToUpper());
            }

            if (bucketName == string.Empty)
            {
                throw new ArgumentException(nameof(bucketName).ToUpper());
            }

            var args = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);

            await _minioClient.RemoveObjectAsync(args).ConfigureAwait(false);

            return true;
        }

        public async Task<IEnumerable<ObjectFile>> ListAllAsync(string bucketName)
        {
            if (string.IsNullOrWhiteSpace(bucketName))
            {
                throw new ArgumentException(nameof(bucketName).ToUpper());
            }

            List<Item> itens = new List<Item>();
            var tcs = new TaskCompletionSource<List<Item>>();

            // Usando IObservable<Item> com Subscribe
            var observable = _minioClient.ListObjectsAsync(
                new ListObjectsArgs()
                    .WithBucket(bucketName)
                    .WithRecursive(true)
            );

            // Subscrição para acumular os itens
            observable.Subscribe(
                item =>
                {
                    if (!item.IsDir)
                        itens.Add(item);
                },
                ex => tcs.TrySetException(ex),
                () => tcs.TrySetResult(itens)
            );

            // Aguarda toda a listagem terminar
            itens = await tcs.Task;

            // Cria os objetos com os bytes
            List<ObjectFile> objectFiles = new List<ObjectFile>();

            foreach (var item in itens)
            {
              var memoryStream = new MemoryStream();

                await _minioClient.GetObjectAsync(new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(item.Key)
                    .WithCallbackStream(stream => stream.CopyTo(memoryStream)));

                byte[] bytes = memoryStream.ToArray();

                ObjectFile objectFile = new ObjectFile(
                    item.Key,
                    long.Parse(item.Size.ToString()),
                    DateTime.Parse(item.LastModified),
                    item.ETag,
                    item.IsLatest,
                    false, // Mude conforme necessário
                    item.ContentType,
                    bytes,
                    DateTime.Parse(item.LastModified) // Pode ser outro campo se necessário
                );

                objectFiles.Add(objectFile);
            }

            return objectFiles;
        }


    }
}
