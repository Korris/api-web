using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.TapShow.Services;

using Common.Core.Enums;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Enums;
using Common.SeedWork.Exceptions;
using Mcsg.Api.Areas.TapShow.Constants;
using Mcsg.Api.Areas.TapShow.Interfaces;
using Mcsg.Api.Interfaces;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Uploaded image handling shared by post (thumbnail) and segment (image) services. Mirrors GamePostService.Resources.
/// </summary>
public class TapShowResourceService : ITapShowResourceService
{
    #region -- Methods --

    public TapShowResourceService(IMcsgContext context, ISetting setting, IStorageClient sc, ILogger<TapShowResourceService> logger)
    {
        _context = context;
        _setting = setting;
        _sc = sc;
        _logger = logger;
    }

    public async Task<TapShowResource> GetTempResourceAsync(string? hashId, Guid userId, ResourceType type = ResourceType.Image)
    {
        var resource = await _context.TapShowResources
            .FirstOrDefaultAsync(r => r.HashId == hashId && r.AuthorId == userId && r.PostId == null && r.IsDelete && r.Type == type);
        return resource ?? throw new BadRequestException(nameof(E000), TapShowConfig.InvalidResourceUrlMessage);
    }

    public void Attach(TapShowResource resource, TapShowPost post, TapShowSegment? segment = null, TapShowCharacter? character = null)
    {
        if (resource.PostId == post.Id && resource.SegmentId == segment?.Id && resource.CharacterId == character?.Id && !resource.IsDelete)
        {
            return;
        }
        resource.Post = post;
        resource.Segment = segment;
        resource.Character = character;
        resource.IsDelete = false;
        resource.ModifiedBy = post.UserId;
        resource.ModifiedOn = DateTime.UtcNow;
    }

    public void Release(TapShowResource resource, Guid userId)
    {
        resource.Delete(userId);
        _releasedObjects.Add((resource.Url!, resource.BucketName, resource.MinioInstance));
    }

    /// <summary>
    /// Failures are logged, never thrown: the DB change is already committed and an orphan file beats a failed request
    /// </summary>
    public async Task RemoveReleasedObjectsAsync()
    {
        foreach (var (objectName, bucket, instance) in _releasedObjects)
        {
            try
            {
                await _sc.GetStrategy(instance).RemoveObject(objectName, bucket);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[TAPSHOW-STORAGE] cannot remove object {Object}", objectName);
            }
        }
        _releasedObjects.Clear();
    }

    public string PublicUrl(TapShowResource resource)
    {
        return _setting.GetMinio(resource.MinioInstance ?? MinioInstanceType.Default).GetPublicUrl(resource.BucketName, resource.Url);
    }

    #endregion

    #region -- Fields --

    private readonly IMcsgContext _context;
    private readonly ISetting _setting;
    private readonly IStorageClient _sc;
    private readonly ILogger<TapShowResourceService> _logger;
    private readonly List<(string ObjectName, string? Bucket, MinioInstanceType? Instance)> _releasedObjects = new();

    #endregion
}
