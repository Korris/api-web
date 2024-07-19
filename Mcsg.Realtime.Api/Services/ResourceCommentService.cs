using Dapper;

namespace Mcsg.Realtime.Api.Services
{
    using Common.Core.Constants;
    using Common.Core.Enums;
    using Common.Core.Extensions;
    using Common.Core.Interfaces;
    using Common.SeedWork.Extensions;
    using Interfaces;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Repositories;
    using Lib.Data.Repositories.Interface;
    using Requests;

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

                var tempObjectName = $"{Setting.MinioFolder.Media}/{tempBlobName}";
                var isExistTempFile = await _sc.Strategy.StatObjectAsync(tempObjectName, null);

                var targetObjectName = $"{Setting.MinioFolder.Media}/{targetBlobName}";
                var isExistTargetFile = await _sc.Strategy.StatObjectAsync(targetObjectName, null);

                if (isExistTempFile != null && isExistTargetFile == null)
                {
                    await _sc.Strategy.CopyObject(tempObjectName, targetObjectName, null, null);

                    resource.Size = isExistTempFile!.Size;
                    await _sc.Strategy.RemoveObject(tempObjectName, null);

                    resource.Type = resource.Name.GetResourceType();
                    resource.Url = targetBlobName.UrlEncode();
                    //resource.SubPostId = Guid.Empty;
                    await _resourceRepository.UpdateAsync(resource);
                }
                #endregion

                response.HashId = resource.HashId;
                response.Url = _setting.Minio.MediaApiUrl.GetMediaPath(resource.Name, resource.Url);
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
