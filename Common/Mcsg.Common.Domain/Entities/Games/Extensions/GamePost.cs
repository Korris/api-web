namespace Mcsg.Common.Domain.Entities;

using Core.Constants;
using Core.Enums;
using SeedWork.Extensions;

/// <summary>
/// GamePost domain helpers (create / update / delete)
/// </summary>
partial class GamePost
{
    #region -- Methods --

    /// <summary>
    /// Constructor
    /// </summary>
    public GamePost()
    {
    }

    /// <summary>
    /// Create a new public game post
    /// </summary>
    public static GamePost Create(string? title, string? body, string? thumbnailUrl, string? gameUrl,
        string? authorName, Guid? authorId, bool isMature, PostPermission permission, Guid createdBy)
    {
        var hashId = Setting.PostConfig.HashLength.GetRandomString();

        return new GamePost
        {
            Title = title,
            Type = PostType.Game,
            Status = PostStatus.Public,
            HashId = hashId,
            Body = body,
            ThumbnailUrl = thumbnailUrl,
            GameUrl = gameUrl,
            AuthorName = authorName,
            AuthorId = authorId,
            IsMature = isMature,
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
    public void Update(string? title, string? body, string? thumbnailUrl, string? gameUrl,
        string? authorName, Guid? authorId, bool isMature, PostPermission permission, Guid modifiedBy)
    {
        Title = title;
        Body = body;
        ThumbnailUrl = thumbnailUrl;
        GameUrl = gameUrl;
        AuthorName = authorName;
        AuthorId = authorId;
        IsMature = isMature;
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
