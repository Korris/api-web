namespace Mcsg.Api.Areas.TapShow.Interfaces;

using Common.Core.Enums;
using Common.Domain.Entities;

/// <summary>
/// Shared handling of uploaded TapShow images: resolve a temp upload by hashId (owner-checked),
/// attach it to a post / segment, release it and delete released objects from the bucket.
/// Scoped: released objects are collected per request and removed by RemoveReleasedObjectsAsync after SaveChanges.
/// </summary>
public interface ITapShowResourceService
{
    /// <summary>
    /// A temp upload (IsDelete = true, not attached) of the given type owned by this user, or 400
    /// </summary>
    Task<TapShowResource> GetTempResourceAsync(string? hashId, Guid userId, ResourceType type = ResourceType.Image);

    /// <summary>
    /// Attach a resource to a post (thumbnail), or to a segment (image) / character (avatar) of that post
    /// </summary>
    void Attach(TapShowResource resource, TapShowPost post, TapShowSegment? segment = null, TapShowCharacter? character = null);

    /// <summary>
    /// Soft-delete the row and remember its object for deletion after SaveChanges
    /// </summary>
    void Release(TapShowResource resource, Guid userId);

    /// <summary>
    /// Best-effort delete of the released objects (never throws)
    /// </summary>
    Task RemoveReleasedObjectsAsync();

    /// <summary>
    /// Public URL of a stored object
    /// </summary>
    string PublicUrl(TapShowResource resource);
}
