using Microsoft.EntityFrameworkCore;

namespace Mcsg.Realtime.Api.Services;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Dtos;
using Interfaces;
using Requests;

public partial class ResourceCommentService : IResourceCommentService
{
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    public ResourceCommentService(IMcsgContext context, ISetting setting, IStorageClient sc)
    {
        _context = context;
        _setting = setting;
        _sc = sc;
    }

    public async Task<ResourceCommentResp?> AddResourceToComment(ResourceCommentDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.UserFolder) || string.IsNullOrWhiteSpace(dto.HashId))
        {
            return null;
        }

        if (dto.MicroService == MicroService.Comic.ToString())
        {
            return await AddComicResourceToComment(dto);
        }

        if (dto.MicroService == MicroService.Document.ToString())
        {
            return await AddDocumentResourceToComment(dto);
        }

        if (dto.MicroService == MicroService.Story.ToString())
        {
            return await AddStoryResourceToComment(dto);
        }

        return await AddSocialResourceToComment(dto);
    }

    private async Task<ResourceCommentResp?> AddComicResourceToComment(ResourceCommentDto dto)
    {
        var resource = await _context.ComicResourceAvailable.FirstOrDefaultAsync(p => p.HashId == dto.HashId);
        if (resource == null)
        {
            return null;
        }

        #region -- Copy file from temp target --
        var tempBlobName = resource.Name.GetTempBlobName(dto.UserFolder);
        var targetBlobName = resource.Name.GetMediaBlobName(dto.SubFolder);

        var tempObjectName = $"{Setting.MinioFolder.Comic}/{tempBlobName}";
        var isExistTempFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(tempObjectName, null);

        var targetObjectName = $"{Setting.MinioFolder.Comic}/{targetBlobName}";
        var isExistTargetFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(targetObjectName, null);

        if (isExistTempFile != null && isExistTargetFile == null)
        {
            await _sc.GetStrategy(resource.MinioInstance).CopyObject(tempObjectName, targetObjectName, null, null);

            resource.Size = isExistTempFile!.Size;
            await _sc.GetStrategy(resource.MinioInstance).RemoveObject(tempObjectName, null);

            resource.Type = resource.Name.GetResourceType();
            resource.Url = targetObjectName;
            //resource.SubPostId = Guid.Empty;
            await _context.SaveChangesAsync(default);
        }
        #endregion

        return new ResourceCommentResp
        {
            HashId = resource.HashId,
            Url = await _sc.GetCdnUrlAsync(resource.Url, resource.BucketName, resource.MinioInstance, resource.Type),
            Id = resource.Id
        };
    }

    private async Task<ResourceCommentResp?> AddDocumentResourceToComment(ResourceCommentDto dto)
    {
        var resource = await _context.DocumentResourceAvailable.FirstOrDefaultAsync(p => p.HashId == dto.HashId);
        if (resource == null)
        {
            return null;
        }

        #region -- Copy file from temp target --
        var tempBlobName = resource.Name.GetTempBlobName(dto.UserFolder);
        var targetBlobName = resource.Name.GetMediaBlobName(dto.SubFolder);

        var tempObjectName = $"{Setting.MinioFolder.Document}/{tempBlobName}";
        var isExistTempFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(tempObjectName, null);

        var targetObjectName = $"{Setting.MinioFolder.Document}/{targetBlobName}";
        var isExistTargetFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(targetObjectName, null);

        if (isExistTempFile != null && isExistTargetFile == null)
        {
            await _sc.GetStrategy(resource.MinioInstance).CopyObject(tempObjectName, targetObjectName, null, null);

            resource.Size = isExistTempFile!.Size;
            await _sc.GetStrategy(resource.MinioInstance).RemoveObject(tempObjectName, null);

            resource.Type = resource.Name.GetResourceType();
            resource.Url = targetObjectName;
            await _context.SaveChangesAsync(default);
        }
        #endregion

        return new ResourceCommentResp
        {
            HashId = resource.HashId,
            Url = await _sc.GetCdnUrlAsync(resource.Url, resource.BucketName, resource.MinioInstance, resource.Type),
            Id = resource.Id
        };
    }

    private async Task<ResourceCommentResp?> AddSocialResourceToComment(ResourceCommentDto dto)
    {
        var resource = await _context.SocialResourceAvailable.FirstOrDefaultAsync(p => p.HashId == dto.HashId);
        if (resource == null)
        {
            return null;
        }

        #region -- Copy file from temp target --
        var tempBlobName = resource.Name.GetTempBlobName(dto.UserFolder);
        var targetBlobName = resource.Name.GetMediaBlobName(dto.SubFolder);

        var tempObjectName = $"{Setting.MinioFolder.Social}/{tempBlobName}";
        var isExistTempFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(tempObjectName, null);

        var targetObjectName = $"{Setting.MinioFolder.Social}/{targetBlobName}";
        var isExistTargetFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(targetObjectName, null);

        if (isExistTempFile != null && isExistTargetFile == null)
        {
            await _sc.GetStrategy(resource.MinioInstance).CopyObject(tempObjectName, targetObjectName, null, null);

            resource.Size = isExistTempFile!.Size;
            await _sc.GetStrategy(resource.MinioInstance).RemoveObject(tempObjectName, null);

            resource.Type = resource.Name.GetResourceType();
            resource.Url = targetObjectName;
            //resource.SubPostId = Guid.Empty;
            await _context.SaveChangesAsync(default);
        }
        #endregion

        return new ResourceCommentResp
        {
            HashId = resource.HashId,
            Url = await _sc.GetCdnUrlAsync(resource.Url, resource.BucketName, resource.MinioInstance, resource.Type),
            Id = resource.Id
        };
    }

    private async Task<ResourceCommentResp?> AddStoryResourceToComment(ResourceCommentDto dto)
    {
        var resource = await _context.StoryResourceAvailable.FirstOrDefaultAsync(p => p.HashId == dto.HashId);
        if (resource == null)
        {
            return null;
        }

        #region -- Copy file from temp target --
        var tempBlobName = resource.Name.GetTempBlobName(dto.UserFolder);
        var targetBlobName = resource.Name.GetMediaBlobName(dto.SubFolder);

        var tempObjectName = $"{Setting.MinioFolder.Story}/{tempBlobName}";
        var isExistTempFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(tempObjectName, null);

        var targetObjectName = $"{Setting.MinioFolder.Story}/{targetBlobName}";
        var isExistTargetFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(targetObjectName, null);

        if (isExistTempFile != null && isExistTargetFile == null)
        {
            await _sc.GetStrategy(resource.MinioInstance).CopyObject(tempObjectName, targetObjectName, null, null);

            resource.Size = isExistTempFile!.Size;
            await _sc.GetStrategy(resource.MinioInstance).RemoveObject(tempObjectName, null);

            resource.Type = resource.Name.GetResourceType();
            resource.Url = targetObjectName;
            //resource.SubPostId = Guid.Empty;
            await _context.SaveChangesAsync(default);
        }
        #endregion

        return new ResourceCommentResp
        {
            HashId = resource.HashId,
            Url = await _sc.GetCdnUrlAsync(resource.Url, resource.BucketName, resource.MinioInstance, resource.Type),
            Id = resource.Id
        };
    }

    #region -- Fields --

    /// <summary>
    /// DB Context
    /// </summary>
    private readonly IMcsgContext _context;

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
