using Dapper;

namespace Mcsg.Api.Areas.Social.Services;

using Common.Core.Constants;
using Common.Domain;
using Common.SeedWork.Exceptions;
using Mcsg.Api.Areas.Social.Models;
using static Common.Core.Constants.Setting;

/// <summary>
/// Chapters scheduled for a future publish date ("coming soon"), across story, comic and document.
/// Own partial file so PostService.cs / PostService.Query.cs do not grow further.
/// </summary>
public partial class PostService
{
    #region -- Methods --

    /// <summary>
    /// Returns sub posts whose PublishDate is still in the future, soonest first.
    /// Parent post must be public (not deleted, status public, permission public); the sub post must not be deleted
    /// or private. Premium chapters are included so they can be announced before release.
    /// </summary>
    /// <param name="take">Number of rows; clamped to 1..MaxUpcomingSubPosts</param>
    public async Task<List<UpcomingSubPostResponse>> GetUpcomingSubPosts(int take)
    {
        try
        {
            var param = new
            {
                Take = Math.Clamp(take, 1, MaxUpcomingSubPosts),
                Status = StatusUtils.PostStatusInt
            };

            var rows = await _postCommentRepository.Connection.QueryAsync<UpcomingSubPostResponse>(GetUpcomingSubPostsQuery, param);
            return rows.ToList();
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    #endregion

    #region -- Queries --

    /// <summary>
    /// Upper bound for the take parameter
    /// </summary>
    private const int MaxUpcomingSubPosts = 50;

    /// <summary>
    /// One SELECT per content type (same shape, different schema), UNION ALL, then order by publish date.
    /// Each branch is limited to @Take before the UNION so Postgres only scans the earliest rows per type.
    /// Type values match the posts' "Type" column: 1 story, 2 comic, 4 document.
    /// </summary>
    private string GetUpcomingSubPostsQuery
    {
        get
        {
            return @"
WITH upcoming_story AS (
    SELECT ssp.""Id"", ssp.""HashId"", ssp.""Title"" AS ""SubPostTitle"", ssp.""PublishDate"",
           sp.""Id"" AS ""PostId"", sp.""HashId"" AS ""PostHashId"", sp.""Title"", 1 AS ""Type""
    FROM story.""StorySubPosts"" ssp
    JOIN story.""StoryPosts"" sp ON sp.""Id"" = ssp.""PostId""
    WHERE ssp.""IsDelete"" = false AND sp.""IsDelete"" = false
    AND ssp.""PublishDate"" > TIMEZONE('UTC', now())
    AND ssp.""Status"" = ANY(@Status) AND ssp.""Permission"" <> 1
    AND sp.""Status"" = 1 AND sp.""Permission"" = 0
    ORDER BY ssp.""PublishDate"" ASC
    LIMIT @Take
),
upcoming_comic AS (
    SELECT csp.""Id"", csp.""HashId"", csp.""Title"" AS ""SubPostTitle"", csp.""PublishDate"",
           cp.""Id"" AS ""PostId"", cp.""HashId"" AS ""PostHashId"", cp.""Title"", 2 AS ""Type""
    FROM comic.""ComicSubPosts"" csp
    JOIN comic.""ComicPosts"" cp ON cp.""Id"" = csp.""PostId""
    WHERE csp.""IsDelete"" = false AND cp.""IsDelete"" = false
    AND csp.""PublishDate"" > TIMEZONE('UTC', now())
    AND csp.""Status"" = ANY(@Status) AND csp.""Permission"" <> 1
    AND cp.""Status"" = 1 AND cp.""Permission"" = 0
    ORDER BY csp.""PublishDate"" ASC
    LIMIT @Take
),
upcoming_document AS (
    SELECT dsp.""Id"", dsp.""HashId"", dsp.""Title"" AS ""SubPostTitle"", dsp.""PublishDate"",
           dp.""Id"" AS ""PostId"", dp.""HashId"" AS ""PostHashId"", dp.""Title"", 4 AS ""Type""
    FROM document.""DocumentSubPosts"" dsp
    JOIN document.""DocumentPosts"" dp ON dp.""Id"" = dsp.""PostId""
    WHERE dsp.""IsDelete"" = false AND dp.""IsDelete"" = false
    AND dsp.""PublishDate"" > TIMEZONE('UTC', now())
    AND dsp.""Status"" = ANY(@Status) AND dsp.""Permission"" <> 1
    AND dp.""Status"" = 1 AND dp.""Permission"" = 0
    ORDER BY dsp.""PublishDate"" ASC
    LIMIT @Take
)
SELECT ""Id"", ""HashId"", ""SubPostTitle"", ""PublishDate"", ""PostId"", ""PostHashId"", ""Title"", ""Type""
FROM (
    SELECT * FROM upcoming_story
    UNION ALL SELECT * FROM upcoming_comic
    UNION ALL SELECT * FROM upcoming_document
) u
ORDER BY ""PublishDate"" ASC
LIMIT @Take;";
        }
    }

    #endregion
}
