using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Game.Services;

using Common.Core.Enums;
using Common.Domain.Entities;
using Common.SeedWork.Enums;
using Common.SeedWork.Exceptions;
using Mcsg.Api.Areas.Game.Constants;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// GameResource handling for posts: resolve uploaded files by hashId (owner-checked), attach / release,
/// and delete released objects from the public bucket. Mirrors the Comic thumbnail flow.
/// </summary>
public partial class GamePostService
{
    #region -- Helpers --

    /// <summary>
    /// Which slot of the post a resource fills
    /// </summary>
    private enum ResourceKind
    {
        Thumbnail,
        Game
    }

    /// <summary>
    /// A temp upload (IsDelete = true, no PostId) owned by this user, of the right type. Anything else → 400.
    /// </summary>
    private async Task<GameResource> GetTempResourceAsync(string? hashId, Guid userId, ResourceKind kind)
    {
        var type = TypeOf(kind);
        var resource = await _context.GameResources
            .FirstOrDefaultAsync(r => r.HashId == hashId && r.AuthorId == userId && r.PostId == null && r.IsDelete && r.Type == type);
        return resource ?? throw new BadRequestException(nameof(E000), GameConfig.InvalidResourceUrlMessage);
    }

    /// <summary>
    /// Update: same hashId as the attached file → keep it; new hashId → take the new temp upload and release the old one
    /// </summary>
    private async Task<GameResource> ResolveForUpdateAsync(GamePost post, string? hashId, Guid userId, ResourceKind kind)
    {
        var type = TypeOf(kind);
        var current = post.GameResources.FirstOrDefault(r => !r.IsDelete && r.Type == type);
        if (current != null && current.HashId == hashId)
        {
            return current;
        }
        var replacement = await GetTempResourceAsync(hashId, userId, kind);
        if (current != null)
        {
            Release(current, userId);
        }
        return replacement;
    }

    private static void Attach(GameResource resource, GamePost post)
    {
        if (resource.PostId == post.Id && !resource.IsDelete)
        {
            return;
        }
        resource.Post = post;
        resource.IsDelete = false;
        resource.ModifiedBy = post.UserId;
        resource.ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft-delete the row and remember the object for deletion after SaveChanges
    /// </summary>
    private void Release(GameResource resource, Guid userId)
    {
        resource.Delete(userId);
        _releasedObjects.Add((resource.Url!, resource.BucketName, resource.MinioInstance));
    }

    /// <summary>
    /// Best-effort delete of released objects. Failures are logged, never thrown:
    /// the DB change is already committed and an orphan file is preferable to a failed request.
    /// </summary>
    private async Task RemoveReleasedObjectsAsync()
    {
        foreach (var (objectName, bucket, instance) in _releasedObjects)
        {
            try
            {
                await _sc.GetStrategy(instance).RemoveObject(objectName, bucket);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[GAME-STORAGE] cannot remove object {Object}", objectName);
            }
        }
        _releasedObjects.Clear();
    }

    private string PublicUrl(GameResource resource)
    {
        return _setting.GetMinio(resource.MinioInstance ?? MinioInstanceType.Default).GetPublicUrl(resource.BucketName, resource.Url);
    }

    private static ResourceType TypeOf(ResourceKind kind)
    {
        return kind == ResourceKind.Thumbnail ? ResourceType.Image : ResourceType.Other;
    }

    #endregion

    #region -- Fields --

    private readonly List<(string ObjectName, string? Bucket, MinioInstanceType? Instance)> _releasedObjects = new();

    #endregion
}
