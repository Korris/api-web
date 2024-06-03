using Azure.Storage.Blobs;
using Dapper;
using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Extensions;
using Mcsg.Lib.Common.Helpers;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Mcsg.Realtime.Api.DTOs;

namespace Mcsg.Realtime.Api.Services
{
    public interface IResourceCommentService
    {
        Task<ResourceCommentResp> AddResourceToComment(string userName, string hashId, ResourceLocationType locationType);
    }
    public partial class ResourceCommentService : IResourceCommentService
    {
        private readonly IRepository<Resource> _resourceRepository;
        private readonly IAzureBlobStorageService _blobStorageService;
        private IConfiguration _configuration;
        public ResourceCommentService(IUnitOfWork unitOfWork
            , IAzureBlobStorageService blobStorageService
            , IConfiguration configuration)
        {
            _resourceRepository = unitOfWork.GetRepository<Resource>();
            _blobStorageService = blobStorageService;
            _configuration = configuration;
        }
        public async Task<ResourceCommentResp> AddResourceToComment(string userName, string hashId, ResourceLocationType locationType)
        {
            var response = new ResourceCommentResp();
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(hashId))
            {
                return null;
            }

            var resource = await _resourceRepository.Connection.QueryFirstOrDefaultAsync<Resource>(GetResourceQuery, new { HashId = hashId });

            if (resource != null)
            {
                string tempBlobName = resource.Name.GetTempBlobName(userName);
                string targetBlobName = resource.Name.GetMediaBlobName(userName);

                // Get a reference to the temp blob
                BlobClient tempBlob = _blobStorageService.GetBlobClient(tempBlobName, BlobStorageDefinition.MediaContainer);
                // Get a reference to the target blob
                BlobClient targetBlob = _blobStorageService.GetBlobClient(targetBlobName, BlobStorageDefinition.MediaContainer);

                var isExistTempFile = await tempBlob.ExistsAsync();
                var isExistTargetFile = await targetBlob.ExistsAsync();

                if (isExistTempFile && !isExistTargetFile)
                {
                    var copyInfo = await targetBlob.StartCopyFromUriAsync(tempBlob.Uri);
                    copyInfo.WaitForCompletion();

                    if (copyInfo.HasCompleted)
                    {
                        resource.Size = copyInfo.Value;
                        await tempBlob.DeleteAsync();
                    }

                    resource.Type = resource.Name.GetResourceType();
                    resource.Url = UrlHelper.CreateMediaUrl(targetBlobName, _configuration["FileSettings:MediaEncryptKey"]);
                    //resource.SubPostId = Guid.Empty;
                    await _resourceRepository.UpdateAsync(resource);
                }

                response.HashId = resource.HashId;
                response.Url = UrlHelper.GetMediaPath(_configuration["FileSettings:MediaUrl"], resource.Name, resource.Url);
                response.Id = resource.Id;
            }


            return response;
        }
    }
}
