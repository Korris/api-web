using Dapper;

namespace Mcsg.Api.Areas.Social.Services;

using Common.Core.Constants;
using Common.SeedWork.Exceptions;
using Mcsg.Api.Areas.Social.Models;
using static Common.Core.Constants.Setting;

/// <summary>
/// "Posts I am commenting on": the posts a given user has commented on, ordered by the newest comment on them,
/// across feed, story, comic and document. Own partial file so PostService.cs / PostService.Query.cs do not grow.
/// </summary>
public partial class PostService
{
    #region -- Methods --

    /// <summary>
    /// Returns the posts <paramref name="userId"/> has commented on, most recently active first.
    /// NewComments = comments other people wrote after this user's last comment on that post, i.e. what they
    /// have not seen yet. Chapter comments are attributed to the parent series so a series is one card.
    /// </summary>
    /// <param name="userId">Whose comment history to use</param>
    /// <param name="take">Number of cards; clamped to 1..MaxActiveCommentPosts</param>
    public async Task<List<ActiveCommentPostResponse>> GetActiveCommentPosts(Guid userId, int take)
    {
        try
        {
            var param = new
            {
                UserId = userId,
                Take = Math.Clamp(take, 1, MaxActiveCommentPosts)
            };

            var rows = await _postCommentRepository.Connection.QueryAsync<ActiveCommentPostResponse>(GetActiveCommentPostsQuery, param);
            return rows.ToList();
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    #endregion

    #region -- Queries --

    private const int MaxActiveCommentPosts = 50;

    /// <summary>
    /// Step 1 (my_posts): posts the user commented on, from the 7 comment tables (post-level for all four types,
    /// chapter-level for story/comic/document mapped to the parent post), with the user's latest comment time.
    /// Social sub post comments are left out on purpose: those belong to shared/quoted feed items, not to a series.
    /// Step 2 (all_comments): every public comment on those posts only, so no full comment-table scan.
    /// Step 3 (agg): totals, "new since my last comment", last activity; keep the @Take most recently active.
    /// Step 4: newest comment per post with its author, and the post row for title/hashId.
    /// </summary>
    private string GetActiveCommentPostsQuery
    {
        get
        {
            return @"
WITH my_comments AS (
    SELECT c.""PostId"", 0 AS ""Type"", c.""CreatedOn""
    FROM social.""SocialPostComments"" c
    WHERE c.""AuthorId"" = @UserId AND c.""IsDelete"" = false AND c.""Status"" = 1
    UNION ALL
    SELECT c.""PostId"", 1, c.""CreatedOn""
    FROM story.""StoryPostComments"" c
    WHERE c.""AuthorId"" = @UserId AND c.""IsDelete"" = false AND c.""Status"" = 1
    UNION ALL
    SELECT sp.""PostId"", 1, c.""CreatedOn""
    FROM story.""StorySubPostComments"" c
    JOIN story.""StorySubPosts"" sp ON sp.""Id"" = c.""PostId"" AND sp.""IsDelete"" = false
    WHERE c.""AuthorId"" = @UserId AND c.""IsDelete"" = false AND c.""Status"" = 1
    UNION ALL
    SELECT c.""PostId"", 2, c.""CreatedOn""
    FROM comic.""ComicPostComments"" c
    WHERE c.""AuthorId"" = @UserId AND c.""IsDelete"" = false AND c.""Status"" = 1
    UNION ALL
    SELECT sp.""PostId"", 2, c.""CreatedOn""
    FROM comic.""ComicSubPostComments"" c
    JOIN comic.""ComicSubPosts"" sp ON sp.""Id"" = c.""PostId"" AND sp.""IsDelete"" = false
    WHERE c.""AuthorId"" = @UserId AND c.""IsDelete"" = false AND c.""Status"" = 1
    UNION ALL
    SELECT c.""PostId"", 4, c.""CreatedOn""
    FROM document.""DocumentPostComments"" c
    WHERE c.""AuthorId"" = @UserId AND c.""IsDelete"" = false AND c.""Status"" = 1
    UNION ALL
    SELECT sp.""PostId"", 4, c.""CreatedOn""
    FROM document.""DocumentSubPostComments"" c
    JOIN document.""DocumentSubPosts"" sp ON sp.""Id"" = c.""PostId"" AND sp.""IsDelete"" = false
    WHERE c.""AuthorId"" = @UserId AND c.""IsDelete"" = false AND c.""Status"" = 1
),
my_posts AS (
    SELECT ""PostId"", ""Type"", MAX(""CreatedOn"") AS ""MyLastCommentOn""
    FROM my_comments
    GROUP BY ""PostId"", ""Type""
),
all_comments AS (
    SELECT c.""PostId"", c.""AuthorId"", c.""Body"", c.""CreatedOn""
    FROM social.""SocialPostComments"" c
    JOIN my_posts mp ON mp.""PostId"" = c.""PostId"" AND mp.""Type"" = 0
    WHERE c.""IsDelete"" = false AND c.""Status"" = 1
    UNION ALL
    SELECT c.""PostId"", c.""AuthorId"", c.""Body"", c.""CreatedOn""
    FROM story.""StoryPostComments"" c
    JOIN my_posts mp ON mp.""PostId"" = c.""PostId"" AND mp.""Type"" = 1
    WHERE c.""IsDelete"" = false AND c.""Status"" = 1
    UNION ALL
    SELECT sp.""PostId"", c.""AuthorId"", c.""Body"", c.""CreatedOn""
    FROM story.""StorySubPosts"" sp
    JOIN my_posts mp ON mp.""PostId"" = sp.""PostId"" AND mp.""Type"" = 1
    JOIN story.""StorySubPostComments"" c ON c.""PostId"" = sp.""Id""
    WHERE sp.""IsDelete"" = false AND c.""IsDelete"" = false AND c.""Status"" = 1
    UNION ALL
    SELECT c.""PostId"", c.""AuthorId"", c.""Body"", c.""CreatedOn""
    FROM comic.""ComicPostComments"" c
    JOIN my_posts mp ON mp.""PostId"" = c.""PostId"" AND mp.""Type"" = 2
    WHERE c.""IsDelete"" = false AND c.""Status"" = 1
    UNION ALL
    SELECT sp.""PostId"", c.""AuthorId"", c.""Body"", c.""CreatedOn""
    FROM comic.""ComicSubPosts"" sp
    JOIN my_posts mp ON mp.""PostId"" = sp.""PostId"" AND mp.""Type"" = 2
    JOIN comic.""ComicSubPostComments"" c ON c.""PostId"" = sp.""Id""
    WHERE sp.""IsDelete"" = false AND c.""IsDelete"" = false AND c.""Status"" = 1
    UNION ALL
    SELECT c.""PostId"", c.""AuthorId"", c.""Body"", c.""CreatedOn""
    FROM document.""DocumentPostComments"" c
    JOIN my_posts mp ON mp.""PostId"" = c.""PostId"" AND mp.""Type"" = 4
    WHERE c.""IsDelete"" = false AND c.""Status"" = 1
    UNION ALL
    SELECT sp.""PostId"", c.""AuthorId"", c.""Body"", c.""CreatedOn""
    FROM document.""DocumentSubPosts"" sp
    JOIN my_posts mp ON mp.""PostId"" = sp.""PostId"" AND mp.""Type"" = 4
    JOIN document.""DocumentSubPostComments"" c ON c.""PostId"" = sp.""Id""
    WHERE sp.""IsDelete"" = false AND c.""IsDelete"" = false AND c.""Status"" = 1
),
agg AS (
    SELECT mp.""PostId"", mp.""Type"",
           COUNT(*) AS ""TotalComments"",
           COUNT(*) FILTER (WHERE ac.""CreatedOn"" > mp.""MyLastCommentOn"" AND ac.""AuthorId"" <> @UserId) AS ""NewComments"",
           MAX(ac.""CreatedOn"") AS ""LastCommentOn""
    FROM my_posts mp
    JOIN all_comments ac ON ac.""PostId"" = mp.""PostId""
    GROUP BY mp.""PostId"", mp.""Type""
    ORDER BY ""LastCommentOn"" DESC
    LIMIT @Take
),
latest_comment AS (
    SELECT DISTINCT ON (ac.""PostId"") ac.""PostId"", ac.""AuthorId"", ac.""Body"", ac.""CreatedOn""
    FROM all_comments ac
    JOIN agg a ON a.""PostId"" = ac.""PostId""
    ORDER BY ac.""PostId"", ac.""CreatedOn"" DESC
),
posts AS (
    SELECT ""Id"", ""HashId"", ""Title"", ""Body"", ""Status"", ""IsDelete"" FROM social.""SocialPosts""
    WHERE ""Id"" IN (SELECT ""PostId"" FROM agg WHERE ""Type"" = 0)
    UNION ALL
    SELECT ""Id"", ""HashId"", ""Title"", ""Body"", ""Status"", ""IsDelete"" FROM story.""StoryPosts""
    WHERE ""Id"" IN (SELECT ""PostId"" FROM agg WHERE ""Type"" = 1)
    UNION ALL
    SELECT ""Id"", ""HashId"", ""Title"", ""Body"", ""Status"", ""IsDelete"" FROM comic.""ComicPosts""
    WHERE ""Id"" IN (SELECT ""PostId"" FROM agg WHERE ""Type"" = 2)
    UNION ALL
    SELECT ""Id"", ""HashId"", ""Title"", ""Body"", ""Status"", ""IsDelete"" FROM document.""DocumentPosts""
    WHERE ""Id"" IN (SELECT ""PostId"" FROM agg WHERE ""Type"" = 4)
)
SELECT a.""PostId"", a.""Type"", p.""HashId"",
       COALESCE(NULLIF(p.""Title"", ''), LEFT(p.""Body"", 100)) AS ""Title"",
       a.""TotalComments"", a.""NewComments"", a.""LastCommentOn"",
       lc.""Body"" AS ""LastCommentBody"",
       u.""UserName"" AS ""LastCommentUserName"",
       u.""ProfileName"" AS ""LastCommentProfileName"",
       u.""Avatar"" AS ""LastCommentAvatar""
FROM agg a
JOIN posts p ON p.""Id"" = a.""PostId"" AND p.""IsDelete"" = false AND p.""Status"" = 1
LEFT JOIN latest_comment lc ON lc.""PostId"" = a.""PostId""
LEFT JOIN identity.""Users"" u ON u.""Id"" = lc.""AuthorId""
ORDER BY a.""LastCommentOn"" DESC;";
        }
    }

    #endregion
}
