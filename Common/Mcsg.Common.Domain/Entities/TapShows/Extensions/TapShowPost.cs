namespace Mcsg.Common.Domain.Entities;

using Core.Constants;
using Core.Enums;
using SeedWork.Extensions;

/// <summary>
/// TapShowPost domain helpers (create / update / delete)
/// </summary>
partial class TapShowPost
{
    #region -- Methods --

    /// <summary>
    /// Constructor
    /// </summary>
    public TapShowPost()
    {
    }

    /// <summary>
    /// Create a new public TapShow post
    /// </summary>
    public static TapShowPost Create(string? title, string? body, string? thumbnailUrl,
        string? authorName, Guid? authorId, bool isMature, bool isCompleted, PostPermission permission, Guid createdBy)
    {
        var hashId = Setting.PostConfig.HashLength.GetRandomString();

        return new TapShowPost
        {
            Title = title,
            Type = PostType.TapShow,
            Status = PostStatus.Public,
            HashId = hashId,
            Body = body,
            ThumbnailUrl = thumbnailUrl,
            AuthorName = authorName,
            AuthorId = authorId,
            IsMature = isMature,
            IsCompleted = isCompleted,
            Permission = permission,
            Hide = HideOption.None,
            ViewCount = 0,
            UserId = createdBy,
            CreatedBy = createdBy
        };
    }

    /// <summary>
    /// Update editable fields
    /// </summary>
    public void Update(string? title, string? body, string? thumbnailUrl,
        string? authorName, Guid? authorId, bool isMature, bool isCompleted, PostPermission permission, Guid modifiedBy)
    {
        Title = title;
        Body = body;
        ThumbnailUrl = thumbnailUrl;
        AuthorName = authorName;
        AuthorId = authorId;
        IsMature = isMature;
        IsCompleted = isCompleted;
        Permission = permission;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft delete
    /// </summary>
    public void Delete(Guid modifiedBy)
    {
        IsDelete = true;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    #endregion
}
