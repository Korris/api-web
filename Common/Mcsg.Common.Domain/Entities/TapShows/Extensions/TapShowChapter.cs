namespace Mcsg.Common.Domain.Entities;

using Core.Constants;
using Core.Enums;
using SeedWork.Extensions;

/// <summary>
/// TapShowChapter domain helpers (create / update / delete)
/// </summary>
partial class TapShowChapter
{
    #region -- Methods --

    /// <summary>
    /// Constructor
    /// </summary>
    public TapShowChapter()
    {
    }

    /// <summary>
    /// Create a chapter under a post. Draft chapters are only visible to the owner.
    /// </summary>
    public static TapShowChapter Create(TapShowPost post, string? title, string? thumbnailUrl, float order, PostStatus status, Guid createdBy)
    {
        return new TapShowChapter
        {
            PostId = post.Id,
            Post = post,
            UserId = createdBy,
            AuthorId = post.AuthorId,
            HashId = Setting.PostConfig.SubHashLength.GetRandomString(),
            Title = title,
            ThumbnailUrl = thumbnailUrl,
            Order = order,
            Status = status,
            PublishDate = status == PostStatus.Public ? DateTime.UtcNow : null,
            IsEnableComment = false,
            CreatedBy = createdBy
        };
    }

    /// <summary>
    /// Update editable fields
    /// </summary>
    public void Update(string? title, string? thumbnailUrl, float order, PostStatus status, Guid modifiedBy)
    {
        Title = title;
        ThumbnailUrl = thumbnailUrl;
        Order = order;
        if (status == PostStatus.Public && Status != PostStatus.Public)
        {
            PublishDate = DateTime.UtcNow;
        }
        Status = status;
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
