using System.Linq.Expressions;

namespace Mcsg.Api.Areas.Document.Extensions;

using Common.Domain.Dtos;
using Common.Domain.Entities;
using Mcsg.Api.Areas.Document.Dtos;

/// <summary>
/// DocumentPost -> PostFeedDto mapping used by the LoadFeed query
/// </summary>
public static class PostFeedExtension
{
    #region -- Methods --

    /// <summary>
    /// EF projection: only the columns/sub-collections the feed item needs
    /// (latest comment per post & per chapter, published chapters, favorites)
    /// </summary>
    public static readonly Expression<Func<DocumentPost, DocumentPost>> FeedProjection = p => new DocumentPost
    {
        Id = p.Id,
        Title = p.Title,
        HashId = p.HashId,
        ThumbnailUrl = p.ThumbnailUrl,
        Body = p.Body,
        Status = p.Status,
        IsMature = p.IsMature,
        IsCompleted = p.IsCompleted,
        ViewCount = p.ViewCount,
        CustomNote = p.CustomNote,
        CreatedOn = p.CreatedOn,
        UserId = p.UserId,
        Permission = p.Permission,
        AuthorName = p.User.ProfileName,
        DocumentSubPosts = p.DocumentSubPosts.Where(sp => !sp.IsDelete && sp.PublishDate <= DateTime.UtcNow).Select(sp => new DocumentSubPost
        {
            Id = sp.Id,
            CreatedOn = sp.CreatedOn,
            PublishDate = sp.PublishDate,
            DocumentSubPostComments = sp.DocumentSubPostComments.Where(c => !c.IsDelete).Select(c => new DocumentSubPostComment
            {
                Id = c.Id,
                CreatedOn = c.CreatedOn,
                AuthorId = c.AuthorId,
                Body = c.Body,
                Author = new User { Avatar = c.Author.Avatar },
                GifId = c.GifId,
                Resource = c.Resource == null ? null : new DocumentResource
                {
                    BucketName = c.Resource.BucketName,
                    MinioInstance = c.Resource.MinioInstance,
                    Url = c.Resource.Url
                },
                CustomNote = c.CustomNote
            }).OrderByDescending(c => c.CreatedOn).Take(1).ToList()
        }).ToList(),
        DocumentPostComments = p.DocumentPostComments.Where(c => !c.IsDelete).Select(c => new DocumentPostComment
        {
            Id = c.Id,
            CreatedOn = c.CreatedOn,
            AuthorId = c.AuthorId,
            Body = c.Body,
            Author = new User { Avatar = c.Author.Avatar },
            GifId = c.GifId,
            Resource = c.Resource == null ? null : new DocumentResource
            {
                BucketName = c.Resource.BucketName,
                MinioInstance = c.Resource.MinioInstance,
                Url = c.Resource.Url
            },
            CustomNote = c.CustomNote
        }).OrderByDescending(c => c.CreatedOn).Take(1).ToList(),
        DocumentPostFavorites = p.DocumentPostFavorites.Where(f => !f.IsDelete).Select(f => new DocumentPostFavorite
        {
            UserId = f.UserId
        }).ToList()
    };

    /// <summary>
    /// Map a projected post to the feed item
    /// </summary>
    /// <param name="p">Post (projected by <see cref="FeedProjection"/>)</param>
    /// <param name="userId">Current user</param>
    public static PostFeedDto ToFeedDto(this DocumentPost p, Guid? userId)
    {
        var latestPostComment = p.DocumentPostComments.Select(c => ToCommentLate(c, c.Author, c.Resource));
        var latestChapterComment = p.DocumentSubPosts.SelectMany(sp => sp.DocumentSubPostComments.Select(c => ToCommentLate(c, c.Author, c.Resource)));
        var latestPublishDate = p.DocumentSubPosts.OrderByDescending(sp => sp.PublishDate).Select(sp => sp.PublishDate).FirstOrDefault();

        return new PostFeedDto
        {
            Id = p.Id,
            Title = p.Title,
            HashId = p.HashId,
            ThumbnailUrl = p.ThumbnailUrl,
            Body = p.Body,
            Status = p.Status,
            Permission = p.Permission,
            IsMature = p.IsMature,
            IsCompleted = p.IsCompleted,
            ViewCount = p.ViewCount,
            CustomNote = p.CustomNote,
            CreatedOn = p.CreatedOn,
            ChapterCount = p.DocumentSubPosts.Count,
            FollowCount = p.DocumentPostFavorites.Count,
            AuthorName = p.AuthorName,
            IsCurrentUserAuthor = userId == p.UserId,
            IsFollowing = p.DocumentPostFavorites.Any(f => f.UserId == userId),
            LatestComment = latestPostComment.Concat(latestChapterComment).OrderByDescending(c => c.CreatedOn).FirstOrDefault(),
            SortCreatedOn = latestPublishDate ?? p.CreatedOn
        };
    }

    /// <summary>
    /// Comment (post or chapter) -> CommentLateDto
    /// </summary>
    private static CommentLateDto ToCommentLate(BasePostComment c, User author, BaseResource? resource) => new()
    {
        Id = c.Id,
        Body = c.Body,
        AuthorId = c.AuthorId,
        CreatedOn = c.CreatedOn,
        Avatar = author.Avatar,
        GifId = c.GifId,
        MinioInstance = resource?.MinioInstance,
        BucketName = resource?.BucketName,
        ResourceUrl = resource?.Url,
        CustomNote = c.CustomNote
    };

    #endregion
}
