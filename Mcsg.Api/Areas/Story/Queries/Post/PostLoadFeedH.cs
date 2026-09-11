using MediatR;
using Microsoft.EntityFrameworkCore;
using Mcsg.Api.Interfaces;

namespace Mcsg.Api.Areas.Story.Queries;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Story.Extensions;
using Mcsg.Api.Areas.Story.Filters;
using Mcsg.Api.Areas.Story.Requests;

/// <summary>
/// PATCH v1/LoadFeed — public feed of comics (by author / keyword / hashtag / favorites).
/// Ported from api-mobile PostSearchH with IsMine = false.
/// </summary>
public partial class PostLoadFeedH : BaseMinioH, IRequestHandler<PostLoadFeedR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    /// <param name="businessText">Business text</param>
    public PostLoadFeedH(IMcsgContext context, ISetting setting, IStorageClient sc, IBusinessText businessText) : base(context, setting, sc)
    {
        _businessText = businessText;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(PostLoadFeedR request, CancellationToken cancellationToken)
    {
        var res = new SearchResponse(request.PageNum, request.PageSize, request.Paging);

        try
        {
            var q = _context.Available<StoryPost>(false);

            #region -- Filter --
            Guid? userId = null;
            string? userName = null;
            string? keyword = null;
            string? hashtag = null;

            if (request.Filter != null)
            {
                var ft = (request.Filter + "").ToInstNull<PostFilter.Search>();
                if (ft != null)
                {
                    userId = ft.UserId;
                    userName = ft.UserName;
                    keyword = ft.Keyword;
                    hashtag = ft.Hashtag;
                }
            }

            // Convert UserName to UserId
            userId ??= await _context.GetUserId(userName);

            // Keyword (hoisted: EF evaluates keyword.ToLower() client-side even when keyword is null)
            var kw = (keyword ?? "").Trim().ToLower();
            if (kw != "")
            {
                q = q.Where(p => (p.Title + "").ToLower().Contains(kw) || (p.Body + "").ToLower().Contains(kw));
            }

            // Hashtag
            if (!string.IsNullOrWhiteSpace(hashtag))
            {
                q = q.Where(p => p.StoryTagPosts.Any(tp => (tp.Tag.Name + "") == hashtag));
            }

            // Status
            q = q.Where(p => StatusUtils.PostStatuses.Contains(p.Status));

            // Hides (platform) — feed is never the author's own list
            if (request.Hides != null && request.Hides.Count > 0)
            {
                q = q.Where(p => !request.Hides.Contains((int)p.Hide));
                q = q.Where(p => p.StorySubPosts.Any(sp => !sp.IsDelete));
            }

            // Permission: author feed shows all of their posts, otherwise public (+ premium for premium users)
            if (userId == null)
            {
                q = request.IsPremium
                    ? q.Where(p => p.Permission == PostPermission.Public || p.Permission == PostPermission.Premium)
                    : q.Where(p => p.Permission == PostPermission.Public);
            }
            else
            {
                q = q.Where(p => p.UserId == userId);
            }

            // Hidden by current user
            q = q.Where(p => !p.StoryPostHides.Any(h => h.UserId == request.UserId && !h.IsDelete));

            q = q.OrderByDescending(p => p.StorySubPosts.Max(sp => sp.CreatedOn) > p.CreatedOn
                ? p.StorySubPosts.Max(sp => sp.CreatedOn)
                : p.CreatedOn);

            // Favorite
            if (request.IsFavorite && request.UserId != null)
            {
                q = q.Where(p => p.StoryPostFavorites.Any(f => f.UserId == request.UserId && !f.IsDelete));
            }
            #endregion

            // Paging
            res.TotalRecords = await q.CountAsync(cancellationToken);
            if (request.Paging)
            {
                q = q.Sort(request.Sort).PageBy(request.Offset, request.PageSize);
            }

            // Result
            var posts = await q.Select(PostFeedExtension.FeedProjection).ToListAsync(cancellationToken);
            var data = posts.Select(p => p.ToFeedDto(request.UserId)).OrderByDescending(p => p.SortCreatedOn).ToList();

            var postIds = data.Select(p => p.Id).ToList();
            var totalComments = await GetTotalCommentsAsync(postIds, cancellationToken);
            var reactions = await GetReactionsAsync(postIds, request.UserId, cancellationToken);

            foreach (var item in data)
            {
                item.IsBlur = item.Status == PostStatus.Inactive || item.IsMature == true;
                item.IsCensored = !request.IsAdministrator && !item.IsCurrentUserAuthor && item.Status == PostStatus.Inactive;
                item.Reaction = reactions[item.Id];
                item.TotalComments = totalComments.TryGetValue(item.Id, out var total) ? total : null;

                if (item.LatestComment != null)
                {
                    item.LatestComment.Body = await _businessText.Process(item.LatestComment.Body);

                    if (item.LatestComment.ResourceUrl != null)
                    {
                        item.LatestComment.ResourceUrl = await _sc.GetPublicUrl(item.LatestComment.ResourceUrl, item.LatestComment.BucketName, item.LatestComment.MinioInstance);
                    }
                }
            }

            res.SetSuccess(data);
        }
        catch (Exception ex)
        {
            ex.Message.LogError();
            res.SetError(ex.Message);
        }

        return res;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Business text
    /// </summary>
    private readonly IBusinessText _businessText;

    #endregion
}
