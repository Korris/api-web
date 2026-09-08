using Dapper;

namespace Mcsg.Api.Areas.Social.Services;

using Common.Core.Constants;
using Common.Domain;
using Common.SeedWork.Exceptions;
using Mcsg.Api.Areas.Social.Models;
using static Common.Core.Constants.Setting;

/// <summary>
/// Newest posts across every content type, no tag filter and no per-type quota.
/// Kept in its own partial file so PostService.cs / PostService.Query.cs do not grow further.
/// </summary>
public partial class PostService
{
    #region -- Methods --

    /// <summary>
    /// Returns the newest <paramref name="take"/> public posts across feed, story, comic and document, ordered by
    /// the latest activity (post creation, or newest chapter for series types). Same visibility rules as
    /// GetLatestPostsByTag: not deleted, status public, permission public, chapter publish date reached.
    /// </summary>
    /// <param name="take">Number of posts; clamped to 1..MaxLatestPosts</param>
    public async Task<ListIdForHomePage> GetLatestPosts(int take)
    {
        try
        {
            var param = new
            {
                Take = Math.Clamp(take, 1, MaxLatestPosts),
                Status = StatusUtils.PostStatusInt
            };

            var rows = await _postCommentRepository.Connection.QueryAsync<LatestPostsResponse>(GetLatestPostsQuery, param);
            var list = rows.ToList();
            return new ListIdForHomePage { TotalItems = list.Count, LatestPostsResponses = list };
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    #endregion

    #region -- Queries --

    /// <summary>
    /// Upper bound for the take parameter so a caller cannot pull the whole table through this endpoint
    /// </summary>
    private const int MaxLatestPosts = 50;

    /// <summary>
    /// One CTE per content type with the same filters as GetLatestPostsByTagQuery minus the tag join,
    /// then a UNION ordered by CreatedOn (post or newest chapter) and limited to @Take.
    /// Series types are pre-limited per CTE so Postgres never sorts the full table before the UNION.
    /// </summary>
    private string GetLatestPostsQuery
    {
        get
        {
            return @"
WITH latest_feed AS (
    SELECT p.""Id"", p.""CreatedOn"", p.""HashId"", 0 AS ""Type""
    FROM social.""SocialPosts"" p
    WHERE p.""IsDelete"" = false AND p.""Type"" = 0 AND p.""Status"" = 1
    ORDER BY p.""CreatedOn"" DESC
    LIMIT @Take
),
latest_story AS (
    SELECT sp.""Id"",
        GREATEST(sp.""CreatedOn"", COALESCE(MAX(ssp.""CreatedOn""), sp.""CreatedOn"")) AS ""CreatedOn"",
        sp.""HashId"", 1 AS ""Type""
    FROM story.""StoryPosts"" sp
    LEFT JOIN story.""StorySubPosts"" ssp ON sp.""Id"" = ssp.""PostId""
    WHERE sp.""IsDelete"" = false AND ssp.""IsDelete"" = false
    AND sp.""Type"" = 1
    AND sp.""Status"" = 1 AND ssp.""Status"" = ANY(@Status)
    AND sp.""Permission"" = 0 AND ssp.""Permission"" = 0
    AND (ssp.""PublishDate"" IS NULL OR ssp.""PublishDate"" < TIMEZONE('UTC', now()))
    GROUP BY sp.""Id"", sp.""CreatedOn"", sp.""HashId""
    ORDER BY 2 DESC
    LIMIT @Take
),
latest_comic AS (
    SELECT cp.""Id"",
        GREATEST(cp.""CreatedOn"", COALESCE(MAX(csp.""CreatedOn""), cp.""CreatedOn"")) AS ""CreatedOn"",
        cp.""HashId"", 2 AS ""Type""
    FROM comic.""ComicPosts"" cp
    LEFT JOIN comic.""ComicSubPosts"" csp ON cp.""Id"" = csp.""PostId""
    WHERE cp.""IsDelete"" = false AND csp.""IsDelete"" = false
    AND cp.""Type"" = 2
    AND cp.""Status"" = 1 AND csp.""Status"" = ANY(@Status)
    AND cp.""Permission"" = 0 AND csp.""Permission"" = 0
    AND (csp.""PublishDate"" IS NULL OR csp.""PublishDate"" < TIMEZONE('UTC', now()))
    GROUP BY cp.""Id"", cp.""CreatedOn"", cp.""HashId""
    ORDER BY 2 DESC
    LIMIT @Take
),
latest_document AS (
    SELECT dp.""Id"",
        GREATEST(dp.""CreatedOn"", COALESCE(MAX(dsp.""CreatedOn""), dp.""CreatedOn"")) AS ""CreatedOn"",
        dp.""HashId"", 4 AS ""Type""
    FROM document.""DocumentPosts"" dp
    LEFT JOIN document.""DocumentSubPosts"" dsp ON dp.""Id"" = dsp.""PostId""
    WHERE dp.""IsDelete"" = false AND dsp.""IsDelete"" = false
    AND dp.""Type"" = 4
    AND dp.""Status"" = 1 AND dsp.""Status"" = ANY(@Status)
    AND dp.""Permission"" = 0 AND dsp.""Permission"" = 0
    AND (dsp.""PublishDate"" IS NULL OR dsp.""PublishDate"" < TIMEZONE('UTC', now()))
    GROUP BY dp.""Id"", dp.""CreatedOn"", dp.""HashId""
    ORDER BY 2 DESC
    LIMIT @Take
),
combined AS (
    SELECT * FROM latest_feed
    UNION ALL SELECT * FROM latest_story
    UNION ALL SELECT * FROM latest_comic
    UNION ALL SELECT * FROM latest_document
)
SELECT ""Id"" AS ""PostId"", ""HashId"", ""Type""
FROM combined
ORDER BY ""CreatedOn"" DESC
LIMIT @Take;";
        }
    }

    #endregion
}
