namespace Mcsg.Comic.Api.Services
{
    using Common.Core.Enums;

    public partial class CommentService
    {
        private string GetReactionByTargetIdsQuery = @"SELECT ""Type"", ""TargetId"", SUM(""Count"") AS ""Count"", SUM(""ReactByCurrent"") AS ""ReactByCurrent""
                                                        FROM (
                                                            SELECT ""Type"", ""TargetId"", COUNT(*) AS ""Count"", CASE
                                                                WHEN ""AuthorId"" = @UserId THEN 1
                                                                ELSE 0
                                                            END AS ""ReactByCurrent""
                                                            FROM {0}
                                                            WHERE ""TargetId"" = ANY(@TargetIds)
                                                            AND ""IsDelete"" = false
                                                            GROUP BY ""Type"", ""TargetId"", ""AuthorId""
                                                        ) react	
                                                        GROUP BY ""Type"", ""TargetId""
                                                        ORDER BY ""Count"" DESC;";

        private string GetReplyByCommentIdQuery = @"SELECT 
                                                pc.""CreatedBy"" as AuthorId,
                                                pc.""Body"",
                                                pc.""CustomNote"",
                                                pc.""Id"",
                                                pc.""CreatedOn"",
                                                pc.""ParentId"",
                                                pc.""PostId"",
                                                u.""Avatar"" as UserAvatar,
                                                u.""ProfileName"" as AuthorName,
                                                u.""ProfileId"",
                                                r.""Name"" as ResourceName,
                                                r.""Url"" as ResourceUrl,
                                                r.""HashId"" as ResourceHashId
                                               FROM {0} pc
                                               LEFT JOIN ""comic"".""ComicResources"" r on pc.""ResourceId"" = r.""Id""
                                               LEFT JOIN identity.""Users"" u on pc.""CreatedBy"" = u.""Id""
                                               WHERE pc.""ParentId"" = @CommentId
                                               AND pc.""IsDelete"" = false
                                               LIMIT @PageSize
                                               OFFSET @Offset;
                                                
                                               SELECT COUNT(pc.*)
                                               FROM {0} pc
                                               LEFT JOIN ""comic"".""ComicResources"" r on pc.""ResourceId"" = r.""Id""
                                               LEFT JOIN identity.""Users"" u on pc.""CreatedBy"" = u.""Id""
                                               WHERE pc.""ParentId"" = @CommentId
                                               AND pc.""IsDelete"" = false";
        private string GetCommentWithMostReactionQuery = $@"
                                                SELECT 
                                                    pc.""CreatedBy"" as AuthorId,
                                                    pc.""Id"",
                                                    pc.""Body"",
                                                    pc.""CustomNote"",
                                                    pc.""CreatedOn"",
                                                    pc.""GifId"",
                                                    pc.""PostId"",
                                                    pc.""ModifiedOn"",
                                                    p.""Title"",
                                                    NULL as Order,
                                                    u.""Avatar"" as UserAvatar,
                                                    u.""ProfileName"" as AuthorName,
                                                    u.""UserName"" as UserName,
                                                    u.""ProfileId"",
                                                    COUNT(reply.*) as ReplyCount, 
                                                    r.""Name"" as ResourceName,
                                                    r.""Url"" as ResourceUrl,
                                                    r.""HashId"" as ResourceHashId,
                                                    COALESCE(COUNT(pcr.""Id""), 0) AS reaction_count
                                                FROM ""comic"".""ComicPostComments""  pc
                                                LEFT JOIN ""comic"".""ComicPostComments"" reply on reply.""ParentId"" = pc.""Id"" AND reply.""IsDelete"" = false
                                                LEFT JOIN identity.""Users"" u on pc.""CreatedBy"" = u.""Id""
                                                LEFT JOIN ""comic"".""ComicPostCommentReactions"" pcr on pc.""Id"" = pcr.""TargetId""
                                                LEFT JOIN ""comic"".""ComicPosts""  p on pc.""PostId"" = p.""Id""                                                
                                                LEFT JOIN ""comic"".""ComicResources"" r on pc.""ResourceId"" = r.""Id""
                                                WHERE p.""HashId"" = @HashId and pc.""ParentId"" is null
                                                AND p.""IsDelete"" = false
                                                AND pc.""IsDelete"" = false
                                                GROUP BY pc.""CreatedBy"",pc.""Id"",pc.""CustomNote"",p.""Title"",u.""Avatar"",u.""ProfileName"",u.""UserName"",u.""ProfileId"",r.""Name"",r.""Url"",r.""HashId""
                                                UNION
                                                SELECT 
                                                    spc.""CreatedBy"" as AuthorId,
                                                    spc.""Id"",
                                                    spc.""Body"",
                                                    spc.""CustomNote"",
                                                    spc.""CreatedOn"",
                                                    spc.""GifId"",
                                                    spc.""PostId"",
                                                    spc.""ModifiedOn"",
                                                    sp.""Title"",
                                                    sp.""Order"",
                                                    u.""Avatar"",
                                                    u.""ProfileName"",
                                                    u.""UserName"" as UserName,
                                                    u.""ProfileId"",
                                                    COUNT(reply.*) ReplyCount,
                                                    r.""Name"" as ResourceName,
                                                    r.""Url"" as ResourceUrl,
                                                    r.""HashId"" as ResourceHashId,
                                                    COALESCE(COUNT(spcr.""Id""), 0) AS reaction_count
                                                FROM ""comic"".""ComicSubPostComments"" spc
                                                LEFT JOIN ""comic"".""ComicSubPostComments"" reply on reply.""ParentId"" = spc.""Id""
                                                LEFT JOIN identity.""Users"" u on spc.""CreatedBy"" = u.""Id""
                                                LEFT JOIN ""comic"".""ComicSubPostCommentReactions""  spcr ON spc.""Id"" = spcr.""TargetId""
                                                LEFT JOIN ""comic"".""ComicSubPosts""  sp ON spc.""PostId"" = sp.""Id""
                                                LEFT JOIN ""comic"".""ComicResources"" r on spc.""ResourceId"" = r.""Id""
                                                WHERE sp.""PostId"" = (SELECT ""Id"" FROM ""comic"".""ComicPosts""   WHERE ""HashId"" =@HashId) 
                                                AND spc.""ParentId"" is null
                                                AND spc.""IsDelete"" = false
                                                GROUP BY spc.""CreatedBy"", spc.""Id"",spc.""CustomNote"",sp.""Title"",sp.""Order"",u.""Avatar"",u.""ProfileName"",u.""UserName"",u.""ProfileId"",r.""Name"",r.""Url"",r.""HashId""
                                                ORDER BY reaction_count desc,
                                                ""CreatedOn"" desc
                                                OFFSET @Offset
                                                LIMIT @PageSize;

                                                SELECT
                                                    (SELECT COUNT(*)
                                                     FROM ""comic"".""ComicPostComments""  pc
                                                     JOIN ""comic"".""ComicPosts""  p ON pc.""PostId""= p.""Id"" 
                                                     WHERE p.""HashId"" = @HashId 
                                                    and ""ParentId"" is null 
                                                     AND pc.""IsDelete"" = false) 
                                                    +
                                                    (SELECT COUNT(*)
                                                     FROM ""comic"".""ComicSubPostComments"" spc
                                                     JOIN ""comic"".""ComicSubPosts"" sp ON spc.""PostId""= sp.""Id""
                                                     JOIN ""comic"".""ComicPosts""  p ON sp.""PostId""= p.""Id""
                                                     WHERE p.""HashId"" = @HashId and ""ParentId"" is null
                                                     AND spc.""IsDelete"" = false) AS total_comment_count";

        private string GetTotalPostCommentQuery => $@"SELECT COUNT(*)
                                                         FROM ""comic"".""ComicPostComments""  pc
                                                         JOIN ""comic"".""ComicPosts""  p ON pc.""PostId""= p.""Id""
                                                         WHERE p.""HashId"" = @HashId
                                                         AND pc.""IsDelete"" = false";

        private string GetTotalCommentQuery => $@"SELECT 
                                                        (SELECT COUNT(*)
                                                         FROM ""comic"".""ComicPostComments""  pc
                                                         JOIN ""comic"".""ComicPosts""  p ON pc.""PostId""= p.""Id""
                                                         WHERE p.""HashId"" = @HashId
                                                         AND pc.""IsDelete"" = false) 
                                                        +
                                                        (SELECT COUNT(*)
                                                         FROM ""comic"".""ComicSubPostComments"" spc
                                                         JOIN ""comic"".""ComicSubPosts"" sp ON spc.""PostId""= sp.""Id""
                                                         JOIN ""comic"".""ComicPosts""  p ON sp.""PostId""= p.""Id""
                                                         WHERE p.""HashId"" = @HashId
                                                         AND spc.""IsDelete"" = false) AS total_comment_count";
        private string GetCommentOfPostQuery
        {
            get
            {
                return @$"      WITH RECURSIVE cte AS (
                                    SELECT ""Id"", ""ParentId"", ""PostId"", ""AuthorId"", ""ModifiedOn"", ""Body"", ""CustomNote"" ,""ResourceId"", ""GifId"", ""IsDelete"", ""QuoteId"", 1 AS CommentLevel
                                    FROM {_postCommentRepository.TableName}
                                    WHERE ""ParentId"" IS NULL AND ""PostId"" = @PostId
                                    UNION ALL
                                    SELECT post.""Id"", post.""ParentId"", post.""PostId"", post.""AuthorId"", post.""ModifiedOn"", post.""Body"", post.""CustomNote"" ,post.""ResourceId"", post.""GifId"", post.""IsDelete"", post.""QuoteId"", ct.CommentLevel + 1
                                    FROM cte ct
                                    JOIN {_postCommentRepository.TableName} post ON post.""ParentId"" = ct.""Id""
                                    )
                                SELECT  cte.""Id"" , cte.""ParentId"", cte.""PostId"", cte.""Body"", cte.""CustomNote"" ,cte.""ModifiedOn"", cte.""AuthorId"", 
                                        (CASE WHEN us.""ProfileName"" IS NULL THEN us.""UserName""  ELSE us.""ProfileName"" END) AS AuthorName, us.""UserName"" ,us.""Avatar"" AS UserAvatar
                                        , cte.""ResourceId"", res.""HashId"" AS ResourceHashId, res.""Name"" AS ResourceName, res.""Url"" AS ResourceUrl, cte.""GifId"", cte.CommentLevel, cte.""QuoteId""
                                FROM cte
                                LEFT JOIN {_userRepository.TableName} us ON cte.""AuthorId"" = us.""Id""
                                LEFT JOIN {_resourceRepository.TableName} res ON cte.""ResourceId"" = res.""Id""
                                WHERE cte.""IsDelete"" = false
                                ORDER BY
                                    cte.CommentLevel,
                                    CASE 
                                        WHEN cte.CommentLevel = 1 THEN cte.""{{0}}""
                                        ELSE NULL
                                        END DESC; ";
            }
        }

        private string GetCommentByIdQuery
        {
            get
            {
                return @$"WITH cte AS (
                 SELECT ""Id"", ""ParentId"", ""PostId"", ""AuthorId"", ""ModifiedOn"", ""Body"", ""CustomNote"", ""ResourceId"", ""GifId"", ""IsDelete"", ""QuoteId""
                 FROM @CommentSource
                 WHERE ""ParentId"" IS NULL AND ""Id"" = @CommentId
             )
             SELECT
                 cte.""Id"",
                 cte.""ParentId"",
                 cte.""PostId"",
                 cte.""Body"",
                 cte.""CustomNote"",
                 cte.""ModifiedOn"",
                 cte.""AuthorId"",
                 (CASE WHEN us.""ProfileName"" IS NULL THEN us.""UserName"" ELSE us.""ProfileName"" END) AS AuthorName,
                 us.""UserName"",
                 us.""Avatar"" AS UserAvatar,
                 cte.""ResourceId"",
                 res.""HashId"" AS ResourceHashId,
                 res.""Name"" AS ResourceName,
                 res.""Url"" AS ResourceUrl,
                 cte.""GifId"",
                 cte.""QuoteId"",
                 (
                     SELECT COUNT(*)
                     FROM @CommentSource sub_comments
                     WHERE sub_comments.""ParentId"" = cte.""Id""
                 ) AS ""ReplyCount""
             FROM cte
             LEFT JOIN {_userRepository.TableName} us ON cte.""AuthorId"" = us.""Id""
             LEFT JOIN {_resourceRepository.TableName} res ON cte.""ResourceId"" = res.""Id""
             WHERE cte.""IsDelete"" = false;";
            }
        }

        private string GetCommentOfSubPostQuery
        {
            get
            {
                return @$"WITH RECURSIVE cte AS (
                                   SELECT ""Id"", ""ParentId"", ""PostId""
                                            , ""AuthorId"", ""ModifiedOn""
                                            , ""Body"", ""CustomNote"", ""QuoteId"", ""ResourceId"", ""GifId"", ""IsDelete"", 1 AS CommentLevel
                                   FROM {_subPostCommentRepository.TableName}
                                   WHERE ""ParentId"" IS NULL AND ""PostId"" = @PostId
                                   UNION ALL
                                   SELECT post.""Id"", post.""ParentId"", post.""PostId""
                                            , post.""AuthorId"", post.""ModifiedOn""
                                            , post.""Body"", post.""CustomNote"", post.""QuoteId"", post.""ResourceId"", post.""GifId"", post.""IsDelete"", ct.CommentLevel + 1
                                   FROM cte ct
                                   JOIN {_subPostCommentRepository.TableName} post ON post.""ParentId"" = ct.""Id""
                                )
                                SELECT cte.""Id"" , cte.""ParentId"", cte.""PostId"", cte.""Body"", cte.""CustomNote"", cte.""QuoteId"", cte.""ModifiedOn""
                                            , cte.""AuthorId"", (CASE WHEN us.""ProfileName"" IS NULL THEN us.""UserName""  ELSE us.""ProfileName"" END) AS AuthorName
                                            , us.""Avatar"" AS UserAvatar
                                            , cte.""ResourceId"", res.""HashId"" AS ResourceHashId, res.""Name"" AS ResourceName, res.""Url"" AS ResourceUrl, cte.""GifId""
                                            , cte.CommentLevel
                                FROM cte
                                LEFT JOIN {_userRepository.TableName} us ON cte.""AuthorId"" = us.""Id""
                                LEFT JOIN {_resourceRepository.TableName} res ON cte.""ResourceId"" = res.""Id""
                                WHERE cte.""IsDelete"" = false
                                ORDER BY cte.CommentLevel, cte.""{{0}}"" DESC";
            }
        }
        private string GetCommentByPostInHomePageQuery
        {
            get
            {
                return @$"SELECT com.""Id"", com.""PostId""
                                , comUser.""Id"" AS AuthorId
                                , (CASE WHEN comUser.""ProfileName"" IS NULL THEN comUser.""UserName""  ELSE comUser.""ProfileName"" END) AS AuthorName
                                , comUser.""Avatar"" AS UserAvatar
                                , com.""Body"", com.""ModifiedOn""
                                , res.""HashId"" AS ResourceHashId, res.""Name"" AS ResourceName, res.""Url"" AS ResourceUrl
                                , com.""GifId""
                                , rep.""Id"" AS ReplyId, (CASE WHEN repUser.""ProfileName"" IS NULL THEN repUser.""UserName""  ELSE repUser.""ProfileName"" END) AS ReplyAuthorName
                                , repUser.""Avatar"" AS ReplyUserAvatar    
                                , rep.""Body"" AS ReplyBody, rep.""ModifiedOn"" AS ReplyLastModifiedDate
                                , repRes.""HashId"" AS ReplyResourceHashId, repRes.""Name"" AS ReplyResourceName, repRes.""Url"" AS ReplyResourceUrl
                                , rep.""GifId"" AS ReplyGifId
                                , rep.""QuoteId"" AS ReplyQuoteId
                                , (SELECT COUNT(""Id"") AS TotalRecord FROM {_postCommentRepository.TableName}
                                        WHERE ""PostId"" = @PostId AND ""IsDelete"" = false) AS TotalRecord
                                FROM {_postCommentRepository.TableName} com
                                LEFT JOIN {_resourceRepository.TableName} res ON com.""ResourceId"" = res.""Id""
                                LEFT JOIN {_userRepository.TableName} comUser ON com.""AuthorId"" = comUser.""Id""
                                LEFT JOIN {_postCommentRepository.TableName} rep ON com.""Id"" = rep.""ParentId"" AND rep.""IsDelete"" = false 
                                LEFT JOIN {_resourceRepository.TableName} repRes ON rep.""ResourceId"" = repRes.""Id""
                                LEFT JOIN {_userRepository.TableName} repUser ON rep.""AuthorId"" = repUser.""Id""
                        WHERE com.""PostId"" = @PostId AND com.""ParentId"" IS NULL AND com.""IsDelete"" = false 
                        ORDER BY com.""ModifiedOn"" DESC
                        LIMIT 1 ";
            }
        }

        private string GetCommentBySubPostQuery
        {
            get
            {
                return @$"SELECT com.""Id"", com.""PostId""
                                , comUser.""Id"" AS AuthorId
                                , (CASE WHEN comUser.""ProfileName"" IS NULL THEN comUser.""UserName""  ELSE comUser.""ProfileName"" END) AS AuthorName
                                , comUser.""Avatar"" AS UserAvatar
                                , com.""Body"", com.""ModifiedOn""
                                , res.""HashId"" AS ResourceHashId, res.""Name"" AS ResourceName, res.""Url"" AS ResourceUrl
                                , com.""GifId""
                                , rep.""Id"" AS ReplyId, (CASE WHEN repUser.""ProfileName"" IS NULL THEN repUser.""UserName""  ELSE repUser.""ProfileName"" END) AS ReplyAuthorName
                                , repUser.""Avatar"" AS ReplyUserAvatar    
                                , rep.""Body"" AS ReplyBody, rep.""ModifiedOn"" AS ReplyLastModifiedDate
                                , repRes.""HashId"" AS ReplyResourceHashId, repRes.""Name"" AS ReplyResourceName, repRes.""Url"" AS ReplyResourceUrl
                                , rep.""GifId"" AS ReplyGifId
                                , (SELECT COUNT(""Id"") AS TotalRecord FROM {_subPostCommentRepository.TableName}
                                        WHERE ""PostId"" = @PostId AND ""IsDelete"" = false) AS TotalRecord
                                FROM {_subPostCommentRepository.TableName} com
                                LEFT JOIN {_resourceRepository.TableName} res ON com.""ResourceId"" = res.""Id""
                                LEFT JOIN {_userRepository.TableName} comUser ON com.""AuthorId"" = comUser.""Id""
                                LEFT JOIN {_subPostCommentRepository.TableName} rep ON com.""Id"" = rep.""ParentId"" AND rep.""IsDelete"" = false 
                                LEFT JOIN {_resourceRepository.TableName} repRes ON rep.""ResourceId"" = repRes.""Id""
                                LEFT JOIN {_userRepository.TableName} repUser ON rep.""AuthorId"" = repUser.""Id""
                        WHERE com.""PostId"" = @PostId AND com.""ParentId"" IS NULL AND com.""IsDelete"" = false 
                        ORDER BY com.""ModifiedOn"" DESC
                        LIMIT 1 ";
            }
        }

        private string GetCommentBySubPostInHomePageQuery
        {
            get
            {
                return @$"SELECT com.""Id"", com.""PostId""
                                , comUser.""Id"" AS AuthorId
                                , (CASE WHEN comUser.""ProfileName"" IS NULL THEN comUser.""UserName""  ELSE comUser.""ProfileName"" END) AS AuthorName
                                , comUser.""Avatar"" AS UserAvatar
                                , com.""Body"", com.""ModifiedOn""
                                , res.""HashId"" AS ResourceHashId, res.""Name"" AS ResourceName, res.""Url"" AS ResourceUrl
                                , com.""GifId""
                                , rep.""Id"" AS ReplyId, (CASE WHEN repUser.""ProfileName"" IS NULL THEN repUser.""UserName""  ELSE repUser.""ProfileName"" END) AS ReplyAuthorName
                                , repUser.""Avatar"" AS ReplyUserAvatar    
                                , rep.""Body"" AS ReplyBody, rep.""ModifiedOn"" AS ReplyLastModifiedDate
                                , repRes.""HashId"" AS ReplyResourceHashId, repRes.""Name"" AS ReplyResourceName, repRes.""Url"" AS ReplyResourceUrl
                                , rep.""GifId"" AS ReplyGifId
                                , (SELECT COUNT(""Id"") AS TotalRecord FROM {_subPostCommentRepository.TableName}
                                        WHERE ""PostId"" = @PostId AND ""IsDelete"" = false) AS TotalRecord
                                FROM {_subPostCommentRepository.TableName} com
                                LEFT JOIN {_resourceRepository.TableName} res ON com.""ResourceId"" = res.""Id""
                                LEFT JOIN {_userRepository.TableName} comUser ON com.""AuthorId"" = comUser.""Id""
                                LEFT JOIN {_subPostCommentRepository.TableName} rep ON com.""Id"" = rep.""ParentId"" AND rep.""IsDelete"" = false 
                                LEFT JOIN {_resourceRepository.TableName} repRes ON rep.""ResourceId"" = repRes.""Id""
                                LEFT JOIN {_userRepository.TableName} repUser ON rep.""AuthorId"" = repUser.""Id""
                        WHERE com.""PostId"" = @PostId AND com.""ParentId"" IS NULL AND com.""IsDelete"" = false 
                        ORDER BY com.""ModifiedOn"" DESC
                        LIMIT 1 ";
            }
        }

        private string CountSubPostOfPostQuery
        {
            get
            {
                return $@"SELECT T1.""PostId"", COUNT (DISTINCT T1.""Id"") AS ""Count"" 
                        FROM {_subPostRepository.TableName} T1
                        LEFT JOIN {_subPostRepository.TableName} T2
                        ON T1.""PostId"" = T2.""PostId""
                        WHERE T2.""Id"" = @PostId AND T2.""IsDelete"" = false 
                        GROUP BY T1.""PostId"" ";
            }
        }
        private string GetUserMentionsInComments
        {
            get
            {
                return $@"SELECT men.""Id""
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
        }
    }
}
