using Dapper;

namespace Mcsg.Api.Areas.Social.Services;

using Common.Core.Enums;
using Common.Domain;
using Common.SeedWork.Extensions;
using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Areas.Social.Requests;

/// <summary>
/// Ranked ids for the home page tabs (For you / Following / New / Trending) across feed, story, comic, document and tapshow.
/// Details are hydrated by HomeFeedAggregationService with each area's get-post-by-list-id.
/// </summary>
public partial class PostService
{
    #region -- Methods --

    public async Task<ListIdForHomePage> GetHomeFeedIds(PostHomeFeedR req)
    {
        var pageNumber = req.PageNumber < 1 ? 1 : req.PageNumber;
        var pageSize = req.PageSize < 1 ? DefaultHomeFeedPageSize : Math.Min(req.PageSize, MaxHomeFeedPageSize);

        switch (req.Type)
        {
            case HomeFeedTab.Following:
                if (req.UserId == null)
                {
                    return new ListIdForHomePage { TotalItems = 0, LatestPostsResponses = new List<LatestPostsResponse>() };
                }
                return await GetNewestPostIds(req.UserId, pageNumber, pageSize);

            case HomeFeedTab.New:
                return await GetNewestPostIds(null, pageNumber, pageSize);

            case HomeFeedTab.Trending:
            {
                // Analytic engagement ranking over the last TrendingDays, not personalized
                var now = DateTime.UtcNow;
                var ranked = await GetMostEngagedPosts(_setting.NumberOfPosts, _setting.PercentFeed, _setting.PercentComic, _setting.PercentDocument,
                    _setting.PercentStory, _setting.PercentTapShow, now.AddDays(-TrendingDays).StartOfDayUtc().ToString(), now.EndOfDayUtc().ToString(), null, req.IsPremium);
                var items = ranked.Items
                    .OrderByDescending(p => p.Point)
                    .Select(p => new LatestPostsResponse { PostId = p.PostId.ToGuid(), HashId = p.HashId, Type = (PostType)p.Type, Point = p.Point })
                    .ToList();
                return Page(items, pageNumber, pageSize);
            }

            default:
            {
                // For you = the existing personalized home ranking
                var ranked = await GetLatestPostsByType(req);
                return Page(ranked.LatestPostsResponses ?? new List<LatestPostsResponse>(), pageNumber, pageSize);
            }
        }
    }

    #endregion

    #region -- Helpers --

    private static ListIdForHomePage Page(List<LatestPostsResponse> items, int pageNumber, int pageSize)
    {
        return new ListIdForHomePage
        {
            TotalItems = items.Count,
            LatestPostsResponses = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
        };
    }

    /// <summary>
    /// Newest public posts across every type ordered by latest activity (post creation, or newest published chapter for series).
    /// <paramref name="followerId"/> set: only posts of the users that follower follows.
    /// </summary>
    private async Task<ListIdForHomePage> GetNewestPostIds(Guid? followerId, int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;
        var follow = followerId == null
            ? ""
            : @" AND p.""UserId"" IN (SELECT f.""UserFollowingId"" FROM identity.""UserFollows"" f WHERE f.""UserFollowerId"" = @FollowerId AND f.""IsDelete"" = false)";

        var query = GetNewestPostIdsQuery.Replace("[Follow]", follow);
        var param = new
        {
            FollowerId = followerId,
            Status = StatusUtils.PostStatusInt,
            Limit = offset + pageSize,
            Offset = offset,
            PageSize = pageSize
        };

        using var multi = await _postCommentRepository.Connection.QueryMultipleAsync(query, param);
        var rows = (await multi.ReadAsync<LatestPostsResponse>()).ToList();
        var total = await multi.ReadFirstAsync<int>();
        return new ListIdForHomePage { TotalItems = total, LatestPostsResponses = rows };
    }

    #endregion

    #region -- Queries --

    private const int DefaultHomeFeedPageSize = 10;
    private const int MaxHomeFeedPageSize = 50;
    private const int TrendingDays = 7;

    /// <summary>
    /// Same visibility rules as GetLatestPostsQuery (+ TapShow): one CTE per type, pre-limited to offset + page size,
    /// then UNION ordered by activity. Second result = total count. [Follow] is replaced by the following filter or nothing.
    /// </summary>
    private const string NewestPostsCtes = @"
feed AS (
    SELECT p.""Id"", p.""HashId"", 0 AS ""Type"", p.""CreatedOn"" AS ""ActivityOn""
    FROM social.""SocialPosts"" p
    WHERE p.""IsDelete"" = false AND p.""Type"" = 0 AND p.""Status"" = 1 [Follow]
),
story AS (
    SELECT p.""Id"", p.""HashId"", 1 AS ""Type"", GREATEST(p.""CreatedOn"", MAX(s.""CreatedOn"")) AS ""ActivityOn""
    FROM story.""StoryPosts"" p
    INNER JOIN story.""StorySubPosts"" s ON p.""Id"" = s.""PostId""
    WHERE p.""IsDelete"" = false AND s.""IsDelete"" = false AND p.""Type"" = 1
    AND p.""Status"" = 1 AND s.""Status"" = ANY(@Status)
    AND p.""Permission"" = 0 AND s.""Permission"" = 0
    AND (s.""PublishDate"" IS NULL OR s.""PublishDate"" < TIMEZONE('UTC', now())) [Follow]
    GROUP BY p.""Id"", p.""HashId"", p.""CreatedOn""
),
comic AS (
    SELECT p.""Id"", p.""HashId"", 2 AS ""Type"", GREATEST(p.""CreatedOn"", MAX(s.""CreatedOn"")) AS ""ActivityOn""
    FROM comic.""ComicPosts"" p
    INNER JOIN comic.""ComicSubPosts"" s ON p.""Id"" = s.""PostId""
    WHERE p.""IsDelete"" = false AND s.""IsDelete"" = false AND p.""Type"" = 2
    AND p.""Status"" = 1 AND s.""Status"" = ANY(@Status)
    AND p.""Permission"" = 0 AND s.""Permission"" = 0
    AND (s.""PublishDate"" IS NULL OR s.""PublishDate"" < TIMEZONE('UTC', now())) [Follow]
    GROUP BY p.""Id"", p.""HashId"", p.""CreatedOn""
),
document AS (
    SELECT p.""Id"", p.""HashId"", 4 AS ""Type"", GREATEST(p.""CreatedOn"", MAX(s.""CreatedOn"")) AS ""ActivityOn""
    FROM document.""DocumentPosts"" p
    INNER JOIN document.""DocumentSubPosts"" s ON p.""Id"" = s.""PostId""
    WHERE p.""IsDelete"" = false AND s.""IsDelete"" = false AND p.""Type"" = 4
    AND p.""Status"" = 1 AND s.""Status"" = ANY(@Status)
    AND p.""Permission"" = 0 AND s.""Permission"" = 0
    AND (s.""PublishDate"" IS NULL OR s.""PublishDate"" < TIMEZONE('UTC', now())) [Follow]
    GROUP BY p.""Id"", p.""HashId"", p.""CreatedOn""
),
tapshow AS (
    SELECT p.""Id"", p.""HashId"", 7 AS ""Type"", p.""CreatedOn"" AS ""ActivityOn""
    FROM tapshow.""TapShowPosts"" p
    WHERE p.""IsDelete"" = false AND p.""Status"" = 1 AND p.""Permission"" = 0 [Follow]
)";

    private static string GetNewestPostIdsQuery => $@"
WITH {NewestPostsCtes},
combined AS (
    (SELECT * FROM feed ORDER BY ""ActivityOn"" DESC LIMIT @Limit)
    UNION ALL (SELECT * FROM story ORDER BY ""ActivityOn"" DESC LIMIT @Limit)
    UNION ALL (SELECT * FROM comic ORDER BY ""ActivityOn"" DESC LIMIT @Limit)
    UNION ALL (SELECT * FROM document ORDER BY ""ActivityOn"" DESC LIMIT @Limit)
    UNION ALL (SELECT * FROM tapshow ORDER BY ""ActivityOn"" DESC LIMIT @Limit)
)
SELECT ""Id"" AS ""PostId"", ""HashId"", ""Type"", 0::real AS ""Point""
FROM combined
ORDER BY ""ActivityOn"" DESC
LIMIT @PageSize OFFSET @Offset;

WITH {NewestPostsCtes}
SELECT ((SELECT COUNT(*) FROM feed) + (SELECT COUNT(*) FROM story) + (SELECT COUNT(*) FROM comic)
      + (SELECT COUNT(*) FROM document) + (SELECT COUNT(*) FROM tapshow))::int;";

    #endregion
}
