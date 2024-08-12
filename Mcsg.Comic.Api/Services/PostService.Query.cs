namespace Mcsg.Comic.Api.Services
{
    using Common.Core.Enums;

    public partial class PostService
    {
        private string GetTotalCommentQuery => $@"SELECT 
                                                        (SELECT COUNT(*)
                                                         FROM ""comic"".""ComicPostComments""  pc
                                                         JOIN ""comic"".""ComicPosts""  p ON pc.""PostId""= p.""Id""
                                                         WHERE p.""HashId"" = @HashId) 
                                                        +
                                                        (SELECT COUNT(*)
                                                         FROM ""comic"".""ComicSubPostComments"" spc
                                                         JOIN ""comic"".""ComicSubPosts"" sp ON spc.""PostId""= sp.""Id""
                                                         JOIN ""comic"".""ComicPosts""  p ON sp.""PostId""= p.""Id""
                                                         WHERE p.""HashId"" = @HashId) AS total_comment_count";
        private string GetSeriesQuery
        {
            get
            {
                return @"SELECT 
                        p.""Id"", 
                        p.""Title"", 
                        p.""Body"",
                        p.""HashId"", 
                        p.""AuthorId"",
                        p.""AuthorName"",
                        p.""UserId"",
                        p.""ThumbnailUrl"",
                        p.""CoverUrl"",
                        p.""Permission"",
                        p.""IsMature"",
                        p.""IsCompleted"",
                        postview.""ViewCount"",
                        u.""ProfileName"", 
                        u.""UserName"",
                        u.""ProfileId"",
                        u.""Avatar"" as ""UserAvatar"",
                        p.""Status"", p.""Type"", 
                        array_agg(tag.""Name"") as Tags,
                        p.""CreatedOn"",
                        sp.""Id"", 
                        sp.""HashId"",sp.""IsExclusive"",
                        sp.""Title"",
                        sp.""Order"",
                        sp.""Status"",
                        sp.""IsPremium"",
                        sp.""Permission"",
                        COUNT(DISTINCT spcm.""Id"") as ""CommentCount"",
                        subpostview.""ViewCount"",
                        sp.""CreatedOn"",
                        sp.""PublishDate"",
                        ux.""Id"" as ""UserExclusiveId"" ,
                        sp.""CreatorNote"",
                        sp.""IsEnableComment""
                        FROM ""comic"".""ComicPosts""  p
                        LEFT JOIN identity.""Users"" u ON p.""UserId"" = u.""Id""
                        LEFT JOIN ""comic"".""ComicTagPosts"" tp ON tp.""PostId"" = p.""Id"" AND tp.""IsDelete"" = false
                        LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id""
                        LEFT JOIN ""comic"".""ComicSubPosts"" sp ON sp.""PostId"" = p.""Id"" AND sp.""IsDelete"" = false [WithPermission]  [Not-load-chapter]    
                        LEFT JOIN ""UserExclusiveSubPosts"" ux ON ux.""SubPostId"" = sp.""Id"" AND ux.""UserId"" = @UserId
                        LEFT JOIN ""comic"".""ComicSubPostComments"" spcm ON spcm.""PostId"" = sp.""Id"" AND spcm.""IsDelete"" = false
                        --post view count
                        LEFT JOIN LATERAL (
                                SELECT 
                                ""EntityId"", 
                                ""Count"" as ""ViewCount""
                                    FROM ""SmartCountActions"" 
                                WHERE ""EntityId"" = p.""Id"" AND ""EntityType"" = 0 AND ""ActionType"" = 2
LIMIT 1
                                ) postview ON postview.""EntityId"" = p.""Id""
--subpost view count
LEFT JOIN LATERAL (
                                SELECT 
                                ""EntityId"", 
                                ""Count"" as ""ViewCount""
                                    FROM ""SmartCountActions"" 
                                WHERE ""EntityId"" = sp.""Id"" AND ""EntityType"" = 1 AND ""ActionType"" = 2
LIMIT 1
                                ) subpostview ON subpostview.""EntityId"" = sp.""Id""
                        WHERE 
                        p.""HashId"" = @HashId AND p.""IsDelete"" = false
                        -- TODO AND (@IsAccessPrivate = true OR p.""IsPrivate"" = false )
                        GROUP BY p.""Id"",p.""Title"", p.""Body"", p.""HashId"", p.""Permission"",p.""UserId"",
                        p.""IsMature"",p.""IsCompleted"",postview.""ViewCount"",
                        p.""AuthorId"",p.""AuthorName"",u.""ProfileName"", u.""UserName"" ,u.""ProfileId"",u.""Avatar"", p.""CreatedOn"",
                        p.""Status"", p.""Type"", p.""CreatedOn"",sp.""Id"",sp.""HashId"",sp.""Title"",sp.""Order"", sp.""Status"", sp.""IsPremium"" ,ux.""Id"",
                        sp.""Permission"",subpostview.""ViewCount"",
                        sp.""PublishDate""
                        ORDER BY sp.""Order""
                        ";
            }
        }

        private string GetPostWithHashId

        {
            get
            {
                return @"
                    SELECT ""Id"", ""Title"", ""HashId"", ""Type"", ""Body"", ""Status"", ""CreatedOn"", ""CreatedBy"", ""ModifiedOn"", ""ModifiedBy"", ""IsDelete"", ""Permission"", ""ThumbnailUrl"", ""AuthorName"", ""CoverUrl"", ""IsMature"",""IsCompleted"", ""ViewCount"", ""AuthorId"", ""UserId""
                    FROM ""comic"".""ComicPosts"" 
                    WHERE ""HashId"" = @HashId;";
            }
        }

        private string GetPostAndLastSubPostOrder

        {
            get
            {
                return @"
                    SELECT ""Id"", ""Title"", ""HashId"", ""Type"", ""Body"", ""Status"", ""CreatedOn"", ""CreatedBy"", ""ModifiedOn"", ""ModifiedBy"", ""IsDelete"", ""Permission"", ""ThumbnailUrl"", ""AuthorName"", ""CoverUrl"", ""IsMature"", ""IsCompleted"", ""ViewCount"", ""AuthorId"", ""UserId""
                    FROM ""comic"".""ComicPosts"" 
                    WHERE ""HashId"" = @HashId;

                    SELECT MAX(""Order"")
                    FROM ""comic"".""ComicSubPosts"" sp
                    INNER JOIN ""comic"".""ComicPosts""  p
                    ON sp.""PostId"" = p.""Id""
                    WHERE p.""HashId"" = @HashId AND p.""IsDelete"" = false AND sp.""IsDelete"" = false
";
            }
        }

        private string GetRelatedBoxPostQuery => @"
                                select p.""Title"",p.""HashId"",p.""Id"",p.""ThumbnailUrl"", 
                                COALESCE(pr.reaction_count, 0) AS TotalReacts,
                                COALESCE(pc.comment_count, 0) + COALESCE(spc.sub_comment_count, 0) AS TotalComment,
                                CASE WHEN COUNT(r.""Type"") > 0 THEN jsonb_agg(DISTINCT jsonb_build_object('Type', r.""Type"")) ELSE NULL END AS ReactionStr,
                                CASE WHEN COUNT(t.""Id"") > 0 THEN array_agg(DISTINCT t.""Name"") ELSE NULL END as Tags
                                from ""comic"".""ComicPosts""  p
                                LEFT JOIN ""comic"".""ComicTagPosts"" tp ON tp.""PostId"" = p.""Id""
                                LEFT JOIN ""Tags"" t ON tp.""TagId"" = t.""Id"" 
                                LEFT JOIN 
                                    (SELECT ""TargetId"", COUNT(*) AS reaction_count 
                                     FROM ""comic"".""ComicPostReactions"" 
                                         WHERE ""IsDelete"" = false
                                     GROUP BY ""TargetId"") pr ON p.""Id""= pr.""TargetId""
                                LEFT JOIN 
                                    (SELECT ""PostId"", COUNT(*) AS comment_count 
                                     FROM ""comic"".""ComicPostComments"" 
                                                  WHERE ""IsDelete"" = false
                                     GROUP BY ""PostId"") pc ON p.""Id"" = pc.""PostId""
                                LEFT JOIN 
                                    (SELECT sp.""PostId"", COUNT(spc.""Id"") AS sub_comment_count 
                                     FROM ""comic"".""ComicSubPostComments"" spc
                                     JOIN ""comic"".""ComicSubPosts"" sp ON spc.""PostId""= sp.""Id""
                                     WHERE spc.""IsDelete""= false
                                     GROUP BY sp.""PostId"") spc ON p.""Id"" = spc.""PostId""
                                LEFT JOIN 
                                    ""comic"".""ComicPostReactions"" r ON p.""Id"" = r.""TargetId"" AND r.""IsDelete"" = false
                                WHERE p.""Type"" != 0
                                And p.""IsDelete"" = false
                                [QueryCondition]
                                GROUP BY p.""Title"",p.""HashId"",p.""Id"",p.""ThumbnailUrl"",pr.reaction_count,pc.comment_count,spc.sub_comment_count
                                ORDER BY RANDOM()
                                LIMIT @Limit";
        private string GetRelatedPostQuery => @"SELECT post.""SelectType"",post.""Id"",post.""Title"", post.""Body"", post.""HashId"",
                        post.""UserId"", post.""ProfileName"",post.""ProfileId"",post.""Avatar"" as ""UserAvatar"", post.""ThumbnailUrl"", 
                        post.""ChapterCount"",
                        post.""Status"", post.""Type"",post.""ViewCount"",post.""TotalComment"",
                        post.""CreatedOn"",post.""AuthorName"", post.""CoverUrl"", post.""IsMature"", post.""IsCompleted"", post.""Permission"", post.""AuthorId"",
                        post.""SubPostStr"", 
                        CASE WHEN COUNT(r.""Type"") > 0 THEN jsonb_agg(DISTINCT jsonb_build_object('Type', r.""Type"")) ELSE null END AS ReactionByPostStr,
                        COUNT(r.""Type"") AS TotalReact,
                        array_agg(DISTINCT tag.""Name"") as Tags from
                            (SELECT  p.""Id"",
                            p.""Title"", p.""Body"",  
                            p.""HashId"",p.""UserId"", sp.""Total"" AS ""ChapterCount"",
                            u.""ProfileName"",u.""ProfileId"",u.""Avatar"", p.""ThumbnailUrl"", 
                            p.""AuthorName"", p.""CoverUrl"", p.""IsMature"", p.""IsCompleted"", p.""Permission"",p.""AuthorId"",
                             postid.""SelectType"",
                            p.""Status"", p.""Type"", postview.""ViewCount"",
                            p.""CreatedOn"",
                               (
               SELECT COUNT(*) 
               FROM ""comic"".""ComicPostComments"" pc 
               WHERE pc.""PostId"" = p.""Id"" AND pc.""IsDelete"" = FALSE
           ) + (
               SELECT COUNT(*)
                FROM ""comic"".""ComicSubPostComments"" spc
               INNER JOIN ""comic"".""ComicSubPosts"" sp ON spc.""PostId"" = sp.""Id""
               WHERE sp.""PostId"" = p.""Id"" AND spc.""IsDelete"" = FALSE
           ) AS ""TotalComment"",
                            to_jsonb(array_agg(sp.*)) AS ""SubPostStr""    
                             
                            FROM ""comic"".""ComicPosts""  p
                              INNER JOIN-- Select Id
                             (
                                [SelectPostIdsQuery]  
                            ) postid 
                             ON postid.""Id"" = p.""Id""
                            LEFT JOIN identity.""Users"" u ON p.""UserId"" = u.""Id""        
                            [JoinSubPostSubQuery]    
                            --Post view
                            LEFT JOIN LATERAL (
                                SELECT 
                                ""EntityId"", 
                                ""Count"" as ""ViewCount""
                                    FROM ""SmartCountActions"" 
                                WHERE ""EntityId"" = p.""Id"" AND ""EntityType"" = 0 AND ""ActionType"" = 2
                                LIMIT 1
                                ) postview ON postview.""EntityId"" = p.""Id""
                            
                            GROUP BY postid.""SelectType"", p.""Id"",p.""Title"", p.""Body"", p.""HashId"", p.""UserId"", 
                            p.""AuthorName"", p.""CoverUrl"", p.""IsMature"",p.""IsCompleted"", p.""Permission"",p.""AuthorId"",
                            sp.""Total"",
                            u.""ProfileName"", u.""ProfileId"",u.""Avatar"", p.""ThumbnailUrl"", 
                            p.""Status"", p.""Type"",postview.""ViewCount"",
                            p.""CreatedOn""
                            ) 
                        AS post
                        LEFT JOIN ""comic"".""ComicTagPosts"" tp ON tp.""PostId"" = post.""Id""
                        LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id"" 
                        LEFT JOIN ""comic"".""ComicPostReactions"" r ON r.""TargetId"" = post.""Id""  AND r.""IsDelete"" = FALSE
                        GROUP BY post.""SelectType"", post.""Id"",post.""Title"", post.""Body"", post.""HashId"", 
                        post.""AuthorName"", post.""CoverUrl"", post.""IsMature"",post.""IsCompleted"", post.""Permission"",post.""AuthorId"",
                        post.""UserId"",post.""ProfileName"",post.""ProfileId"",post.""Avatar"", post.""ThumbnailUrl"", post.""ChapterCount"", post.""TotalComment"",
                        post.""Status"", post.""Type"", post.""ViewCount"",
                        post.""CreatedOn"",
                        post.""SubPostStr""
                        ORDER BY ""[OrderBy]"" desc;

                        [CountResults] ";


        private string GetTopAllPostAllTypeByTagQuery
        {
            get
            {
                return @"SELECT post.""SelectType"",post.""Id"",post.""Title"", post.""Body"", post.""HashId"",
                        post.""UserId"", post.""ProfileName"",post.""ProfileId"",post.""Avatar"" as ""UserAvatar"", post.""ThumbnailUrl"", 
                        post.""ChapterCount"",
                        post.""Status"", post.""Type"",post.""ViewCount"", post.""TotalSubPostComment"" + COALESCE(COUNT(comment.""Id""), 0) AS ""TotalComment"",
                        post.""CreatedOn"",post.""AuthorName"",u.""UserName"",post.""CoverUrl"", post.""IsMature"", post.""IsCompleted"", post.""Permission"", post.""AuthorId"",
                        post.""SubPostStr"", 
                        array_agg(tag.""Name"") as Tags from
                            (SELECT  p.""Id"",
                            p.""Title"", p.""Body"",  
                            p.""HashId"",p.""UserId"", sp.""Total"" AS ""ChapterCount"",
                            u.""ProfileName"",u.""ProfileId"",u.""Avatar"", p.""ThumbnailUrl"", 
                            p.""AuthorName"", p.""CoverUrl"", p.""IsMature"", p.""IsCompleted"", p.""Permission"",p.""AuthorId"",
                             postid.""SelectType"",
                            p.""Status"", p.""Type"", postview.""ViewCount"",
                            p.""CreatedOn"",--sp.""Id"" as ""SPID"",
SUM(""CommentCount"") as ""TotalSubPostComment"",
                            --sp.""ChapterCount"" AS ""ChapterCount"",
                            to_jsonb(array_agg(sp.*)) AS ""SubPostStr""    
                             
                            FROM ""comic"".""ComicPosts""  p
                              INNER JOIN-- Select Id
                             (
                                [SelectPostIdsQuery]  
                            ) postid 
                             ON postid.""Id"" = p.""Id""
                            LEFT JOIN identity.""Users"" u ON p.""UserId"" = u.""Id""        
                            [JoinSubPostSubQuery]    
--Post view
LEFT JOIN LATERAL (
                                SELECT 
                                ""EntityId"", 
                                ""Count"" as ""ViewCount""
                                    FROM ""SmartCountActions"" 
                                WHERE ""EntityId"" = p.""Id"" AND ""EntityType"" = 0 AND ""ActionType"" = 2
LIMIT 1
                                ) postview ON postview.""EntityId"" = p.""Id""
                            
                            GROUP BY postid.""SelectType"", p.""Id"",p.""Title"", p.""Body"", p.""HashId"", p.""UserId"", 
                            p.""AuthorName"", p.""CoverUrl"", p.""IsMature"",p.""IsCompleted"", p.""Permission"",p.""AuthorId"",
                            sp.""Total"",
                            u.""ProfileName"", u.""ProfileId"",u.""Avatar"", p.""ThumbnailUrl"", 
                            p.""Status"", p.""Type"",postview.""ViewCount"",
                            p.""CreatedOn""
                            ) 
                        AS post
                        LEFT JOIN identity.""Users"" u ON post.""UserId"" = u.""Id""
                        LEFT JOIN ""comic"".""ComicTagPosts"" tp ON tp.""PostId"" = post.""Id""
                        LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id"" 
                        LEFT JOIN ""comic"".""ComicPostComments"" comment ON comment.""PostId"" = post.""Id"" AND comment.""IsDelete"" = false
                        GROUP BY post.""SelectType"", post.""Id"",post.""Title"", post.""Body"", post.""HashId"", 
                        post.""AuthorName"", post.""CoverUrl"", post.""IsMature"",post.""IsCompleted"", post.""Permission"",post.""AuthorId"",
                        post.""UserId"",post.""ProfileName"",post.""ProfileId"",post.""Avatar"", post.""ThumbnailUrl"", post.""ChapterCount"",post.""TotalSubPostComment"",
                        post.""Status"", post.""Type"", post.""ViewCount"",
                        post.""CreatedOn"",
                        post.""SubPostStr"",
                        u.""UserName""
                        ORDER BY ""[OrderBy]"" desc;

                        [CountResults] ";
            }
        }

        #region Top hit
        private string GetTopSubQueryJoinSubPostQuery
        {
            get
            {
                return @"LEFT JOIN LATERAL 
                            (
                                SELECT sp.""Id"",sp.""HashId"",sp.""PostId"",sp.""CreatedOn"",sp.""Title"",sp.""Order"", sp.""IsExclusive"",
COUNT(spcm.""Id"") as ""CommentCount"",subpostview.""ViewCount"",
count(*) OVER() AS ""Total"" 
                                FROM ""comic"".""ComicSubPosts"" sp 
--Comment count
LEFT JOIN ""comic"".""ComicSubPostComments"" spcm ON spcm.""PostId"" = sp.""Id"" AND spcm.""IsDelete"" = false
--View count
LEFT JOIN LATERAL (
                                SELECT 
                                ""EntityId"", 
                                ""Count"" as ""ViewCount""
                                    FROM ""SmartCountActions"" 
                                WHERE ""EntityId"" = sp.""Id"" AND ""EntityType"" = 1 AND ""ActionType"" = 2
LIMIT 1
                                ) subpostview ON subpostview.""EntityId"" = sp.""Id""
                                WHERE sp.""PostId"" = p.""Id"" AND sp.""IsDelete"" = false  AND sp.""IsDelete"" = false  
                                AND (sp.""PublishDate"" IS NULL OR sp.""PublishDate"" < TIMEZONE('UTC', now()))

                                GROUP BY sp.""Id"", sp.""PostId"", sp.""Title"",sp.""Order"",subpostview.""ViewCount""
                                ORDER BY sp.""Order"" DESC
                                LIMIT 2
                            ) sp ON sp.""PostId"" = p.""Id""";
            }
        }
        private string GetTopPostHitQuery
        {
            get
            {
                return @"--HIT
                                SELECT DISTINCT qpost.""Id"", COUNT(pcm.""Id"") as COUNTCM, qpost.""CreatedOn"", 0 AS ""SelectType""

                                FROM ""comic"".""ComicPosts""  qpost
                                 INNER JOIN ""comic"".""ComicPostComments"" pcm ON pcm.""PostId"" = qpost.""Id"" 
                                AND pcm.""CreatedOn"" > @LastWeek
                                WHERE  qpost.""Type"" = @PostType AND qpost.""Status"" = @PostStatus AND pcm.""IsDelete"" = false 
                                AND qpost.""IsDelete"" = false                                 
                                GROUP BY qpost.""Id""
                                ORDER BY COUNTCM DESC
                                LIMIT @PageSize
                                OFFSET @Offet";
            }
        }
        private string GetTopPostHitToCountQuery
        {
            get
            {
                return @"--HIT
                                SELECT qpost.""Id""
                                FROM ""comic"".""ComicPosts""  qpost
                                 INNER JOIN ""comic"".""ComicPostComments"" pcm ON pcm.""PostId"" = qpost.""Id"" 
                                AND pcm.""CreatedOn"" > @LastWeek
                                WHERE  qpost.""Type"" = @PostType AND qpost.""Status"" = @PostStatus AND pcm.""IsDelete"" = false 
                                AND qpost.""IsDelete"" = false                                 
                                GROUP BY qpost.""Id""";
            }
        }
        #endregion

        #region Top latest

        private string GetTopLatestPostHitQuery
        {
            get
            {
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, psp1.""CreatedOn"", 1 AS ""SelectType""
                                 FROM ""comic"".""ComicPosts""  qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId"", sp1.""CreatedOn"" 
                                    FROM ""comic"".""ComicSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  AND sp1.""Status"" = @PostStatus
                                    GROUP BY sp1.""Id"",sp1.""PostId"",sp1.""CreatedOn""
                                    ORDER BY sp1.""CreatedOn"" DESC
                                    LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
                                WHERE  qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
                                AND qpost1.""IsDelete"" = false                                 
                                GROUP BY qpost1.""Id"", psp1.""CreatedOn""
                                ORDER BY psp1.""CreatedOn"" DESC
                                LIMIT @PageSize
                                OFFSET @Offet";
            }
        }
        private string GetTopLatestPostToCountQuery
        {
            get
            {
                return @"SELECT qpost1.""Id""
                                 FROM ""comic"".""ComicPosts""  qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId""
                                    FROM ""comic"".""ComicSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  AND sp1.""Status"" = @PostStatus
                                    GROUP BY sp1.""Id"", sp1.""PostId""
                                    -- LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
                                WHERE qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
                                AND qpost1.""IsDelete"" = false                                 
                                GROUP BY qpost1.""Id""";
            }
        }
        #endregion

        #region Top latestByTag

        private string GetTopLatestPostByTagQuery
        {
            get
            {
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, psp1.""CreatedOn"", 1 AS ""SelectType""
                                 FROM ""comic"".""ComicPosts""  qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId"", sp1.""CreatedOn"" 
                                    FROM ""comic"".""ComicSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  AND sp1.""Status"" = @PostStatus
                                    GROUP BY sp1.""Id"",sp1.""PostId"",sp1.""CreatedOn""
                                    ORDER BY sp1.""CreatedOn"" DESC
                                    LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
                                LEFT JOIN ""comic"".""ComicTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
                                LEFT JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 

                                WHERE (@TagName IS NULL OR qtag.""Name"" = @TagName) AND qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
                                AND qpost1.""Permission"" = @PostPermission
                                AND qpost1.""IsDelete"" = false                                 
                                GROUP BY qpost1.""Id"", psp1.""CreatedOn""
                                ORDER BY psp1.""CreatedOn"" DESC
                                LIMIT @PageSize
                                OFFSET @Offet";
            }
        }

        private string GetTopLatestPostByMultiTagQuery
        {
            get
            {
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, psp1.""CreatedOn"", 1 AS ""SelectType""
                                 FROM ""comic"".""ComicPosts""  qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId"", sp1.""CreatedOn"" 
                                    FROM ""comic"".""ComicSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  AND sp1.""Status"" = @PostStatus
                                    GROUP BY sp1.""Id"",sp1.""PostId"",sp1.""CreatedOn""
                                    ORDER BY sp1.""CreatedOn"" DESC
                                    LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
                                LEFT JOIN ""comic"".""ComicTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
                                LEFT JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
                                [WhereMainQuery]                                 
                                GROUP BY qpost1.""Id"", psp1.""CreatedOn""
                                ORDER BY psp1.""CreatedOn"" DESC
                                LIMIT @PageSize
                                OFFSET @Offet";
            }
        }
        private string GetTopLatestPostByTagToCountQuery
        {
            get
            {
                return @"SELECT qpost1.""Id""
                                 FROM ""comic"".""ComicPosts""  qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId""
                                    FROM ""comic"".""ComicSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  AND sp1.""Status"" = @PostStatus
                                    GROUP BY sp1.""Id"", sp1.""PostId""
                                    -- LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
                                LEFT JOIN ""comic"".""ComicTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
                                LEFT JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
                                WHERE (@TagName IS NULL OR qtag.""Name"" = @TagName) AND qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
                                AND qpost1.""Permission"" = @PostPermission
                                AND qpost1.""IsDelete"" = false                                 
                                GROUP BY qpost1.""Id""";
            }
        }
        private string GetTopLatestPostByMultiTagToCountQuery
        {
            get
            {
                return @"SELECT qpost1.""Id""
                                 FROM ""comic"".""ComicPosts""  qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId""
                                    FROM ""comic"".""ComicSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  AND sp1.""Status"" = @PostStatus
                                    GROUP BY sp1.""Id"", sp1.""PostId""
                                    -- LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
                                LEFT JOIN ""comic"".""ComicTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
                                LEFT JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
                                [WhereMainQuery]                                 
                                GROUP BY qpost1.""Id""";
            }
        }
        #endregion
        #region Top latestByTag - Favorite

        private string GetTopLatestPostByFavoriteQuery
        {
            get
            {
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, psp1.""CreatedOn"", 1 AS ""SelectType""
                                 FROM ""comic"".""ComicPosts""  qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId"", sp1.""CreatedOn"" 
                                    FROM ""comic"".""ComicSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  AND sp1.""Status"" = @PostStatus
                                    GROUP BY sp1.""Id"",sp1.""PostId"",sp1.""CreatedOn""
                                    ORDER BY sp1.""CreatedOn"" DESC
                                    LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 

                                INNER JOIN ""comic"".""ComicTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
INNER JOIN ""TagFavorites"" tagfa ON qtp.""TagId"" = tagfa.""TagId""
                                INNER JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 

                                WHERE tagfa.""UserId"" = @UserId AND qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
                                AND qpost1.""IsDelete"" = false                                 
                                GROUP BY qpost1.""Id"", psp1.""CreatedOn""
                                ORDER BY psp1.""CreatedOn"" DESC
                                LIMIT @PageSize
                                OFFSET @Offet";
            }
        }
        private string GetTopLatestPostByFavoriteToCountQuery
        {
            get
            {
                return @"SELECT qpost1.""Id""
                                 FROM ""comic"".""ComicPosts""  qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId""
                                    FROM ""comic"".""ComicSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  AND sp1.""Status"" = @PostStatus
                                    GROUP BY sp1.""Id"", sp1.""PostId""
                                    -- LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
                                LEFT JOIN ""comic"".""ComicTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
INNER JOIN ""TagFavorites"" tagfa ON qtp.""TagId"" = tagfa.""TagId""
                                LEFT JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
                                WHERE tagfa.""UserId"" = @UserId AND qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
                                AND qpost1.""IsDelete"" = false                                 
                                GROUP BY qpost1.""Id""";
            }
        }
        #endregion

        #region Top completed
        private string GetTopLatestCompletePostHitQuery
        {
            get
            {
                return @"SELECT qpost1.""Id"", 0 as COUNTCM, qpost1.""CreatedOn"", 2 AS ""SelectType""
                                 FROM ""comic"".""ComicPosts""  qpost1

                                WHERE  qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
                                AND qpost1.""IsDelete"" = false AND qpost1.""IsCompleted"" = true                    

                                ORDER BY qpost1.""CreatedOn"" DESC
                                LIMIT @PageSize
                                OFFSET @Offet";
            }
        }
        private string GetTopLatestCompletePostToCountQuery
        {
            get
            {
                return @"SELECT qpost2.""Id""
                                 FROM ""comic"".""ComicPosts""  qpost2
                                WHERE  qpost2.""Type"" = @PostType AND qpost2.""Status"" = @PostStatus
                                AND qpost2.""IsDelete"" = false AND qpost2.""IsCompleted"" = true";
            }
        }
        #endregion

        #region Top Recommended

        private string GetTopRecommendedPostHitQuery
        {
            get
            {
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, qpost1.""CreatedOn"", 4 AS ""SelectType""
                                FROM ""comic"".""ComicPosts""  qpost1                                 
                                WHERE  qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
                                AND qpost1.""IsDelete"" = false                                 
                                GROUP BY qpost1.""Id"", qpost1.""CreatedOn""
                                ORDER BY qpost1.""CreatedOn"" DESC
                                LIMIT @PageSize";
            }
        }
        #endregion

        #region By tag

        private string GetLatestPostByTagHitQuery
        {
            get
            {
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, qpost1.""ModifiedOn"" AS ""CreatedOn"", 3 AS ""SelectType""
                                 FROM ""comic"".""ComicPosts""  qpost1        
                                 INNER JOIN ""comic"".""ComicTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
                                INNER JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
                                WHERE  qtag.""Name"" = @TagName AND qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
                                AND qpost1.""IsDelete"" = false                                 
                                                            
                                GROUP BY qpost1.""Id"", qpost1.""ModifiedOn""
                                ORDER BY qpost1.""ModifiedOn"" DESC
                                LIMIT @PageSize
                                OFFSET @Offet";
            }
        }
        private string GetLatestPostByTagToCountQuery
        {
            get
            {
                return @"SELECT qpost1.""Id""
                                 FROM ""comic"".""ComicPosts""  qpost1
                                 INNER JOIN ""comic"".""ComicTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
                                INNER JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
                                WHERE  qtag.""Name"" = @TagName AND qpost1.""Type"" = @PostType 
AND qpost1.""Status"" = @PostStatus AND qpost1.""IsDelete"" = false                            
                                GROUP BY qpost1.""Id""";
            }
        }

        #endregion

        #region Followed Post
        private string GetMyPostFollowedIdsQuery
        {
            get
            {
                return @" --My post
                                SELECT qpost.""Id"", 0 AS ""SelectType""
                                FROM ""comic"".""ComicPosts""  qpost
                                LEFT JOIN ""comic"".""ComicFollowedPosts"" cfp on qpost.""Id"" = cfp.""PostId""
                                WHERE cfp.""CreatedBy"" = @UserId AND cfp.""IsDelete"" = false  AND qpost.""IsDelete"" = false             
                                ORDER BY cfp.""[OrderBy]"" DESC
                                LIMIT @PageSize
                                OFFSET @Offet";
            }
        }
        private string GetMyPostFollowedCountQuery
        {
            get
            {
                return @" --My post
                                SELECT qpost.""Id""
                                FROM ""comic"".""ComicPosts""  qpost                                 
                                LEFT JOIN ""comic"".""ComicFollowedPosts"" cfp on qpost.""Id"" = cfp.""PostId""
                                WHERE cfp.""CreatedBy"" = @UserId AND cfp.""IsDelete"" = false  AND qpost.""IsDelete"" = false";
            }
        }
        #endregion

        #region My series
        private string GetMyPostIdsQuery
        {
            get
            {
                return @"--My post

                                SELECT qpost.""Id"", 0 AS ""SelectType""
                                FROM ""comic"".""ComicPosts""  qpost
                                WHERE qpost.""CreatedBy"" = @UserId AND qpost.""Type"" = @PostType AND qpost.""IsDelete"" = false

                                ORDER BY ""[OrderBy]"" DESC
                                LIMIT @PageSize
                                OFFSET @Offet";
            }
        }
        private string GetMyPostCountQuery
        {
            get
            {
                return @" --My post
                                SELECT qpost.""Id""
                                FROM ""comic"".""ComicPosts""  qpost                                 
                                WHERE qpost.""CreatedBy"" = @UserId AND qpost.""Type"" = @PostType AND qpost.""IsDelete"" = false";
            }
        }
        private string GetMyAllComicStoryQuery
        {
            get
            {
                return $@" SELECT ""HashId"", ""Title"", ""Type"", ""Status""
                                FROM {_postRepository.TableName}                                 
                                WHERE ""CreatedBy"" = @UserId AND ""IsDelete"" = false AND ""Type"" IN ({(int)PostType.Story}, {(int)PostType.Comic}) 
                                ORDER BY ""CreatedOn"" DESC;";
            }
        }

        #endregion

        #region By User
        private string GetLatestPostByUserHitQuery
        {
            get
            {
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, qpost1.""ModifiedOn"" AS ""CreatedOn"", 3 AS ""SelectType""
                                 FROM ""comic"".""ComicPosts""  qpost1        
                                 INNER JOIN identity.""Users"" user1 ON user1.""Id"" = qpost1.""UserId""                                
                                WHERE  user1.""UserName"" = @ProfileName AND qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
                                AND qpost1.""IsDelete"" = false                                 
                                                            
                                GROUP BY qpost1.""Id"", qpost1.""ModifiedOn""
                                ORDER BY qpost1.""ModifiedOn"" DESC
                                LIMIT @PageSize
                                OFFSET @Offet";
            }
        }

        private string GetLatestPostByUserToCountQuery
        {
            get
            {
                return @"SELECT qpost1.""Id""
                                 FROM ""comic"".""ComicPosts""  qpost1
                                 INNER JOIN identity.""Users"" user1 ON user1.""Id"" = qpost1.""UserId""                                
                                WHERE  user1.""UserName"" = @ProfileName AND qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
                                AND qpost1.""IsDelete"" = false
                                GROUP BY qpost1.""Id""";
            }
        }
        #endregion

        #region SubPost - View

        private string GetSeriesChapterByHashIdWithJoinOrder
        {
            get
            {
                return @"SELECT sp.""Id"",sp.""HashId"",sp.""Name"", sp.""Title"", sp.""PostId"", sp.""Order"", sp.""Body"",sp.""IsPremium"", sp.""IsExclusive"", ux.""Id"" as ""UserExclusiveId"",
                    sp.""Status"", sp.""CreatedOn"", sp.""CreatedBy"", sp.""ModifiedOn"", 
                    sp.""ModifiedBy"", sp.""IsDelete"", count.""ViewCount"", sp.""AuthorId"", u.""ProfileName"",
                    sp.""UserId"", sp.""PublishDate"", sp.""Permission"",sp.""CreatorNote"",
                    sp.""IsEnableComment"", spcmc.""CommentCount"",
                    rs.""Id"", rs.""AuthorId"", rs.""Title"", rs.""Name"", rs.""Url"", rs.""Type"", rs.""CreatedOn"", 
                    rs.""CreatedBy"", rs.""ModifiedOn"", rs.""ModifiedBy"", rs.""IsDelete"", rs.""HashId"", rs.""SubPostId"", 
                    rs.""Status"", rs.""Size"", rs.""LocationType"", rs.""Height"", rs.""Width"", rs.""Order""
                    FROM ""comic"".""ComicSubPosts"" sp
                    INNER JOIN ""comic"".""ComicPosts""  p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
LEFT JOIN ""UserExclusiveSubPosts"" ux ON ux.""SubPostId"" = sp.""Id"" AND ux.""UserId"" = @UserId
INNER JOIN identity.""Users"" u ON u.""Id"" = sp.""CreatedBy""
                    LEFT JOIN ""comic"".""ComicResources"" rs ON sp.""Id"" = rs.""SubPostId"" AND rs.""IsDelete"" = false
LEFT JOIN LATERAL 
                            (
                                SELECT COUNT(spcm.""Id"") as ""CommentCount"", spcm.""PostId"" as ""SubPostId""
FROM ""comic"".""ComicSubPostComments"" spcm 
                                WHERE spcm.""PostId"" = sp.""Id"" AND spcm.""IsDelete"" = false
                                GROUP BY ""PostId""
                                LIMIT 1
) spcmc ON spcmc.""SubPostId"" = sp.""Id""


LEFT JOIN LATERAL (
                                SELECT 
                                ""EntityId"", 
                                ""Count"" as ""ViewCount""
                                    FROM ""SmartCountActions"" 
                                WHERE ""EntityId"" = sp.""Id"" AND ""EntityType"" = 1 AND ""ActionType"" = 2
LIMIT 1
                                ) count ON count.""EntityId"" = sp.""Id""

                    WHERE p.""HashId"" = @PostHashId AND sp.""Order"" = @SubPostOrder AND sp.""IsDelete"" = false
                    ORDER BY rs.""Order"";";
            }
        }
        private string GetSeriesChapterByHashIdOrder
        {
            get
            {
                return @"SELECT sp.""Id"",sp.""HashId"", sp.""Title"", sp.""PostId"", sp.""Order"", sp.""Body"", sp.""IsExclusive"",
                sp.""Status"", sp.""CreatedOn"", sp.""CreatedBy"", sp.""ModifiedOn"", 
                sp.""ModifiedBy"", sp.""IsDelete"", count.""ViewCount"", sp.""AuthorId"", 
                sp.""UserId"", sp.""PublishDate"", sp.""Permission"",sp.""CreatorNote"",
sp.""IsEnableComment""
                    FROM ""comic"".""ComicSubPosts"" sp
                    INNER JOIN ""comic"".""ComicPosts""  p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
LEFT JOIN LATERAL (
                                SELECT 
                                ""EntityId"", 
                                ""Count"" as ""ViewCount""
                                    FROM ""SmartCountActions"" 
                                WHERE ""EntityId"" = sp.""Id"" AND ""EntityType"" = 1 AND ""ActionType"" = 2
LIMIT 1
                                ) count ON count.""EntityId"" = sp.""Id""
                    WHERE p.""HashId"" = @PostHashId AND sp.""Order"" = @SubPostOrder AND sp.""IsDelete"" = false";
            }
        }

        private string GetSeriesChaptersByHashId
        {
            get
            {
                return @"SELECT sp.""Id"",sp.""HashId"",sp.""Name"", sp.""Title"", sp.""PostId"", sp.""Order"", sp.""Body"",sp.""IsPremium"", sp.""IsExclusive"",ux.""Id"" as ""UserExclusiveId"",
                sp.""Status"", sp.""CreatedOn"", sp.""CreatedBy"", sp.""ModifiedOn"", 
                sp.""ModifiedBy"", sp.""IsDelete"", count.""ViewCount"", sp.""AuthorId"", 
                sp.""UserId"", sp.""PublishDate"", sp.""Permission"", sp.""CreatorNote"",
sp.""IsEnableComment"", spcmc.""CommentCount""
                    FROM ""comic"".""ComicSubPosts"" sp
LEFT JOIN ""UserExclusiveSubPosts"" ux ON ux.""SubPostId"" = sp.""Id"" AND ux.""UserId"" = @UserId
                    INNER JOIN ""comic"".""ComicPosts""  p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
LEFT JOIN LATERAL 
                            (
                                SELECT COUNT(spcm.""Id"") as ""CommentCount"", spcm.""PostId"" as ""SubPostId""
FROM ""comic"".""ComicSubPostComments"" spcm 
                                WHERE spcm.""PostId"" = sp.""Id"" AND spcm.""IsDelete"" = false
                                GROUP BY ""PostId""
                                LIMIT 1
) spcmc ON spcmc.""SubPostId"" = sp.""Id""
LEFT JOIN LATERAL (
                                SELECT 
                                ""EntityId"", 
                                ""Count"" as ""ViewCount""
                                    FROM ""SmartCountActions"" 
                                WHERE ""EntityId"" = sp.""Id"" AND ""EntityType"" = 1 AND ""ActionType"" = 2
LIMIT 1
                                ) count ON count.""EntityId"" = sp.""Id""
                    WHERE p.""HashId"" = @PostHashId [WithPermission] AND sp.""IsDelete"" = false
                    ORDER BY ""[OrderBy]""
                    LIMIT @PageSize
                    OFFSET @Offet;

                        SELECT COUNT(*) AS TotalItems 
                        FROM (
                                SELECT sp.""Id""            
                                FROM ""comic"".""ComicSubPosts"" sp
                                INNER JOIN ""comic"".""ComicPosts""  p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
                                WHERE p.""HashId"" = @PostHashId AND sp.""IsDelete"" = false
                            ) p;
";
            }
        }

        private string GetSeriesChaptersSimpleByHashId
        {
            get
            {
                return @"SELECT sp.""Id"", sp.""Title"", sp.""Order""
                    FROM ""comic"".""ComicSubPosts"" sp
                    INNER JOIN ""comic"".""ComicPosts""  p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
                    WHERE p.""HashId"" = @PostHashId AND sp.""IsDelete"" = false
                    ORDER BY sp.""Order"";

                        SELECT COUNT(*) AS TotalItems 
                        FROM (
                                SELECT sp.""Id""            
                                FROM ""comic"".""ComicSubPosts"" sp
                                INNER JOIN ""comic"".""ComicPosts""  p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
                                WHERE p.""HashId"" = @PostHashId AND sp.""IsDelete"" = false
                            ) p;
";
            }
        }
        private string GetSeriesChaptersWithOffsetSimpleByHashId
        {
            get
            {
                return $@"SELECT sp.""Id"", sp.""Title"", sp.""PublishDate"", sp.""Order""
                    FROM {_subPostRepository.TableName} sp
                    INNER JOIN {_postRepository.TableName} p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
                    WHERE p.""CreatedBy"" = @UserId AND p.""HashId"" = @PostHashId AND sp.""IsDelete"" = false
                    ORDER BY sp.""Order""
                    LIMIT @PageSize
                    OFFSET @Offet;

                    SELECT COUNT(*) AS TotalItems 
                    FROM (
                            SELECT sp.""Id""            
                            FROM {_subPostRepository.TableName} sp
                            INNER JOIN {_postRepository.TableName} p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
                            WHERE p.""CreatedBy"" = @UserId AND p.""HashId"" = @PostHashId AND sp.""IsDelete"" = false
                        ) p;
                        ";
            }
        }
        #endregion
        private string PaginationGetAllPostTopByTagQuery
        {
            get
            {
                return @"SELECT post.""SelectType"",post.""Id"",post.""Title"", post.""Body"", post.""HashId"",
                        post.""UserId"", post.""ProfileName"",post.""ProfileId"", post.""ThumbnailUrl"", 
                        post.""ChapterCount"",
                        post.""Status"", post.""Type"",post.""ViewCount"",
                        post.""CreatedOn"",post.""AuthorName"", post.""CoverUrl"", post.""IsMature"",post.""IsCompleted"", post.""Permission"", post.""AuthorId"",
                        post.""SubPostStr"", 
                        array_agg(tag.""Name"") as Tags from
                            (SELECT  p.""Id"",
                            p.""Title"", p.""Body"",  
                            p.""HashId"",p.""UserId"", sp.""Total"" AS ""ChapterCount"",
                            u.""ProfileName"",u.""ProfileId"", p.""ThumbnailUrl"", 
                            p.""AuthorName"", p.""CoverUrl"", p.""IsMature"",p.""IsCompleted"", p.""Permission"",p.""AuthorId"",
                             postid.""SelectType"",
                            p.""Status"", p.""Type"", p.""ViewCount"",
                            p.""CreatedOn"",--sp.""Id"" as ""SPID"",
                            --sp.""ChapterCount"" AS ""ChapterCount"",
                            to_jsonb(array_agg(sp.*)) AS ""SubPostStr""    
                             
                            FROM ""comic"".""ComicPosts""  p
                              INNER JOIN-- Select Id
                             (
                                [SelectPostIdsQuery] 
                            ) postid 
                             ON postid.""Id"" = p.""Id""
                            LEFT JOIN identity.""Users"" u ON p.""UserId"" = u.""Id""        
                            LEFT JOIN LATERAL 
                            (
                                SELECT ""Id"",""PostId"",""CreatedOn"",""Title"",""Order"", count(*) OVER() AS ""Total"" 
                                FROM ""comic"".""ComicSubPosts"" sp 
                                WHERE ""PostId"" = p.""Id"" AND sp.""IsDelete"" = false
                                GROUP BY ""Id"", ""PostId"", ""Title"",""Order""
                                ORDER BY ""Order"" DESC
                                LIMIT 2
                            ) sp ON sp.""PostId"" = p.""Id""                            
                            
                            GROUP BY postid.""SelectType"", p.""Id"",p.""Title"", p.""Body"", p.""HashId"", p.""UserId"", 
                            p.""AuthorName"", p.""CoverUrl"", p.""IsMature"",p.""IsCompleted"", p.""Permission"",p.""AuthorId"",
                            sp.""Total"",
                            u.""ProfileName"", u.""ProfileId"", p.""ThumbnailUrl"", 
                            p.""Status"", p.""Type"",p.""ViewCount"",
                            p.""CreatedOn""
                            ) 
                        AS post
                        LEFT JOIN ""comic"".""ComicTagPosts"" tp ON tp.""PostId"" = post.""Id""
                        LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id"" 
                        GROUP BY post.""SelectType"", post.""Id"",post.""Title"", post.""Body"", post.""HashId"", 
                        post.""AuthorName"", post.""CoverUrl"", post.""IsMature"",post.""IsCompleted"", post.""Permission"",post.""AuthorId"",
                        post.""UserId"",post.""ProfileName"",post.""ProfileId"", post.""ThumbnailUrl"", post.""ChapterCount"",
                        post.""Status"", post.""Type"", post.""ViewCount"",
                        post.""CreatedOn"",
                        post.""SubPostStr"";";
            }
        }

        private string PaginationCountResult
        {
            get
            {
                return @"SELECT COUNT(*) AS TotalItems FROM ([WhereCountQuery]) subq;";

            }
        }
        private string ExecSoftDeletePost
        {
            get
            {
                return @"UPDATE ""comic"".""ComicPosts"" 
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""Id"" = @PostId;

                    UPDATE ""comic"".""ComicResources""
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    FROM (SELECT ""Id""
                          FROM ""comic"".""ComicSubPosts"" WHERE ""PostId"" = @PostId) AS sp
                    WHERE ""comic"".""ComicResources"".""SubPostId"" = sp.""Id"";
    
                    UPDATE ""comic"".""ComicSubPostReactions""spr
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    FROM (SELECT ""Id""
                          FROM ""comic"".""ComicSubPosts"" WHERE ""PostId"" = @PostId) AS sp
                    WHERE spr.""TargetId"" = sp.""Id"";
    
                    UPDATE ""comic"".""ComicSubPostComments"" spr
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    FROM (SELECT ""Id""
                          FROM ""comic"".""ComicSubPosts"" WHERE ""PostId"" = @PostId) AS sp
                    WHERE spr.""PostId"" = sp.""Id"";
    
                    UPDATE ""comic"".""ComicSubPostCommentReactions"" spr
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    FROM (SELECT ""Id""
                          FROM ""comic"".""ComicSubPosts"" WHERE ""PostId"" = @PostId) AS sp
                    WHERE spr.""TargetId"" = sp.""Id"";
    
                    UPDATE ""comic"".""ComicTagPosts""
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""PostId"" = @PostId;
    
                    UPDATE ""comic"".""ComicSubPosts""
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""PostId"" = @PostId;
    
                    UPDATE ""comic"".""ComicPostReactions""
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""TargetId"" = @PostId;
    
                    UPDATE ""comic"".""ComicPostComments""
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""PostId"" = @PostId;";
            }
        }


        #region Delete Subpost

        private string GetSubPostIdWithHashIdAndOrder

        {
            get
            {
                return @"SELECT sp.""Id"", sp.""PostId"", 
                sp.""Order"", sp.""Body"", sp.""Status"", p.""UserId""                
            FROM ""comic"".""ComicSubPosts"" sp
            INNER JOIN ""comic"".""ComicPosts""  p ON sp.""PostId"" = p.""Id""
            WHERE p.""HashId"" = @HashId AND sp.""Order"" = @Order
            AND p.""IsDelete"" = false AND sp.""IsDelete"" = false;";
            }
        }
        private string GetSubPostsWithHashIdAndOrders

        {
            get
            {
                return @"SELECT sp.""Id"", sp.""Title"", sp.""PostId"", sp.""Order"", sp.""Body"", sp.""IsExclusive"",
                sp.""Status"", sp.""CreatedOn"", sp.""CreatedBy"", sp.""ModifiedOn"", 
                sp.""ModifiedBy"", sp.""IsDelete"", sp.""ViewCount"", sp.""AuthorId"", 
                sp.""UserId"", sp.""PublishDate"", sp.""Permission"", sp.""CreatorNote"",
sp.""IsEnableComment""                
            FROM ""comic"".""ComicSubPosts"" sp
            INNER JOIN ""comic"".""ComicPosts""  p ON sp.""PostId"" = p.""Id""
            WHERE p.""HashId"" = @HashId AND (sp.""Order"" = @Order1 OR sp.""Order"" = @Order2 )
            AND p.""IsDelete"" = false AND sp.""IsDelete"" = false;";
            }
        }
        private string ExecSoftDeleteSubPost
        {
            get
            {
                return @"UPDATE ""comic"".""ComicSubPosts""
                    SET ""Order"" = 0, ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""Id"" = @SubPostId;

                    UPDATE ""comic"".""ComicResources""
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""SubPostId"" = @SubPostId;
    
                    UPDATE ""comic"".""ComicSubPostReactions""
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""TargetId"" = @SubPostId;
    
                    UPDATE ""comic"".""ComicSubPostComments""
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""PostId"" = @SubPostId;
    
                    UPDATE ""comic"".""ComicSubPostCommentReactions"" 
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""TargetId"" = @SubPostId;";
            }
        }

        private string GetLatestPostsByTypeQuery
        {
            get
            {
                return $@"
                      WITH ranked_posts AS (
          SELECT ""Id"", ""Type"", ""CreatedOn"", ""HashId"",
              ROW_NUMBER() OVER (PARTITION BY ""Type"" ORDER BY ""CreatedOn"" DESC) AS type_rank
          FROM ""comic"".""ComicPosts"" 
          WHERE ""Type"" IN (0, 1, 2)
          AND ""IsDelete"" = false
          AND ""Status"" = {(int)PostStatus.Public}
         ),
         limited_posts AS (
          SELECT ""Id"", ""Type"", ""CreatedOn"", ""HashId""
          FROM ranked_posts
          WHERE (""Type"" = 0 AND type_rank <= @feed)
             OR (""Type"" = 1 AND type_rank <= @story)
             OR (""Type"" = 2 AND type_rank <= @comic)
         ),
         numbered_posts AS (
          SELECT ""Id"", ""Type"", ""CreatedOn"", ""HashId"",
              ROW_NUMBER() OVER (PARTITION BY ""Type"" ORDER BY ""CreatedOn"" DESC) AS num
          FROM limited_posts
         ),
         grouped_posts AS (
          SELECT ""Id"", ""Type"", ""CreatedOn"", ""HashId"",
              CEILING(CAST(num AS FLOAT) / 
              CASE
               WHEN ""Type"" = 0 THEN @feedPercent *10
               WHEN ""Type"" = 1 THEN @storyPercent *10
               WHEN ""Type"" = 2 THEN @comicPercent *10
              END) AS group_number
          FROM numbered_posts
         ),
         final_grouped_posts AS (
          SELECT ""Id"", ""Type"", ""CreatedOn"", ""HashId"", group_number,
              ROW_NUMBER() OVER (PARTITION BY group_number ORDER BY ""CreatedOn"" DESC) AS row_num
          FROM grouped_posts
         )
         SELECT ""Id"", ""Type"", ""CreatedOn"", ""HashId"", group_number
         FROM final_grouped_posts
         WHERE row_num <= 10
         ORDER BY group_number, row_num;

          [GetTotalCount]";
            }
        }

        private string GetLatestPostsByTagQuery
        {
            get
            {
                return $@"
                     WITH ranked_posts AS (
    SELECT p.""Id"", p.""Type"", p.""CreatedOn"", p.""HashId"",
           ROW_NUMBER() OVER (PARTITION BY ""Type"" ORDER BY p.""CreatedOn"" DESC) AS type_rank
    FROM ""comic"".""ComicPosts""  p
LEFT JOIN ""comic"".""ComicTagPosts"" tp on p.""Id"" = tp.""PostId""                
LEFT JOIN ""Tags"" t on t.""Id"" = tp.""TagId""
    WHERE ""Type"" IN (0, 1, 2)
    AND t.""Name"" ILIKE @ExactKeyword   
    AND p.""IsDelete"" = false
    AND p.""Status"" = 1
),
limited_posts AS (
    SELECT ""Id"", ""Type"", ""CreatedOn"", ""HashId""
    FROM ranked_posts
     WHERE (""Type"" = 0 AND type_rank <= @feed)
               OR (""Type"" = 1 AND type_rank <= @story)
               OR (""Type"" = 2 AND type_rank <= @comic)
),
numbered_posts AS (
    SELECT ""Id"", ""Type"", ""CreatedOn"", ""HashId"",
           ROW_NUMBER() OVER (PARTITION BY ""Type"" ORDER BY ""CreatedOn"" DESC) AS num
    FROM limited_posts
),
grouped_posts AS (
    SELECT ""Id"", ""Type"", ""CreatedOn"", ""HashId"",
           CEIL(num / CASE
          WHEN ""Type"" = 0 THEN @feedPercent *10
               WHEN ""Type"" = 1 THEN @storyPercent *10
               WHEN ""Type"" = 2 THEN @comicPercent *10
           END) AS group_number
    FROM numbered_posts
),
final_grouped_posts AS (
    SELECT ""Id"", ""Type"", ""CreatedOn"", ""HashId"", group_number,
           ROW_NUMBER() OVER (PARTITION BY group_number ORDER BY RANDOM()) AS random_row_num
    FROM grouped_posts
)
SELECT ""Id"", ""Type"", ""CreatedOn"", ""HashId"", group_number
FROM final_grouped_posts
WHERE random_row_num <= 10
ORDER BY group_number, random_row_num;


;
        
        [GetTotalCount]
        ";
            }
        }
        #endregion
        private string GetCountPostByTypeQuery => $@"SELECT COUNT(*) 
                                                   FROM ""comic"".""ComicPosts"" 
                                                   WHERE ""IsDelete"" = false 
                                                   AND ""Status"" = {(int)PostStatus.Public}";

        private string GetCountPostByTagQuery => $@"SELECT COUNT(*) 
                                                   FROM ""comic"".""ComicPosts""  p
                                                   LEFT JOIN ""comic"".""ComicTagPosts"" tp on p.""Id"" = tp.""PostId""
                                                   LEFT JOIN ""Tags"" t on t.""Id"" = tp.""TagId""
                                                   WHERE p.""IsDelete"" = false 
                                                   AND p.""Status"" = {(int)PostStatus.Public}
                                                   AND t.""Name"" ILIKE @ExactKeyword";
        private string PremiumWhereQuery
        {
            get
            {
                return @" AND (0). ";
            }
        }

        private string GetPostRandomIdsQuery
        {
            get
            {
                return @"
                        WITH newtable 
                        AS
                        (
                        SELECT * FROM ""comic"".""ComicPosts""  p
                            WHERE p.""IsDelete"" = false
                            ORDER BY RANDOM()
                            LIMIT @numOfItemNeedFilter
                        )
                        SELECT ""Id"" FROM newtable nt
                        WHERE NOT nt.""Id"" = ANY(@postRandomIds)
                        LIMIT @numOfItem
                 ";
            }
        }
        private string GetPostDetailsQuery
        {
            get
            {
                return @"
                
                SELECT 
                    p.""Id"",
                    p.""HashId"",
                    p.""ThumbnailUrl"",
                    p.""Type"",
                    p.""Body"",
                    p.""Title"",
                    p.""AuthorId"",
                    p.""AuthorName"",
                    u.""ProfileName"",
                    p.""ViewCount"",
                    p.""IsMature"",
                    p.""CreatedOn"",
                    to_json(array_agg(distinct(sp.*)) FILTER (WHERE sp.* IS NOT NULL))AS ""SubPosts"",
                    to_json(array_agg(distinct (t.""Name""))  FILTER (WHERE t.""Name"" IS NOT NULL)) AS ""Tags""
                FROM ""comic"".""ComicPosts"" p
                LEFT JOIN ""comic"".""ComicTagPosts"" tp ON p.""Id"" = tp.""PostId""
                LEFT JOIN ""Tags"" t ON tp.""TagId"" = t.""Id""
                LEFT JOIN identity.""Users"" u on u.""Id"" = p.""UserId""
                LEFT JOIN (
                    SELECT ""PostId"",
                           ""Title"",
                           ""Order"",
                           ""CreatedOn"",
                           ROW_NUMBER() OVER (PARTITION BY ""PostId"" ORDER BY ""Order"" desc) AS rn
                    FROM ""comic"".""ComicSubPosts""
                    WHERE ""IsDelete"" = false
                ) sp ON p.""Id"" = sp.""PostId"" AND sp.rn <= 2 
                WHERE p.""HashId"" = ANY(@HashIds)
                GROUP BY  
                          p.""HashId"",
                          p.""Id"",
                          p.""ThumbnailUrl"",
                          p.""Body"",
                          p.""Title"",
                          p.""AuthorId"",
                          p.""AuthorName"",
                          u.""ProfileName"",
                          p.""ViewCount""
            ";
            }
        }
    }
}
