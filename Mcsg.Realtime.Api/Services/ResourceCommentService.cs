using Dapper;

namespace Mcsg.Realtime.Api.Services
{
    using Api.Interfaces;
    using Common.Core.Interfaces;
    using Lib.Common.Constants;
    using Lib.Common.Extensions;
    using Lib.Common.Helpers;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Enums;
    using Lib.Data.Repositories;
    using Lib.Data.Repositories.Interface;
    using Realtime.Api.DTOs;

    public interface IResourceCommentService
    {
        Task<ResourceCommentResp> AddResourceToComment(string userName, string hashId, ResourceLocationType locationType);
    }
    public partial class ResourceCommentService : IResourceCommentService
    {
        private readonly IRepository<Resource> _resourceRepository;
        private IConfiguration _configuration;
        public ResourceCommentService(IUnitOfWork unitOfWork
            , IConfiguration configuration
            , ISetting setting
            , IStorageClient sc)
        {
            _resourceRepository = unitOfWork.GetRepository<Resource>();
            _configuration = configuration;
            _setting = setting;
            _sc = sc;
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
                #region -- Copy file from temp target --
                string tempBlobName = resource.Name.GetTempBlobName(userName);
                string targetBlobName = resource.Name.GetMediaBlobName(userName);

                tempBlobName = $"{BlobStorageDefinition.MediaContainer}/{tempBlobName}";
                var isExistTempFile = await _sc.Strategy.StatObjectAsync(tempBlobName, null);

                targetBlobName = $"{BlobStorageDefinition.MediaContainer}/{targetBlobName}";
                var isExistTargetFile = await _sc.Strategy.StatObjectAsync(targetBlobName, null);

                if (isExistTempFile != null && isExistTargetFile == null)
                {
                    await _sc.Strategy.CopyObject(tempBlobName, targetBlobName, null, null);

                    resource.Size = isExistTempFile!.Size;
                    await _sc.Strategy.RemoveObject(tempBlobName, null);

                    resource.Type = resource.Name.GetResourceType();
                    resource.Url = UrlHelper.CreateMediaUrl(targetBlobName, _setting.Minio.MediaEncryptKey);
                    //resource.SubPostId = Guid.Empty;
                    await _resourceRepository.UpdateAsync(resource);
                }
                #endregion

                response.HashId = resource.HashId;
                response.Url = UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, resource.Name, resource.Url);
                response.Id = resource.Id;
            }

            return response;
        }

        #region -- Fields --

        /// <summary>
        /// Setting
        /// </summary>
        private readonly ISetting _setting;

        /// <summary>
        /// Storage client
        /// </summary>
        private readonly IStorageClient _sc;

        #endregion
    }
}
