using Microsoft.EntityFrameworkCore;

namespace Mcsg.Realtime.Api.Services
{
    using Common.Core.Constants;
    using Common.Core.Enums;
    using Common.Core.Extensions;
    using Common.Core.Interfaces;
    using Common.SeedWork.Extensions;
    using Interfaces;
    using Lib.Data;
    using Requests;

    public partial class ResourceCommentService : IResourceCommentService
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="context">DB context</param>
        /// <param name="setting">Setting</param>
        /// <param name="sc">Storage client</param>
        public ResourceCommentService(McsgDbContext context, ISetting setting, IStorageClient sc)
        {
            _context = context;
            _setting = setting;
            _sc = sc;
        }

        public async Task<ResourceCommentResp> AddResourceToComment(string userFolder, string hashId, ResourceLocationType locationType, string microService)
        {
            if (microService == MicroService.Comic.ToString())
            {
                return await AddComicResourceToComment(userFolder, hashId, locationType);
            }

            if (microService == MicroService.Story.ToString())
            {
                return await AddStoryResourceToComment(userFolder, hashId, locationType);
            }

            return await AddSocialResourceToComment(userFolder, hashId, locationType);
        }

        private async Task<ResourceCommentResp> AddComicResourceToComment(string userFolder, string hashId, ResourceLocationType locationType)
        {
            var response = new ResourceCommentResp();
            if (string.IsNullOrWhiteSpace(userFolder) || string.IsNullOrWhiteSpace(hashId))
            {
                return response;
            }

            var resource = await _context.ComicResourceAvailable.FirstOrDefaultAsync(p => p.HashId == hashId);

            if (resource != null)
            {
                #region -- Copy file from temp target --
                string tempBlobName = resource.Name.GetTempBlobName(userFolder);
                string targetBlobName = resource.Name.GetMediaBlobName(userFolder);

                var tempObjectName = $"{Setting.MinioFolder.Comic}/{tempBlobName}";
                var isExistTempFile = await _sc.Strategy.StatObjectAsync(tempObjectName, null);

                var targetObjectName = $"{Setting.MinioFolder.Comic}/{targetBlobName}";
                var isExistTargetFile = await _sc.Strategy.StatObjectAsync(targetObjectName, null);

                if (isExistTempFile != null && isExistTargetFile == null)
                {
                    await _sc.Strategy.CopyObject(tempObjectName, targetObjectName, null, null);

                    resource.Size = isExistTempFile!.Size;
                    await _sc.Strategy.RemoveObject(tempObjectName, null);

                    resource.Type = resource.Name.GetResourceType();
                    resource.Url = targetObjectName.UrlEncode();
                    //resource.SubPostId = Guid.Empty;
                    await _context.SaveChangesAsync();
                }
                #endregion

                response.HashId = resource.HashId;
                response.Url = _setting.Minio.MediaApiUrl.GetMediaPath(resource.Name, resource.Url);
                response.Id = resource.Id;
            }

            return response;
        }

        private async Task<ResourceCommentResp> AddSocialResourceToComment(string userFolder, string hashId, ResourceLocationType locationType)
        {
            var response = new ResourceCommentResp();
            if (string.IsNullOrWhiteSpace(userFolder) || string.IsNullOrWhiteSpace(hashId))
            {
                return response;
            }

            var resource = await _context.ResourceAvailable.FirstOrDefaultAsync(p => p.HashId == hashId);

            if (resource != null)
            {
                #region -- Copy file from temp target --
                string tempBlobName = resource.Name.GetTempBlobName(userFolder);
                string targetBlobName = resource.Name.GetMediaBlobName(userFolder);

                var tempObjectName = $"{Setting.MinioFolder.Social}/{tempBlobName}";
                var isExistTempFile = await _sc.Strategy.StatObjectAsync(tempObjectName, null);

                var targetObjectName = $"{Setting.MinioFolder.Social}/{targetBlobName}";
                var isExistTargetFile = await _sc.Strategy.StatObjectAsync(targetObjectName, null);

                if (isExistTempFile != null && isExistTargetFile == null)
                {
                    await _sc.Strategy.CopyObject(tempObjectName, targetObjectName, null, null);

                    resource.Size = isExistTempFile!.Size;
                    await _sc.Strategy.RemoveObject(tempObjectName, null);

                    resource.Type = resource.Name.GetResourceType();
                    resource.Url = targetObjectName.UrlEncode();
                    //resource.SubPostId = Guid.Empty;
                    await _context.SaveChangesAsync();
                }
                #endregion

                response.HashId = resource.HashId;
                response.Url = _setting.Minio.MediaApiUrl.GetMediaPath(resource.Name, resource.Url);
                response.Id = resource.Id;
            }

            return response;
        }

        private async Task<ResourceCommentResp> AddStoryResourceToComment(string userFolder, string hashId, ResourceLocationType locationType)
        {
            var response = new ResourceCommentResp();
            if (string.IsNullOrWhiteSpace(userFolder) || string.IsNullOrWhiteSpace(hashId))
            {
                return response;
            }

            var resource = await _context.StoryResourceAvailable.FirstOrDefaultAsync(p => p.HashId == hashId);

            if (resource != null)
            {
                #region -- Copy file from temp target --
                string tempBlobName = resource.Name.GetTempBlobName(userFolder);
                string targetBlobName = resource.Name.GetMediaBlobName(userFolder);

                var tempObjectName = $"{Setting.MinioFolder.Story}/{tempBlobName}";
                var isExistTempFile = await _sc.Strategy.StatObjectAsync(tempObjectName, null);

                var targetObjectName = $"{Setting.MinioFolder.Story}/{targetBlobName}";
                var isExistTargetFile = await _sc.Strategy.StatObjectAsync(targetObjectName, null);

                if (isExistTempFile != null && isExistTargetFile == null)
                {
                    await _sc.Strategy.CopyObject(tempObjectName, targetObjectName, null, null);

                    resource.Size = isExistTempFile!.Size;
                    await _sc.Strategy.RemoveObject(tempObjectName, null);

                    resource.Type = resource.Name.GetResourceType();
                    resource.Url = targetObjectName.UrlEncode();
                    //resource.SubPostId = Guid.Empty;
                    await _context.SaveChangesAsync();
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
        /// DB Context
        /// </summary>
        private readonly McsgDbContext _context;

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
