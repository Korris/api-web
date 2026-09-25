using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Realtime.Services;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Mcsg.Api.Areas.Realtime.Dtos;
using Mcsg.Api.Interfaces;
using Mcsg.Api.Areas.Realtime.Interfaces;
using Mcsg.Api.Areas.Realtime.Requests;

public partial class ResourceCommentService : IResourceCommentService
{
    /// <summary>
    /// Initialize
    /// </summary>
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

        if (dto.MicroService == MicroService.TapShow.ToString())
        {
            return await AddTapShowResourceToComment(dto);
        }

        return await AddSocialResourceToComment(dto);
    }

    private async Task<ResourceCommentResp?> AddComicResourceToComment(ResourceCommentDto dto)
    {
        var resource = await _context.ComicResources.FirstOrDefaultAsync(p => p.HashId == dto.HashId);
        if (resource == null) return null;

        var tempBlobName = resource.Name.GetTempBlobName(dto.UserFolder);
        var targetBlobName = resource.Name.GetMediaBlobName(dto.SubFolder);
        var tempObjectName = $"{Setting.MinioFolder.Comic}/{tempBlobName}";
        var isExistTempFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(tempObjectName, null);
        var targetObjectName = $"{Setting.MinioFolder.Comic}/{targetBlobName}";

        if (isExistTempFile != null)
        {
            await _sc.GetStrategy(resource.MinioInstance).CopyObject(tempObjectName, targetObjectName, null, null);
            resource.Size = isExistTempFile!.Size;
            await _sc.GetStrategy(resource.MinioInstance).RemoveObject(tempObjectName, null);
            resource.Type = resource.Name.GetResourceType();
            resource.Url = targetObjectName;
            resource.IsDelete = false;
            await _context.SaveChangesAsync(default);
        }

        return new ResourceCommentResp
        {
            HashId = resource.HashId,
            Url = await _sc.GetCdnUrlAsync(resource.Url, resource.BucketName, resource.MinioInstance, resource.Type),
            Id = resource.Id
        };
    }

    private async Task<ResourceCommentResp?> AddDocumentResourceToComment(ResourceCommentDto dto)
    {
        var resource = await _context.DocumentResources.FirstOrDefaultAsync(p => p.HashId == dto.HashId);
        if (resource == null) return null;

        var tempBlobName = resource.Name.GetTempBlobName(dto.UserFolder);
        var targetBlobName = resource.Name.GetMediaBlobName(dto.SubFolder);
        var tempObjectName = $"{Setting.MinioFolder.Document}/{tempBlobName}";
        var isExistTempFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(tempObjectName, null);
        var targetObjectName = $"{Setting.MinioFolder.Document}/{targetBlobName}";

        if (isExistTempFile != null)
        {
            await _sc.GetStrategy(resource.MinioInstance).CopyObject(tempObjectName, targetObjectName, null, null);
            resource.Size = isExistTempFile!.Size;
            await _sc.GetStrategy(resource.MinioInstance).RemoveObject(tempObjectName, null);
            resource.Type = resource.Name.GetResourceType();
            resource.Url = targetObjectName;
            resource.IsDelete = false;
            await _context.SaveChangesAsync(default);
        }

        return new ResourceCommentResp
        {
            HashId = resource.HashId,
            Url = await _sc.GetCdnUrlAsync(resource.Url, resource.BucketName, resource.MinioInstance, resource.Type),
            Id = resource.Id
        };
    }

    private async Task<ResourceCommentResp?> AddSocialResourceToComment(ResourceCommentDto dto)
    {
        var resource = await _context.SocialResources.FirstOrDefaultAsync(p => p.HashId == dto.HashId);
        if (resource == null) return null;

        var tempBlobName = resource.Name.GetTempBlobName(dto.UserFolder);
        var targetBlobName = resource.Name.GetMediaBlobName(dto.SubFolder);
        var tempObjectName = $"{Setting.MinioFolder.Social}/{tempBlobName}";
        var isExistTempFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(tempObjectName, null);
        var targetObjectName = $"{Setting.MinioFolder.Social}/{targetBlobName}";

        if (isExistTempFile != null)
        {
            await _sc.GetStrategy(resource.MinioInstance).CopyObject(tempObjectName, targetObjectName, null, null);
            resource.Size = isExistTempFile!.Size;
            await _sc.GetStrategy(resource.MinioInstance).RemoveObject(tempObjectName, null);
            resource.Type = resource.Name.GetResourceType();
            resource.Url = targetObjectName;
            resource.IsDelete = false;
            await _context.SaveChangesAsync(default);
        }

        return new ResourceCommentResp
        {
            HashId = resource.HashId,
            Url = await _sc.GetCdnUrlAsync(resource.Url, resource.BucketName, resource.MinioInstance, resource.Type),
            Id = resource.Id
        };
    }

    private async Task<ResourceCommentResp?> AddStoryResourceToComment(ResourceCommentDto dto)
    {
        var resource = await _context.StoryResources.FirstOrDefaultAsync(p => p.HashId == dto.HashId);
        if (resource == null) return null;

        var tempBlobName = resource.Name.GetTempBlobName(dto.UserFolder);
        var targetBlobName = resource.Name.GetMediaBlobName(dto.SubFolder);
        var tempObjectName = $"{Setting.MinioFolder.Story}/{tempBlobName}";
        var isExistTempFile = await _sc.GetStrategy(resource.MinioInstance).StatObject(tempObjectName, null);
        var targetObjectName = $"{Setting.MinioFolder.Story}/{targetBlobName}";

        if (isExistTempFile != null)
        {
            await _sc.GetStrategy(resource.MinioInstance).CopyObject(tempObjectName, targetObjectName, null, null);
            resource.Size = isExistTempFile!.Size;
            await _sc.GetStrategy(resource.MinioInstance).RemoveObject(tempObjectName, null);
            resource.Type = resource.Name.GetResourceType();
            resource.Url = targetObjectName;
            resource.IsDelete = false;
            await _context.SaveChangesAsync(default);
        }

        return new ResourceCommentResp
        {
            HashId = resource.HashId,
            Url = await _sc.GetCdnUrlAsync(resource.Url, resource.BucketName, resource.MinioInstance, resource.Type),
            Id = resource.Id
        };
    }

    /// <summary>
    /// TapShow uploads (api/tapshow/file/upload-media) are stored directly at their final object
    /// ({Setting.MinioFolder.TapShow}/{userFolder}/images/{hashId}.jpg) with a temp row (IsDelete = true),
    /// so there is no temp file to copy: only the temp row uploaded by the commenter is claimed.
    /// </summary>
    private async Task<ResourceCommentResp?> AddTapShowResourceToComment(ResourceCommentDto dto)
    {
        var resource = await _context.TapShowResources
            .FirstOrDefaultAsync(p => p.HashId == dto.HashId && p.PostId == null && p.SegmentId == null && p.CharacterId == null);
        if (resource == null || string.IsNullOrWhiteSpace(resource.Url)
            || !resource.Url.StartsWith($"{Setting.MinioFolder.TapShow}/{dto.UserFolder}/"))
        {
            return null;
        }

        if (resource.IsDelete)
        {
            resource.IsDelete = false;
            await _context.SaveChangesAsync(default);
        }

        return new ResourceCommentResp
        {
            HashId = resource.HashId,
            Url = await _sc.GetCdnUrlAsync(resource.Url, resource.BucketName, resource.MinioInstance, resource.Type),
            Id = resource.Id
        };
    }

    #region -- Fields --

    private readonly IMcsgContext _context;
    private readonly ISetting _setting;
    private readonly IStorageClient _sc;

    #endregion
}
