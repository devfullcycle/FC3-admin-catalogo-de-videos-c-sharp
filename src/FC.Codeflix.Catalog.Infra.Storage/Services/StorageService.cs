using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Storage.Configuration;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace FC.Codeflix.Catalog.Infra.Storage.Services
{
    public class StorageService : IStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly StorageServiceOptions _options;

        public StorageService(
            StorageClient storageClient,
            IOptions<StorageServiceOptions> options)
        {
            _storageClient = storageClient;
            _options = options.Value;
        }

        public async Task Delete(string filePath, CancellationToken cancellationToken)
        {
            await _storageClient.DeleteObjectAsync(
                _options.BucketName,
                filePath,
                cancellationToken: cancellationToken);
        }

        public async Task<string> Upload(
            string fileName,
            Stream fileStream,
            string contentType,
            CancellationToken cancellationToken)
        {
            await _storageClient.UploadObjectAsync(
                _options.BucketName, fileName, contentType, fileStream,
                cancellationToken: cancellationToken);
            return fileName;
        }
    }
}
