using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Document.Queries;

using Common.Core.Enums;
using Common.Domain.Entities;
using Mcsg.Api.Areas.Document.Models;

/// <summary>
/// LoadFeed — per-post reaction summary and comment counts (batched by post ids)
/// </summary>
public partial class PostLoadFeedH
{
    #region -- Methods --

    /// <summary>
    /// Reaction summary of every post: count per type, total, most used and the current user's reaction
    /// </summary>
    /// <param name="postIds">Post ids</param>
    /// <param name="userId">Current user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>One entry per requested post id (empty summary when the post has no reaction)</returns>
    private async Task<Dictionary<Guid, ReactionsResponse>> GetReactionsAsync(List<Guid> postIds, Guid? userId, CancellationToken cancellationToken)
    {
        var qReaction = _context.Available<DocumentPostReaction>(false).Where(r => postIds.Contains(r.TargetId));

        var counts = await qReaction
            .GroupBy(r => new { r.TargetId, r.Type })
            .Select(g => new { g.Key.TargetId, g.Key.Type, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var currentUserReacts = new Dictionary<Guid, ReactionType>();
        if (userId != null)
        {
            var rows = await qReaction.Where(r => r.AuthorId == userId)
                .Select(r => new { r.TargetId, r.Type })
                .ToListAsync(cancellationToken);
            foreach (var r in rows)
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
                CurrentUserReactType = currentUserReacts.TryGetValue(postId, out var mine) ? mine : null,
                Reactions = reactions,
                MostReactionType = reactions.OrderByDescending(r => r.Count).FirstOrDefault()?.Type
            };
        }

        return res;
    }

    /// <summary>
    /// Total comments per post = post comments + chapter comments (only comments whose author still exists
    /// and whose parent, if any, still exists)
    /// </summary>
    /// <param name="postIds">Post ids</param>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task<Dictionary<Guid, int>> GetTotalCommentsAsync(List<Guid> postIds, CancellationToken cancellationToken)
    {
        var qPostComment = _context.Available<DocumentPostComment>(false);
        var postComments = await (
            from a in qPostComment
            join b in _context.UserAvailable on a.AuthorId equals b.Id
            where postIds.Contains(a.PostId)
                && (a.ParentId == null || qPostComment.Any(p => p.Id == a.ParentId))
            group a by a.PostId into g
            select new { PostId = g.Key, Count = g.Count() }
        ).ToListAsync(cancellationToken);

        var qSubPostComment = _context.Available<DocumentSubPostComment>(false);
        var subPostComments = await (
            from a in qSubPostComment
            join b in _context.Available<DocumentSubPost>(false) on a.PostId equals b.Id
            join c in _context.UserAvailable on a.AuthorId equals c.Id
            where postIds.Contains(b.PostId)
                && (a.ParentId == null || qSubPostComment.Any(p => p.Id == a.ParentId))
            group a by b.PostId into g
            select new { PostId = g.Key, Count = g.Count() }
        ).ToListAsync(cancellationToken);

        var res = new Dictionary<Guid, int>();
        foreach (var i in postComments.Concat(subPostComments))
        {
            res[i.PostId] = res.GetValueOrDefault(i.PostId) + i.Count;
        }

        return res;
    }

    #endregion
}
