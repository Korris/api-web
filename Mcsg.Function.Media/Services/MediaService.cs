using Azure.Storage.Blobs;
using Mcsg.Function.Media.Constants;
using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Lib.Common.Helpers;

namespace Mcsg.Function.Media.Services
{
    public interface IMediaService
    {
        Task<Stream> ServeVideoAsync(string hashUri);
        Task<Stream> ServeImageAsync(string url);
        Task<Stream> ServePublicImageAsync(string name);
        BlobClient GetClient(string hashUri);
        string GetRealUri(string hashUri);
    }

    public class MediaService : IMediaService
    {
        private readonly IAzureBlobStorageService _blobStorageService;
        private readonly IAzureBlobStorageService _publicBlobStorageService;
        private readonly string _encryptKey;

        public MediaService(IAzureBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
            _publicBlobStorageService = new AzureBlobStorageService(Environment.GetEnvironmentVariable("Function:PublicStorageConnection"));
            _encryptKey = Environment.GetEnvironmentVariable("Function:MediaEncryptKey");
        }

        public async Task<Stream> ServeVideoAsync(string hashUri)
        {
            return await _blobStorageService.ReadStreamAsync(GetRealUri(hashUri), FunctionConstant.MediaContainer,
                new Azure.Storage.Blobs.Models.BlobOpenReadOptions(true) { BufferSize = 16 * 1024 * 1024 });
        }

        public async Task<Stream> ServeImageAsync(string url)
        {
            return await _blobStorageService.DownloadAsync(url, FunctionConstant.MediaContainer);
        }

        public async Task<Stream> ServePublicImageAsync(string name)
        {
            return await _publicBlobStorageService.DownloadAsync(name, FunctionConstant.PublicImageContainer);
        }
        public BlobClient GetClient(string hashUri)
        {
            return _blobStorageService.GetBlobClient(GetRealUri(hashUri), FunctionConstant.MediaContainer);
        }
        public string GetRealUri(string hashUri)
        {
            return CryptoHelper.Decrypt(hashUri, _encryptKey);
        }
    }
}
