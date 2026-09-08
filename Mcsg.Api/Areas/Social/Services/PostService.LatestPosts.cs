using Dapper;

namespace Mcsg.Api.Areas.Social.Services;

using Common.Core.Constants;
using Common.Core.Enums;
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
    /// Each card carries one image: the post thumbnail for series types, the first image of the first sub post for feeds.
    /// </summary>
    /// <param name="take">Number of posts; clamped to 1..MaxLatestPosts</param>
    public async Task<List<LatestPostCardResponse>> GetLatestPosts(int take)
    {
        try
        {
            var param = new
            {
                Take = Math.Clamp(take, 1, MaxLatestPosts),
                Status = StatusUtils.PostStatusInt
            };

            var rows = await _postCommentRepository.Connection.QueryAsync<LatestPostCardRow>(GetLatestPostsQuery, param);
            return rows.Select(ToCard).ToList();
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    /// <summary>
    /// Series thumbnails are stored as ready public urls; feed resources are stored as object names and need the CDN prefix.
    /// </summary>
    private LatestPostCardResponse ToCard(LatestPostCardRow row)
    {
        var thumbnail = row.ThumbnailUrl;
        if (row.Type == PostType.Feed && !string.IsNullOrEmpty(thumbnail))
        {
            thumbnail = _sc.GetCdnUrl(thumbnail, row.BucketName, row.MinioInstance, ResourceType.Image);
        }

        return new LatestPostCardResponse
        {
            Id = row.Id,
            HashId = row.HashId,
            Type = row.Type,
            Title = row.Title,
            ThumbnailUrl = thumbnail,
            CreatedOn = row.CreatedOn
        };
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
    /// Every CTE yields the same column list so the UNION lines up; only the feed CTE fills BucketName/MinioInstance.
    /// </summary>
    private string GetLatestPostsQuery
    {
        get
        {
            return @"
WITH latest_feed AS (
    SELECT p.""Id"", p.""CreatedOn"", p.""HashId"", 0 AS ""Type"",
        COALESCE(NULLIF(p.""Title"", ''), LEFT(p.""Body"", 100)) AS ""Title"",
        img.""Url"" AS ""ThumbnailUrl"", img.""BucketName"", img.""MinioInstance""
    FROM social.""SocialPosts"" p
    LEFT JOIN LATERAL (
        SELECT r.""Url"", r.""BucketName"", r.""MinioInstance""
        FROM social.""SocialSubPosts"" sp
        INNER JOIN social.""SocialResources"" r ON r.""SubPostId"" = sp.""Id"" AND r.""IsDelete"" = false AND r.""Type"" = 0
        WHERE sp.""PostId"" = p.""Id"" AND sp.""IsDelete"" = false
        ORDER BY sp.""Order"", r.""Order""
        LIMIT 1
    ) img ON true
    WHERE p.""IsDelete"" = false AND p.""Type"" = 0 AND p.""Status"" = 1
    ORDER BY p.""CreatedOn"" DESC
    LIMIT @Take
),
latest_story AS (
    SELECT sp.""Id"",
        GREATEST(sp.""CreatedOn"", COALESCE(MAX(ssp.""CreatedOn""), sp.""CreatedOn"")) AS ""CreatedOn"",
        sp.""HashId"", 1 AS ""Type"", sp.""Title"", sp.""ThumbnailUrl"", NULL AS ""BucketName"", NULL::int AS ""MinioInstance""
    FROM story.""StoryPosts"" sp
    LEFT JOIN story.""StorySubPosts"" ssp ON sp.""Id"" = ssp.""PostId""
    WHERE sp.""IsDelete"" = false AND ssp.""IsDelete"" = false
    AND sp.""Type"" = 1
    AND sp.""Status"" = 1 AND ssp.""Status"" = ANY(@Status)
    AND sp.""Permission"" = 0 AND ssp.""Permission"" = 0
    AND (ssp.""PublishDate"" IS NULL OR ssp.""PublishDate"" < TIMEZONE('UTC', now()))
    GROUP BY sp.""Id"", sp.""CreatedOn"", sp.""HashId"", sp.""Title"", sp.""ThumbnailUrl""
    ORDER BY 2 DESC
    LIMIT @Take
),
latest_comic AS (
    SELECT cp.""Id"",
        GREATEST(cp.""CreatedOn"", COALESCE(MAX(csp.""CreatedOn""), cp.""CreatedOn"")) AS ""CreatedOn"",
        cp.""HashId"", 2 AS ""Type"", cp.""Title"", cp.""ThumbnailUrl"", NULL AS ""BucketName"", NULL::int AS ""MinioInstance""
    FROM comic.""ComicPosts"" cp
    LEFT JOIN comic.""ComicSubPosts"" csp ON cp.""Id"" = csp.""PostId""
    WHERE cp.""IsDelete"" = false AND csp.""IsDelete"" = false
    AND cp.""Type"" = 2
    AND cp.""Status"" = 1 AND csp.""Status"" = ANY(@Status)
    AND cp.""Permission"" = 0 AND csp.""Permission"" = 0
    AND (csp.""PublishDate"" IS NULL OR csp.""PublishDate"" < TIMEZONE('UTC', now()))
    GROUP BY cp.""Id"", cp.""CreatedOn"", cp.""HashId"", cp.""Title"", cp.""ThumbnailUrl""
    ORDER BY 2 DESC
    LIMIT @Take
),
latest_document AS (
    SELECT dp.""Id"",
        GREATEST(dp.""CreatedOn"", COALESCE(MAX(dsp.""CreatedOn""), dp.""CreatedOn"")) AS ""CreatedOn"",
        dp.""HashId"", 4 AS ""Type"", dp.""Title"", dp.""ThumbnailUrl"", NULL AS ""BucketName"", NULL::int AS ""MinioInstance""
    FROM document.""DocumentPosts"" dp
    LEFT JOIN document.""DocumentSubPosts"" dsp ON dp.""Id"" = dsp.""PostId""
    WHERE dp.""IsDelete"" = false AND dsp.""IsDelete"" = false
    AND dp.""Type"" = 4
    AND dp.""Status"" = 1 AND dsp.""Status"" = ANY(@Status)
    AND dp.""Permission"" = 0 AND dsp.""Permission"" = 0
    AND (dsp.""PublishDate"" IS NULL OR dsp.""PublishDate"" < TIMEZONE('UTC', now()))
    GROUP BY dp.""Id"", dp.""CreatedOn"", dp.""HashId"", dp.""Title"", dp.""ThumbnailUrl""
    ORDER BY 2 DESC
    LIMIT @Take
),
combined AS (
    SELECT * FROM latest_feed
    UNION ALL SELECT * FROM latest_story
    UNION ALL SELECT * FROM latest_comic
    UNION ALL SELECT * FROM latest_document
)
SELECT ""Id"", ""HashId"", ""Type"", ""Title"", ""ThumbnailUrl"", ""BucketName"", ""MinioInstance"", ""CreatedOn""
FROM combined
ORDER BY ""CreatedOn"" DESC
LIMIT @Take;";
        }
    }

    #endregion
}
