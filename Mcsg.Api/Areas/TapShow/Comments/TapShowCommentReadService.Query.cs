namespace Mcsg.Api.Areas.TapShow.Comments;

using Common.Core.Enums;

/// <summary>
/// SQL cloned from Areas/Story/Services/CommentService.Query.cs (post comments only), pointed at the tapshow schema
/// </summary>
public partial class TapShowCommentReadService
{
    private const string CommentTable = @"tapshow.""TapShowPostComments""";
    private const string CommentReactionTable = @"tapshow.""TapShowPostCommentReactions""";
    private const string PostTable = @"tapshow.""TapShowPosts""";
    private const string ResourceTable = @"tapshow.""TapShowResources""";

    public static string GetReactionByTargetIdsQuery = @"SELECT ""Type"", ""TargetId"", SUM(""Count"") AS ""Count"", SUM(""ReactByCurrent"") AS ""ReactByCurrent""
                                                        FROM (
                                                            SELECT r.""Type"", r.""TargetId"", COUNT(*) AS ""Count"", CASE
                                                                WHEN r.""AuthorId"" = @UserId THEN 1
                                                                ELSE 0
                                                            END AS ""ReactByCurrent""
                                                            FROM {0} r
                                                            INNER JOIN ""identity"".""Users"" u ON r.""AuthorId"" = u.""Id""
                                                            WHERE ""TargetId"" = ANY(@TargetIds)
                                                            AND r.""IsDelete"" = false
                                                            GROUP BY r.""Type"", r.""TargetId"", r.""AuthorId""
                                                        ) react
                                                        GROUP BY ""Type"", ""TargetId""
                                                        ORDER BY ""Count"" DESC;";

    private static readonly string GetReplyByCommentIdQuery = $@"SELECT 
                                                pc.""CreatedBy"" as AuthorId,
                                                pc.""Body"",
                                                pc.""CustomNote"",
                                                pc.""Id"",
                                                pc.""CreatedOn"",
                                                pc.""ParentId"",
                                                pc.""PostId"",
                                                pc.""GifId"",
                                                pc.""QuoteId"",
                                                u.""Avatar"" as UserAvatar,
                                                u.""ProfileName"" as AuthorName,
                                                u.""ProfileId"",
                                                u.""UserName"",
                                                r.""Name"" as ResourceName,
                                                r.""Url"" as ResourceUrl,
                                                r.""MinioInstance"",
                                                r.""HashId"" as ResourceHashId
                                               FROM {CommentTable} pc
                                               LEFT JOIN {ResourceTable} r on pc.""ResourceId"" = r.""Id""
                                               LEFT JOIN identity.""Users"" u on pc.""CreatedBy"" = u.""Id"" 
                                               WHERE pc.""ParentId"" = @CommentId AND u.""IsDelete"" = false
                                               AND pc.""IsDelete"" = false
                                               ORDER BY pc.""CreatedOn"" 
                                               LIMIT @PageSize
                                               OFFSET @Offset;
                                                
                                               SELECT COUNT(pc.*)
                                               FROM {CommentTable} pc
                                               LEFT JOIN {ResourceTable} r on pc.""ResourceId"" = r.""Id""
                                               LEFT JOIN identity.""Users"" u on pc.""CreatedBy"" = u.""Id"" 
                                               WHERE pc.""ParentId"" = @CommentId 
                                               AND pc.""IsDelete"" = false AND u.""IsDelete"" = false";

    private static readonly string GetCommentWithMostReactionQuery = $@"
                                                SELECT 
                                                    pc.""CreatedBy"" as AuthorId,
                                                    pc.""Id"",
                                                    pc.""Body"",
                                                    pc.""CustomNote"",
                                                    pc.""CreatedOn"",
                                                    pc.""GifId"",
                                                    pc.""PostId"",
                                                    p.""HashId"" as PostHashId,
                                                    p.""Title"",
                                                    NULL as Order,
                                                    u.""Avatar"" as UserAvatar,
                                                    u.""ProfileName"" as AuthorName,
                                                    u.""UserName"" as UserName,
                                                    u.""ProfileId"",
                                                    (SELECT COUNT(*) 
                                                     FROM {CommentTable} reply 
                                                     LEFT JOIN identity.""Users"" ur on reply.""CreatedBy"" = ur.""Id""
                                                     WHERE reply.""ParentId"" = pc.""Id"" 
                                                     AND reply.""IsDelete"" = false
                                                     AND ur.""IsDelete"" = false) AS ReplyCount,
                                                    r.""Name"" as ResourceName,
                                                    r.""Url"" as ResourceUrl,
                                                    r.""MinioInstance"",
                                                    r.""HashId"" as ResourceHashId,
                                                    COALESCE(COUNT(pcr.""Id""), 0) AS reaction_count
                                                FROM {CommentTable} pc
                                                INNER JOIN identity.""Users"" u on pc.""AuthorId"" = u.""Id"" 
                                                LEFT JOIN {CommentReactionTable} pcr on pc.""Id"" = pcr.""TargetId"" AND pcr.""IsDelete"" = false
                                                LEFT JOIN {PostTable} p on pc.""PostId"" = p.""Id""
                                                LEFT JOIN {ResourceTable} r on pc.""ResourceId"" = r.""Id""
                                                WHERE p.""HashId"" = @HashId and pc.""ParentId"" is null AND u.""IsDelete"" = false
                                                AND p.""IsDelete"" = false 
                                                AND pc.""IsDelete"" = false
                                                GROUP BY pc.""CreatedBy"",pc.""Id"",pc.""CustomNote"",p.""HashId"",p.""Title"",u.""Avatar"",u.""ProfileName"",u.""UserName"",u.""ProfileId"",r.""Name"",r.""Url"",r.""MinioInstance"",r.""HashId""
                                                ORDER BY reaction_count desc,
                                                ""CreatedOn"" desc
                                                OFFSET @Offset
                                                LIMIT @PageSize;

                                                SELECT COUNT(*)
                                                FROM {CommentTable} pc
                                                JOIN {PostTable} p ON pc.""PostId""= p.""Id"" 
                                                LEFT JOIN identity.""Users"" u on p.""UserId"" = u.""Id""
                                                WHERE p.""HashId"" = @HashId AND u.""IsDelete"" = false 
                                                and ""ParentId"" is null 
                                                AND pc.""IsDelete"" = false;";

    /// <summary>
    /// Same selection as GetCommentWithMostReactionQuery, top @Take root comments for each post of a list page
    /// </summary>
    private static readonly string GetMostReactionCommentsOfPostsQuery = $@"
                                                SELECT * FROM (
                                                    SELECT 
                                                        pc.""CreatedBy"" as AuthorId,
                                                        pc.""Id"",
                                                        pc.""Body"",
                                                        pc.""CustomNote"",
                                                        pc.""CreatedOn"",
                                                        pc.""GifId"",
                                                        pc.""PostId"",
                                                        p.""HashId"" as PostHashId,
                                                        p.""Title"",
                                                        NULL as Order,
                                                        u.""Avatar"" as UserAvatar,
                                                        u.""ProfileName"" as AuthorName,
                                                        u.""UserName"" as UserName,
                                                        u.""ProfileId"",
                                                        (SELECT COUNT(*) 
                                                         FROM {CommentTable} reply 
                                                         LEFT JOIN identity.""Users"" ur on reply.""CreatedBy"" = ur.""Id""
                                                         WHERE reply.""ParentId"" = pc.""Id"" 
                                                         AND reply.""IsDelete"" = false
                                                         AND ur.""IsDelete"" = false) AS ReplyCount,
                                                        r.""Name"" as ResourceName,
                                                        r.""Url"" as ResourceUrl,
                                                        r.""MinioInstance"",
                                                        r.""HashId"" as ResourceHashId,
                                                        ROW_NUMBER() OVER (PARTITION BY pc.""PostId"" ORDER BY COUNT(pcr.""Id"") DESC, pc.""CreatedOn"" DESC) AS rn
                                                    FROM {CommentTable} pc
                                                    INNER JOIN identity.""Users"" u on pc.""AuthorId"" = u.""Id"" 
                                                    LEFT JOIN {CommentReactionTable} pcr on pc.""Id"" = pcr.""TargetId"" AND pcr.""IsDelete"" = false
                                                    LEFT JOIN {PostTable} p on pc.""PostId"" = p.""Id""
                                                    LEFT JOIN {ResourceTable} r on pc.""ResourceId"" = r.""Id""
                                                    WHERE pc.""PostId"" = ANY(@PostIds) and pc.""ParentId"" is null AND u.""IsDelete"" = false
                                                    AND p.""IsDelete"" = false 
                                                    AND pc.""IsDelete"" = false
                                                    GROUP BY pc.""CreatedBy"",pc.""Id"",pc.""CustomNote"",p.""HashId"",p.""Title"",u.""Avatar"",u.""ProfileName"",u.""UserName"",u.""ProfileId"",r.""Name"",r.""Url"",r.""MinioInstance"",r.""HashId""
                                                ) ranked
                                                WHERE rn <= @Take
                                                ORDER BY ""PostId"", rn;";

    private static readonly string GetTotalCommentQuery = $@"SELECT (SELECT COUNT(*)
                                                         FROM {CommentTable} pc
                                                         JOIN {PostTable} p ON pc.""PostId""= p.""Id""
                                                         INNER JOIN identity.""Users"" u on pc.""AuthorId"" = u.""Id"" AND u.""IsDelete"" = false
                                                         WHERE p.""HashId"" = @HashId 
                                                         AND pc.""IsDelete"" = false
                                                         AND (
                                                           pc.""ParentId"" IS NULL
                                                             OR (
                                                               pc.""ParentId"" IS NOT NULL 
                                                               AND pc.""IsDelete"" = false
                                                               AND EXISTS (
                                                                 SELECT *
                                                                 FROM {CommentTable} pc1
                                                                 INNER JOIN ""identity"".""Users"" comUser ON pc1.""AuthorId"" = comUser.""Id"" 
                                                                 WHERE pc1.""Id"" = pc.""ParentId""
                                                                 AND comUser.""IsDelete"" = false
                                                                  )
                                                               )
                                                            )) AS total_comment_count";

    private static string GetCommentOfPostQuery => $@"      WITH RECURSIVE cte AS (
                                    SELECT ""Id"", ""ParentId"", ""PostId"", ""AuthorId"", ""CreatedOn"", ""ModifiedOn"", ""Body"", ""CustomNote"" ,""ResourceId"", ""GifId"", ""IsDelete"", ""QuoteId"", 1 AS CommentLevel
                                    FROM {CommentTable}
                                    WHERE ""ParentId"" IS NULL AND ""PostId"" = @PostId
                                    UNION ALL
                                    SELECT post.""Id"", post.""ParentId"", post.""PostId"", post.""AuthorId"", post.""CreatedOn"", post.""ModifiedOn"", post.""Body"", post.""CustomNote"" ,post.""ResourceId"", post.""GifId"", post.""IsDelete"", post.""QuoteId"", ct.CommentLevel + 1
                                    FROM cte ct
                                    JOIN {CommentTable} post ON post.""ParentId"" = ct.""Id""
                                    INNER JOIN identity.""Users"" parentUser ON ct.""AuthorId"" = parentUser.""Id"" AND parentUser.""IsDelete"" = false
                                    )
                                SELECT  cte.""Id"" , cte.""ParentId"", cte.""PostId"", cte.""Body"", cte.""CustomNote"" ,cte.""CreatedOn"", cte.""AuthorId"", 
                                        (CASE WHEN us.""ProfileName"" IS NULL THEN us.""UserName""  ELSE us.""ProfileName"" END) AS AuthorName, us.""UserName"" ,us.""Avatar"" AS UserAvatar
                                        , cte.""ResourceId"", res.""HashId"" AS ResourceHashId, res.""Name"" AS ResourceName, res.""Url"" AS ResourceUrl, res.""MinioInstance"", cte.""GifId"", cte.CommentLevel, cte.""QuoteId""
                                FROM cte
                                LEFT JOIN identity.""Users"" us ON cte.""AuthorId"" = us.""Id""
                                LEFT JOIN {ResourceTable} res ON cte.""ResourceId"" = res.""Id""
                                WHERE cte.""IsDelete"" = false AND us.""IsDelete"" = false
                                ORDER BY
                                    cte.CommentLevel,
                                    CASE 
                                        WHEN cte.CommentLevel = 1 THEN cte.""{{0}}""
                                        ELSE NULL
                                        END DESC; ";

    private static string GetCommentByIdQuery => $@"WITH cte AS (
                 SELECT ""Id"", ""ParentId"", ""PostId"", ""AuthorId"", ""CreatedOn"", ""Body"", ""CustomNote"", ""ResourceId"", ""GifId"", ""IsDelete"", ""QuoteId""
                 FROM {CommentTable}
                 WHERE ""Id"" = @CommentId
             )
             SELECT
                 cte.""Id"",
                 cte.""ParentId"",
                 cte.""PostId"",
                 cte.""Body"",
                 cte.""CustomNote"",
                 cte.""CreatedOn"",
                 cte.""AuthorId"",
                 (CASE WHEN us.""ProfileName"" IS NULL THEN us.""UserName"" ELSE us.""ProfileName"" END) AS AuthorName,
                 us.""UserName"",
                 us.""Avatar"" AS UserAvatar,
                 cte.""ResourceId"",
                 res.""HashId"" AS ResourceHashId,
                 res.""Name"" AS ResourceName,
                 res.""Url"" AS ResourceUrl,
                 res.""MinioInstance"",
                 cte.""GifId"",
                 cte.""QuoteId"",
                 (
                     SELECT COUNT(*)
                     FROM {CommentTable} sub_comments
                     WHERE sub_comments.""ParentId"" = cte.""Id""
                     AND sub_comments.""IsDelete"" = false
                 ) AS ""ReplyCount""
             FROM cte
             LEFT JOIN identity.""Users"" us ON cte.""AuthorId"" = us.""Id""
             LEFT JOIN {ResourceTable} res ON cte.""ResourceId"" = res.""Id""
             WHERE cte.""IsDelete"" = false AND us.""IsDelete"" = false;";

    private static string GetCommentByPostInHomePageQuery => $@"SELECT com.""Id"", com.""PostId""
                                , comUser.""Id"" AS AuthorId
                                , (CASE WHEN comUser.""ProfileName"" IS NULL THEN comUser.""UserName""  ELSE comUser.""ProfileName"" END) AS AuthorName
                                , comUser.""Avatar"" AS UserAvatar
                                , com.""Body"", com.""CreatedOn""
                                , res.""HashId"" AS ResourceHashId, res.""Name"" AS ResourceName, res.""Url"" AS ResourceUrl, res.""MinioInstance""
                                , com.""GifId""
                                , rep.""Id"" AS ReplyId, (CASE WHEN repUser.""ProfileName"" IS NULL THEN repUser.""UserName""  ELSE repUser.""ProfileName"" END) AS ReplyAuthorName
                                , repUser.""Avatar"" AS ReplyUserAvatar    
                                , rep.""Body"" AS ReplyBody, rep.""CreatedOn"" AS ReplyLastCreatedDate
                                , repRes.""HashId"" AS ReplyResourceHashId, repRes.""Name"" AS ReplyResourceName, repRes.""Url"" AS ReplyResourceUrl
                                , rep.""GifId"" AS ReplyGifId
                                , rep.""QuoteId"" AS ReplyQuoteId
                                , (SELECT COUNT(""Id"") AS TotalRecord FROM {CommentTable}
                                        WHERE ""PostId"" = @PostId AND ""IsDelete"" = false) AS TotalRecord
                                FROM {CommentTable} com
                                LEFT JOIN {ResourceTable} res ON com.""ResourceId"" = res.""Id""
                                LEFT JOIN identity.""Users"" comUser ON com.""AuthorId"" = comUser.""Id""
                                LEFT JOIN {CommentTable} rep ON com.""Id"" = rep.""ParentId"" AND rep.""IsDelete"" = false 
                                LEFT JOIN {ResourceTable} repRes ON rep.""ResourceId"" = repRes.""Id""
                                LEFT JOIN identity.""Users"" repUser ON rep.""AuthorId"" = repUser.""Id""
                        WHERE com.""PostId"" = @PostId AND com.""ParentId"" IS NULL AND com.""IsDelete"" = false AND comUser.""IsDelete"" = false
                        ORDER BY com.""CreatedOn"" DESC
                        LIMIT 1 ";

    private static string GetUserMentionsInComments => $@"SELECT men.""Id""
                        , men.""LocationId"", men.""LocationType""
                        , men.""EntityId"", men.""EntityType""
                        , (CASE WHEN use.""ProfileName"" IS NULL THEN use.""UserName"" ELSE use.""ProfileName"" END) AS ProfileName
                        , use.""UserName"" as UserName
                        , men.""Length"", men.""Offset"", men.""Text""
                        FROM public.""Mentions"" men 
                        LEFT JOIN identity.""Users"" use ON men.""EntityId"" = use.""Id"" 
                                                        AND men.""EntityType"" = {(int)EntityType.User}
                        WHERE men.""EntityType"" = {(int)EntityType.User} 
                                AND men.""LocationId"" = ANY(@LocationIds) AND men.""IsDelete"" = false";
}
