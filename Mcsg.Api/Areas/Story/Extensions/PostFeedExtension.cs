using System.Linq.Expressions;

namespace Mcsg.Api.Areas.Story.Extensions;

using Common.Domain.Dtos;
using Common.Domain.Entities;
using Mcsg.Api.Areas.Story.Dtos;

/// <summary>
/// StoryPost -> PostFeedDto mapping used by the LoadFeed query
/// </summary>
public static class PostFeedExtension
{
    #region -- Methods --

    /// <summary>
    /// EF projection: only the columns/sub-collections the feed item needs
    /// (latest comment per post & per chapter, published chapters, favorites)
    /// </summary>
    public static readonly Expression<Func<StoryPost, StoryPost>> FeedProjection = p => new StoryPost
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
        StorySubPosts = p.StorySubPosts.Where(sp => !sp.IsDelete && sp.PublishDate <= DateTime.UtcNow).Select(sp => new StorySubPost
        {
            Id = sp.Id,
            CreatedOn = sp.CreatedOn,
            PublishDate = sp.PublishDate,
            StorySubPostComments = sp.StorySubPostComments.Where(c => !c.IsDelete).Select(c => new StorySubPostComment
            {
                Id = c.Id,
                CreatedOn = c.CreatedOn,
                AuthorId = c.AuthorId,
                Body = c.Body,
                Author = new User { Avatar = c.Author.Avatar },
                GifId = c.GifId,
                Resource = c.Resource == null ? null : new StoryResource
                {
                    BucketName = c.Resource.BucketName,
                    MinioInstance = c.Resource.MinioInstance,
                    Url = c.Resource.Url
                },
                CustomNote = c.CustomNote
            }).OrderByDescending(c => c.CreatedOn).Take(1).ToList()
        }).ToList(),
        StoryPostComments = p.StoryPostComments.Where(c => !c.IsDelete).Select(c => new StoryPostComment
        {
            Id = c.Id,
            CreatedOn = c.CreatedOn,
            AuthorId = c.AuthorId,
            Body = c.Body,
            Author = new User { Avatar = c.Author.Avatar },
            GifId = c.GifId,
            Resource = c.Resource == null ? null : new StoryResource
            {
                BucketName = c.Resource.BucketName,
                MinioInstance = c.Resource.MinioInstance,
                Url = c.Resource.Url
            },
            CustomNote = c.CustomNote
        }).OrderByDescending(c => c.CreatedOn).Take(1).ToList(),
        StoryPostFavorites = p.StoryPostFavorites.Where(f => !f.IsDelete).Select(f => new StoryPostFavorite
        {
            UserId = f.UserId
        }).ToList()
    };

    /// <summary>
    /// Map a projected post to the feed item
    /// </summary>
    /// <param name="p">Post (projected by <see cref="FeedProjection"/>)</param>
    /// <param name="userId">Current user</param>
    public static PostFeedDto ToFeedDto(this StoryPost p, Guid? userId)
    {
        var latestPostComment = p.StoryPostComments.Select(c => ToCommentLate(c, c.Author, c.Resource));
        var latestChapterComment = p.StorySubPosts.SelectMany(sp => sp.StorySubPostComments.Select(c => ToCommentLate(c, c.Author, c.Resource)));
        var latestPublishDate = p.StorySubPosts.OrderByDescending(sp => sp.PublishDate).Select(sp => sp.PublishDate).FirstOrDefault();

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
            ChapterCount = p.StorySubPosts.Count,
            FollowCount = p.StoryPostFavorites.Count,
            AuthorName = p.AuthorName,
            IsCurrentUserAuthor = userId == p.UserId,
            IsFollowing = p.StoryPostFavorites.Any(f => f.UserId == userId),
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
