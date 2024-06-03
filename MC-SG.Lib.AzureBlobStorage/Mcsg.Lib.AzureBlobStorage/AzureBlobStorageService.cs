using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Mcsg.Lib.AzureBlobStorage.Settings;
using Microsoft.Extensions.Options;

namespace Mcsg.Lib.AzureBlobStorage
{
    public interface IAzureBlobStorageService
    {
        BlobContainerClient GetBlobContainerClient(string containerName);
        BlobClient GetBlobClient(string blobName, string containerName);
        Task<Uri> UploadAsync(string blobName, Stream content, string containerName);
        Task<Uri> FastUploadAsync(string blobName, Stream content, string containerName);
        Task<Stream> DownloadAsync(string blobName, string containerName);
        Task DownloadAsync(string blobName, string containerName, string toPath);
        Task<string> DownloadBlobToStringAsync(string blobName, string containerName);
        Task<Stream> ReadStreamAsync(string blobName, string containerName, BlobOpenReadOptions options = null);
        Task DeleteAsync(string blobName, string containerName);
        Task<bool> IsExistAsync(string blobName, string containerName);

        Task<Uri> CreateShareUrl(string blobName, string containerName, int expiredYear = 20);
    }

    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly AzureBlobStorageSettings _settings;
        private readonly string _connectionString;

        public AzureBlobStorageService(IOptions<AzureBlobStorageSettings> settings)
        {
            _settings = settings.Value;
        }

        public AzureBlobStorageService(AzureBlobStorageSettings settings)
        {
            _settings = settings;
        }

        public AzureBlobStorageService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BlobContainerClient GetBlobContainerClient(string containerName)
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                return new BlobContainerClient(_connectionString, containerName);
            }
            else
            {
                return new BlobContainerClient(string.Format(LibConstant.CONNECTION_STRING, _settings.StorageName, _settings.AccountKey),
                    containerName);
            }
        }

        public BlobClient GetBlobClient(string blobName, string containerName)
        {
            var client = GetBlobContainerClient(containerName);
            return client.GetBlobClient(blobName);
        }

        public async Task<Uri> UploadAsync(string blobName, Stream content, string containerName)
        {
            var client = GetBlobContainerClient(containerName);
            await client.UploadBlobAsync(blobName, content);
            return client.Uri;
        }

        public async Task<Uri> FastUploadAsync(string blobName, Stream content, string containerName)
        {
            var client = GetBlobClient(blobName, containerName);
            // Specify the StorageTransferOptions
            BlobUploadOptions options = new()
            {

                TransferOptions = new StorageTransferOptions
                {
                    // Set the maximum number of parallel transfer workers
                    MaximumConcurrency = Environment.ProcessorCount * 2 * 4,

                    // Set the initial transfer length to 8 MiB
                    InitialTransferSize = 8 * 1024 * 1024,

                    // Set the maximum length of a transfer to 4 MiB
                    MaximumTransferSize = 4 * 1024 * 1024
                }
            };


            await client.UploadAsync(content, options);
            return client.Uri;
        }

        public async Task<Stream> DownloadAsync(string blobName, string containerName)
        {
            var client = GetBlobClient(blobName, containerName);
            var isExist = await client.ExistsAsync();

            if (isExist)
            {
                var response = await client.DownloadStreamingAsync();
                return response.Value.Content;
            }

            throw new Exception("Blob dose not exist!.");
        }

        public async Task DownloadAsync(string blobName, string containerName, string toPath)
        {
            var client = GetBlobClient(blobName, containerName);
            var isExist = await client.ExistsAsync();

            if (isExist)
            {
                await client.DownloadToAsync(toPath);
            }
            else
            {
                throw new Exception("Blob does not exist!.");
            }
        }

        public async Task<string> DownloadBlobToStringAsync(string blobName, string containerName)
        {
            var client = GetBlobClient(blobName, containerName);
            var isExist = await client.ExistsAsync();

            if (isExist)
            {
                BlobDownloadResult downloadResult = await client.DownloadContentAsync();
                return downloadResult.Content.ToString();
            }
            else
            {
                throw new Exception("Blob does not exist!.");
            }
        }

        public async Task DeleteAsync(string blobName, string containerName)
        {
            var client = GetBlobClient(blobName, containerName);
            var isExist = await client.ExistsAsync();

            if (isExist)
            {
                await client.DeleteAsync();
            }
            else
            {
                throw new Exception("Blob does not exist!.");
            }
        }

        public async Task<bool> IsExistAsync(string blobName, string containerName)
        {
            var client = GetBlobClient(blobName, containerName);
            return await client.ExistsAsync();
        }

        public async Task<Stream> ReadStreamAsync(string blobName, string containerName, BlobOpenReadOptions options = null)
        {
            var client = GetBlobClient(blobName, containerName);
            if (options == null)
                return await client.OpenReadAsync();
            else
                return await client.OpenReadAsync(options);
        }

        public async Task<Uri> CreateShareUrl(string blobName, string containerName, int expiredYear = 30)
        {
            var client = GetBlobClient(blobName, containerName);
            var isExist = await client.ExistsAsync();
            if (isExist)
            {
                return client.GenerateSasUri(Azure.Storage.Sas.BlobSasPermissions.Read, DateTime.UtcNow.AddYears(expiredYear));
            }
            return new Uri(string.Empty);
        }
    }
}
