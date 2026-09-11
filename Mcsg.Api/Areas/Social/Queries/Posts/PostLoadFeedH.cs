using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Mcsg.Api.Interfaces;

namespace Mcsg.Api.Areas.Social.Queries;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Dtos;
using Mcsg.Api.Areas.Social.Filters;
using Mcsg.Api.Areas.Social.Interfaces;
using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Areas.Social.Requests;

/// <summary>
/// PATCH v1/LoadFeed — feed posts of one user (social.fm_posts_by_username).
/// Ported from api-mobile PostLoadFeedH; item mapping reuses IFeedService.
/// </summary>
public class PostLoadFeedH : BaseMinioH, IRequestHandler<PostLoadFeedR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    /// <param name="feedService">Feed service</param>
    public PostLoadFeedH(IMcsgContext context, ISetting setting, IStorageClient sc, IFeedService feedService) : base(context, setting, sc)
    {
        _feedService = feedService;
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
        const string sql = "SELECT * FROM social.fm_posts_by_username (@UserName, @Hides, @PageSize, @Offset, @IsMyseft)";

        try
        {
            #region -- Filter --
            string? userName = null;

            if (request.Filter != null)
            {
                var ft = (request.Filter + "").ToInstNull<PostFilter.Search>();
                if (ft != null)
                {
                    userName = ft.UserName;
                }
            }

            var authorId = await _context.GetUserId(userName);

            // Total = author's posts visible to the caller (public, premium for premium users, not hidden on this platform)
            var q = _context.Available<SocialPost>(false)
                .Where(p => p.UserId == authorId)
                .Where(p => p.Permission == PostPermission.Public || (request.IsPremium && p.Permission == PostPermission.Premium));

            if (request.Hides != null && request.Hides.Count > 0)
            {
                q = q.Where(p => !request.Hides.Contains((int)p.Hide));
            }

            res.TotalRecords = await q.CountAsync(cancellationToken);
            #endregion

            var favoritePostIds = request.UserId == null
                ? []
                : await _context.Available<SocialPostFavorite>(false)
                    .Where(p => p.UserId == request.UserId).Select(p => p.PostId).ToListAsync(cancellationToken);

            var param = new
            {
                UserName = userName,
                Hides = request.Hides != null ? request.Hides.ToArray() : [],
                PageSize = (int)request.PageSize,
                Offset = (int)request.Offset,
                IsMyseft = true
            };
            var connection = _context.Database.GetDbConnection();
            var rows = (await connection.QueryAsync<FeedsListQueryDbDto>(sql, param)).ToList();

            var sharePostInputs = rows.Where(p => p.SharePostId.HasValue)
                .Select(p => new SharePostInput { Id = p.SharePostId!.Value, Type = p.SharePostType ?? SharePostType.Feed })
                .ToList();
            var sharePosts = await _feedService.GetSharePosts(request, sharePostInputs);
            var reactions = await GetReactionsAsync(rows.Select(p => p.Id).ToList(), request.UserId, cancellationToken);

            var data = new List<FeedDto>();
            foreach (var i in rows)
            {
                var o = _feedService.MappingFeedInListRespone(i, favoritePostIds, i.IsCurrentUserAuthor);
                o.SharePost = sharePosts.FirstOrDefault(p => p.Id == i.SharePostId);
                o.Reaction = reactions[o.Id];
                data.Add(o);
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

    /// <summary>
    /// Reaction summary of every post: count per type, total, most used and the current user's reaction
    /// </summary>
    /// <param name="postIds">Post ids</param>
    /// <param name="userId">Current user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>One entry per requested post id (empty summary when the post has no reaction)</returns>
    private async Task<Dictionary<Guid, ReactionsResponse>> GetReactionsAsync(List<Guid> postIds, Guid? userId, CancellationToken cancellationToken)
    {
        var qReaction = _context.Available<SocialPostReaction>(false).Where(r => postIds.Contains(r.TargetId));

        var counts = await qReaction
            .GroupBy(r => new { r.TargetId, r.Type })
            .Select(g => new { g.Key.TargetId, g.Key.Type, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var currentUserReacts = new Dictionary<Guid, ReactionType>();
        if (userId != null)
        {
            var mine = await qReaction.Where(r => r.AuthorId == userId)
                .Select(r => new { r.TargetId, r.Type })
                .ToListAsync(cancellationToken);
            foreach (var r in mine)
            {
                currentUserReacts.TryAdd(r.TargetId, r.Type);
            }
        }

        var res = new Dictionary<Guid, ReactionsResponse>();
        foreach (var postId in postIds)
        {
            var reactions = counts.Where(c => c.TargetId == postId)
                .Select(c => new ReactionResponse { Type = c.Type, Count = c.Count })
                .ToList();

            res[postId] = new ReactionsResponse
            {
                TargetId = postId,
                TotalReacts = reactions.Sum(r => r.Count),
                CurrentUserReactType = currentUserReacts.TryGetValue(postId, out var type) ? type : null,
                Reactions = reactions,
                MostReactionType = reactions.OrderByDescending(r => r.Count).FirstOrDefault()?.Type
            };
        }

        return res;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Feed service
    /// </summary>
    private readonly IFeedService _feedService;

    #endregion
}
