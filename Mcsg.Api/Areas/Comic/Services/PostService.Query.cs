namespace Mcsg.Api.Areas.Comic.Services
{
    using Common.Core.Enums;

    public partial class PostService
    {
        private string GetTotalCommentQuery => $@"SELECT 
                                                        (SELECT COUNT(*)
                                                         FROM ""comic"".""ComicPostComments"" pc
                                                         JOIN ""comic"".""ComicPosts"" p ON pc.""PostId""= p.""Id"" 
                                                         WHERE p.""HashId"" = @HashId
                                                         And pc.""IsDelete"" = false) 
                                                        +
                                                        (SELECT COUNT(*)
                                                         FROM ""comic"".""ComicSubPostComments"" spc
                                                         JOIN ""comic"".""ComicSubPosts"" sp ON spc.""PostId""= sp.""Id""
                                                         JOIN ""comic"".""ComicPosts"" p ON sp.""PostId""= p.""Id"" 
                                                         WHERE p.""HashId"" = @HashId
                                                         AND spc.""IsDelete"" = false) AS total_comment_count";
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
                        p.""Hide"",
                        p.""ExternalResource"",
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
                        sp.""Sort"",
                        sp.""UserId"",
                        COUNT(DISTINCT spcm.""Id"") as ""CommentCount"",
                        subpostview.""ViewCount"",
                        sp.""CreatedOn"",
                        sp.""PublishDate"",
                        ux.""Id"" as ""UserExclusiveId"" ,
                        sp.""CreatorNote"",
                        sp.""IsEnableComment""
                        FROM ""comic"".""ComicPosts"" p
                        LEFT JOIN identity.""Users"" u ON p.""UserId"" = u.""Id""
                        LEFT JOIN ""comic"".""ComicTagPosts"" tp ON tp.""PostId"" = p.""Id"" AND tp.""IsDelete"" = false
                        LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id""
                        LEFT JOIN ""comic"".""ComicSubPosts"" sp ON sp.""PostId"" = p.""Id"" AND sp.""IsDelete"" = false
                        AND sp.""Status"" = ANY (@PostStatus) AND sp.""PublishDate"" <= @CurrentDate
                        [WithPermission]  [Not-load-chapter]
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
                            AND (NOT (p.""Hide"" = ANY (@Hide) AND p.""Hide"" = ANY (@Hide) IS NOT NULL))
                            AND p.""Status"" = ANY (@PostStatus)
                        -- TODO AND (@IsAccessPrivate = true OR p.""IsPrivate"" = false )
                        GROUP BY p.""Id"",p.""Title"", p.""Body"", p.""HashId"", p.""Permission"",p.""UserId"",
                        p.""IsMature"",p.""IsCompleted"",postview.""ViewCount"", p. ""Hide"", p.""ExternalResource"",
                        p.""AuthorId"",p.""AuthorName"",u.""ProfileName"", u.""UserName"" ,u.""ProfileId"",u.""Avatar"", p.""CreatedOn"",
                        p.""Status"", p.""Type"", p.""CreatedOn"",sp.""Id"",sp.""HashId"",sp.""Title"",sp.""Order"", sp.""Status"", sp.""IsPremium"" ,ux.""Id"",
                        sp.""Permission"", sp.""UserId"",subpostview.""ViewCount"",
                        sp.""PublishDate""
                        ORDER BY sp.""Sort""
                        ";
            }
        }

        private string GetRelatedPostQuery => @"SELECT post.""SelectType"",post.""Id"",post.""Title"", post.""Body"", post.""HashId"",
                        post.""UserId"", post.""ProfileName"",post.""ProfileId"",post.""Avatar"" as ""UserAvatar"",post.""UserName"", post.""ThumbnailUrl"", 
                        post.""ChapterCount"",
                        post.""Status"", post.""Type"",post.""ViewCount"",post.""TotalComment"",
                        post.""CreatedOn"",post.""AuthorName"", post.""CoverUrl"", post.""IsMature"", post.""IsCompleted"", post.""Permission"", post.""AuthorId"",
                        post.""SubPostStr"", 
                        array_agg(DISTINCT tag.""Name"") as Tags from
                            (SELECT  p.""Id"",
                            p.""Title"", p.""Body"",  
                            p.""HashId"",p.""UserId"", sp.""Total"" AS ""ChapterCount"",
                            u.""ProfileName"",u.""ProfileId"",u.""Avatar"",u.""UserName"", p.""ThumbnailUrl"", 
                            p.""AuthorName"", p.""CoverUrl"", p.""IsMature"", p.""IsCompleted"", p.""Permission"",p.""AuthorId"",
                             postid.""SelectType"",
                            p.""Status"", p.""Type"", postview.""ViewCount"",
                            p.""CreatedOn"",
                               (
               SELECT COUNT(*) 
               FROM ""comic"".""ComicPostComments"" pc 
               WHERE pc.""PostId"" = p.""Id"" AND pc.""IsDelete"" = FALSE 
                    AND (NOT (p.""Hide"" = ANY (@Hide) AND p.""Hide"" = ANY (@Hide) IS NOT NULL) OR (p.""UserId"" = @UserId))
                    AND p.""Status"" = ANY (@PostStatus)
           ) + (
               SELECT COUNT(*)
                FROM ""comic"".""ComicSubPostComments"" spc
               INNER JOIN ""comic"".""ComicSubPosts"" sp ON spc.""PostId"" = sp.""Id""
               WHERE sp.""PostId"" = p.""Id"" AND spc.""IsDelete"" = FALSE 
                    AND (NOT (p.""Hide"" = ANY (@Hide) AND p.""Hide"" = ANY (@Hide) IS NOT NULL) OR (p.""UserId"" = @UserId))
                    AND p.""Status"" = ANY (@PostStatus)
           ) AS ""TotalComment"",
                            to_jsonb(array_agg(sp.*)) AS ""SubPostStr"" 
                             
                            FROM ""comic"".""ComicPosts"" p
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
                            
                            WHERE (NOT (p.""Hide"" = ANY (@Hide) AND p.""Hide"" = ANY (@Hide) IS NOT NULL) OR (p.""UserId"" = @UserId))
                            AND p.""Status"" = ANY (@PostStatus)
                            GROUP BY postid.""SelectType"", p.""Id"",p.""Title"", p.""Body"", p.""HashId"", p.""UserId"", 
                            p.""AuthorName"", p.""CoverUrl"", p.""IsMature"",p.""IsCompleted"", p.""Permission"",p.""AuthorId"",
                            sp.""Total"",
                            u.""ProfileName"", u.""ProfileId"",u.""Avatar"",u.""UserName"", p.""ThumbnailUrl"", 
                            p.""Status"", p.""Type"",postview.""ViewCount"",
                            p.""CreatedOn""
                            ) 
                        AS post
                        LEFT JOIN ""comic"".""ComicTagPosts"" tp ON tp.""PostId"" = post.""Id""
                        LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id"" 
                        GROUP BY post.""SelectType"", post.""Id"",post.""Title"", post.""Body"", post.""HashId"", 
                        post.""AuthorName"", post.""CoverUrl"", post.""IsMature"",post.""IsCompleted"", post.""Permission"",post.""AuthorId"",
                        post.""UserId"",post.""ProfileName"",post.""ProfileId"",post.""Avatar"", post.""ThumbnailUrl"",post.""UserName"", post.""ChapterCount"", post.""TotalComment"",
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
                        post.""SubPostStr"",post.""LatestCreatedOn"", post.""Hide"", post.""ExternalResource"",
                        array_agg(DISTINCT tag.""Name"") as Tags from
                            (SELECT  p.""Id"",
                            p.""Title"", p.""Body"",  
                            p.""HashId"",p.""UserId"", sp.""Total"" AS ""ChapterCount"",
                            u.""ProfileName"",u.""ProfileId"",u.""Avatar"", p.""ThumbnailUrl"", 
                            p.""AuthorName"", p.""CoverUrl"", p.""IsMature"", p.""IsCompleted"", p.""Permission"", p.""AuthorId"",
                             postid.""SelectType"",
                            p.""Status"", p.""Type"", postview.""ViewCount"",
                            p.""CreatedOn"", p.""Hide"", p.""ExternalResource"", --sp.""Id"" as ""SPID"",
SUM(""CommentCount"") as ""TotalSubPostComment"",
                            --sp.""ChapterCount"" AS ""ChapterCount"",
                            to_jsonb(array_agg(sp.*)) AS ""SubPostStr"",
                            GREATEST(p.""[OrderBy]"", MAX(sp.""PublishDate"")) AS ""LatestCreatedOn""
                            FROM ""comic"".""ComicPosts"" p
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
                            WHERE NOT (p.""Hide"" = ANY (@Hide) AND p.""Hide"" = ANY (@Hide) IS NOT NULL) [AddNewUserNameContidion]
                            AND p.""Status"" = ANY (@PostStatus)
                            AND p.""Permission""= @PostPermission
                            GROUP BY postid.""SelectType"", p.""Id"",p.""Title"", p.""Body"", p.""HashId"", p.""UserId"", 
                            p.""AuthorName"", p.""CoverUrl"", p.""IsMature"",p.""IsCompleted"", p.""Permission"",p.""AuthorId"",
                            sp.""Total"",
                            u.""ProfileName"", u.""ProfileId"",u.""Avatar"", p.""ThumbnailUrl"", 
                            p.""Status"", p.""Type"",postview.""ViewCount"", p.""Hide"",
                            p.""CreatedOn""
                            ) 
                        AS post
                        LEFT JOIN identity.""Users"" u ON post.""UserId"" = u.""Id"" AND u.""IsDelete"" = false
                        LEFT JOIN ""comic"".""ComicTagPosts"" tp ON tp.""PostId"" = post.""Id""AND tp.""IsDelete"" = false
                        LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id"" 
                        LEFT JOIN ""comic"".""ComicPostComments"" comment ON comment.""PostId"" = post.""Id"" AND comment.""IsDelete"" = false
                        GROUP BY post.""SelectType"", post.""Id"",post.""Title"", post.""Body"", post.""HashId"", 
                        post.""AuthorName"", post.""CoverUrl"", post.""IsMature"",post.""IsCompleted"", post.""Permission"",post.""AuthorId"",
                        post.""UserId"",post.""ProfileName"",post.""ProfileId"",post.""Avatar"", post.""ThumbnailUrl"", post.""ChapterCount"",post.""TotalSubPostComment"",
                        post.""Status"", post.""Type"", post.""ViewCount"", post.""Hide"", post.""ExternalResource"",
                        post.""CreatedOn"",
                        post.""SubPostStr"",
                        u.""UserName"",
                        post.""LatestCreatedOn""
                        HAVING post.""ChapterCount"" > 0
                        ORDER BY ""LatestCreatedOn"" DESC;

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
                                SELECT sp.""Id"",sp.""HashId"",sp.""PostId"",sp.""CreatedOn"",sp.""Title"",sp.""Order"", sp.""IsExclusive"", sp.""PublishDate"",
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
                                WHERE sp.""PostId"" = p.""Id"" AND sp.""IsDelete"" = false AND sp.""Permission"" = @PostPermission
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

                                FROM ""comic"".""ComicPosts"" qpost
                                 INNER JOIN ""comic"".""ComicPostComments"" pcm ON pcm.""PostId"" = qpost.""Id"" 
                                AND pcm.""CreatedOn"" > @LastWeek
                                WHERE  qpost.""Type"" = @PostType AND qpost.""Status"" = ANY (@PostStatus) AND pcm.""IsDelete"" = false 
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
                                FROM ""comic"".""ComicPosts"" qpost
                                 INNER JOIN ""comic"".""ComicPostComments"" pcm ON pcm.""PostId"" = qpost.""Id"" 
                                AND pcm.""CreatedOn"" > @LastWeek
                                WHERE  qpost.""Type"" = @PostType AND qpost.""Status"" = ANY (@PostStatus) AND pcm.""IsDelete"" = false 
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
                                 FROM ""comic"".""ComicPosts"" qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId"", sp1.""CreatedOn"" 
                                    FROM ""comic"".""ComicSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  
                                    AND sp1.""Status"" = ANY (@PostStatus)
                                    GROUP BY sp1.""Id"",sp1.""PostId"",sp1.""CreatedOn""
                                    ORDER BY sp1.""CreatedOn"" DESC
                                    LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
                                WHERE  qpost1.""Type"" = @PostType 
                                AND qpost1.""Status"" = ANY (@PostStatus)
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
                                 FROM ""comic"".""ComicPosts"" qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId""
                                    FROM ""comic"".""ComicSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  
                                    AND sp1.""Status"" = ANY (@PostStatus)
                                    GROUP BY sp1.""Id"", sp1.""PostId""
                                    -- LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
                                WHERE qpost1.""Type"" = @PostType 
                                AND qpost1.""Status"" = ANY (@PostStatus)
                                AND qpost1.""IsDelete"" = false                                 
                                GROUP BY qpost1.""Id""";
            }
        }
        #endregion

        #region Top latestByTag

        private string GetTopLatestPostByMultiTagQuery
        {
            get
            {
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, psp1.""CreatedOn"", 1 AS ""SelectType""
                                 FROM ""comic"".""ComicPosts"" qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId"", sp1.""CreatedOn"" 
                                    FROM ""comic"".""ComicSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false 
                                    AND sp1.""Status"" = ANY (@PostStatus) AND sp1.""Permission"" = @PostPermission
                                    AND (sp1.""PublishDate"" IS NULL OR sp1.""PublishDate"" < TIMEZONE('UTC', now()))
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

        private string GetTopLatestPostByMultiTagToCountQuery
        {
            get
            {
                return @"SELECT qpost1.""Id""
                                 FROM ""comic"".""ComicPosts"" qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId""
                                    FROM ""comic"".""ComicSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false 
                                    AND sp1.""Status"" = ANY (@PostStatus) AND sp1.""Permission"" = @PostPermission
                                    AND (sp1.""PublishDate"" IS NULL OR sp1.""PublishDate"" < TIMEZONE('UTC', now()))
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

        #region Top completed
        private string GetTopLatestCompletePostHitQuery
        {
            get
            {
                return @"SELECT qpost1.""Id"", 0 as COUNTCM, qpost1.""CreatedOn"", 2 AS ""SelectType""
                                 FROM ""comic"".""ComicPosts"" qpost1

                                WHERE  qpost1.""Type"" = @PostType 
1                               AND qpost1.""Status"" = ANY (@PostStatus)
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
                                 FROM ""comic"".""ComicPosts"" qpost2
                                WHERE  qpost2.""Type"" = @PostType 
                                AND qpost2.""Status"" = ANY (@PostStatus)
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
                                FROM ""comic"".""ComicPosts"" qpost1                                 
                                WHERE  qpost1.""Type"" = @PostType AND qpost1.""Status"" = ANY (@PostStatus)
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
                                 FROM ""comic"".""ComicPosts"" qpost1        
                                 INNER JOIN ""comic"".""ComicTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id"" AND qtp.""IsDelete"" = false
                                INNER JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
                                WHERE  qtag.""Name"" = @TagName AND qpost1.""Type"" = @PostType 
                                AND qpost1.""Status"" = ANY (@PostStatus)
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
                                 FROM ""comic"".""ComicPosts"" qpost1
                                 INNER JOIN ""comic"".""ComicTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id"" AND qtp.""IsDelete"" = false
                                INNER JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
                                WHERE  qtag.""Name"" = @TagName AND qpost1.""Type"" = @PostType 
                                AND qpost1.""Status"" = ANY (@PostStatus) AND qpost1.""IsDelete"" = false                            
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
                                FROM ""comic"".""ComicPosts"" qpost
                                LEFT JOIN ""comic"".""ComicPostFavorites"" cfp on qpost.""Id"" = cfp.""PostId""
                                WHERE cfp.""UserId"" = @UserId AND cfp.""IsDelete"" = false  AND qpost.""IsDelete"" = false 
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
                                FROM ""comic"".""ComicPosts"" qpost 
                                LEFT JOIN ""comic"".""ComicPostFavorites"" cfp on qpost.""Id"" = cfp.""PostId""
                                WHERE cfp.""UserId"" = @UserId AND cfp.""IsDelete"" = false  AND qpost.""IsDelete"" = false
                                AND NOT (qpost.""Hide"" = ANY (@Hide) AND qpost.""Hide"" IS NOT NULL)
                                AND qpost.""Status"" = ANY (@PostStatus)";
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
                                FROM ""comic"".""ComicPosts"" qpost
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
                                FROM ""comic"".""ComicPosts"" qpost 
                                WHERE qpost.""CreatedBy"" = @UserId AND qpost.""Type"" = @PostType AND qpost.""IsDelete"" = false";
            }
        }
        private string GetMyAllQuery
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
                                 FROM ""comic"".""ComicPosts"" qpost1        
                                 INNER JOIN identity.""Users"" user1 ON user1.""Id"" = qpost1.""UserId"" 
                                 LEFT JOIN ""comic"".""ComicSubPosts"" sp ON sp.""PostId"" = qpost1.""Id"" AND sp.""IsDelete"" = false
                                 AND sp.""Permission"" != 1 AND sp.""Status"" = ANY (@PostStatus)
                                 AND (sp.""PublishDate"" IS NULL OR sp.""PublishDate"" < TIMEZONE('UTC', now()))
                                WHERE  user1.""UserName"" = @ProfileName AND qpost1.""Type"" = @PostType 
                                AND qpost1.""Status"" = ANY (@PostStatus)
                                AND qpost1.""IsDelete"" = false                                 
                                                            
                                GROUP BY qpost1.""Id"", qpost1.""ModifiedOn""
                                HAVING COUNT(sp.""Id"") > 0
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
                                 FROM ""comic"".""ComicPosts"" qpost1
                                 INNER JOIN identity.""Users"" user1 ON user1.""Id"" = qpost1.""UserId"" 
                                 LEFT JOIN ""comic"".""ComicSubPosts"" sp ON sp.""PostId"" = qpost1.""Id"" AND sp.""IsDelete"" = false
                                 AND sp.""Permission"" != 1 AND sp.""Status"" = ANY (@PostStatus)
                                 AND (sp.""PublishDate"" IS NULL OR sp.""PublishDate"" < TIMEZONE('UTC', now()))
                                WHERE  user1.""UserName"" = @ProfileName AND qpost1.""Type"" = @PostType 
                                AND qpost1.""Status"" = ANY (@PostStatus)
                                AND qpost1.""IsDelete"" = false
                                GROUP BY qpost1.""Id""
                                HAVING COUNT(sp.""Id"") > 0";
            }
        }
        #endregion

        #region SubPost - View

        private string GetSeriesChapterByHashIdWithJoinOrder
        {
            get
            {
                return @"SELECT sp.""Id"",sp.""HashId"",sp.""Name"", sp.""Title"", sp.""PostId"", sp.""Order"", sp.""Body"", sp.""IsPremium"", sp.""IsExclusive"", ux.""Id"" as ""UserExclusiveId"",
                    sp.""Status"", sp.""CreatedOn"", sp.""CreatedBy"", sp.""ModifiedOn"", 
                    sp.""ModifiedBy"", sp.""IsDelete"", count.""ViewCount"", sp.""AuthorId"", u.""ProfileName"",
                    sp.""UserId"", sp.""PublishDate"", sp.""Permission"",sp.""CreatorNote"",
                    sp.""IsEnableComment"", spcmc.""CommentCount"",
                    rs.""Id"", rs.""AuthorId"", rs.""Title"", rs.""Name"", rs.""Url"", rs.""Type"", rs.""CreatedOn"", 
                    rs.""CreatedBy"", rs.""ModifiedOn"", rs.""ModifiedBy"", rs.""IsDelete"", rs.""HashId"", rs.""SubPostId"", 
                    rs.""Status"", rs.""Size"", rs.""LocationType"", rs.""Height"", rs.""Width"", rs.""Order"", rs.""MinioInstance"", rs.""BucketName""
                    FROM ""comic"".""ComicSubPosts"" sp
                    INNER JOIN ""comic"".""ComicPosts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
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
                            AND (NOT (p.""Hide"" = ANY (@Hide) AND p.""Hide"" = ANY (@Hide) IS NOT NULL) OR (u.""Id"" = @UserId))
                            AND sp.""Status"" = ANY (
                            CASE 
                                WHEN u.""Id"" = @UserId THEN @PostStatusForAuthor 
                                ELSE @PostStatus 
                            END
                            )
                    ORDER BY rs.""Order"";";
            }
        }

        private string GetSeriesChaptersByHashId
        {
            get
            {
                return @"SELECT sp.""Id"",sp.""HashId"",sp.""Name"", sp.""Title"", sp.""PostId"", sp.""Order"", sp.""Sort"", sp.""Body"",sp.""IsPremium"", sp.""IsExclusive"",ux.""Id"" as ""UserExclusiveId"",
                sp.""Status"", sp.""CreatedOn"", sp.""CreatedBy"", sp.""ModifiedOn"", 
                sp.""ModifiedBy"", sp.""IsDelete"", count.""ViewCount"", sp.""AuthorId"", 
                sp.""UserId"", sp.""PublishDate"", sp.""Permission"", sp.""CreatorNote"",
sp.""IsEnableComment"", spcmc.""CommentCount""
                    FROM ""comic"".""ComicSubPosts"" sp
LEFT JOIN ""UserExclusiveSubPosts"" ux ON ux.""SubPostId"" = sp.""Id"" AND ux.""UserId"" = @UserId
                    INNER JOIN ""comic"".""ComicPosts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
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
                                INNER JOIN ""comic"".""ComicPosts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
                                WHERE p.""HashId"" = @PostHashId AND sp.""IsDelete"" = false
                            ) p;
";
            }
        }

        private string GetSeriesChaptersSimpleByHashId
        {
            get
            {
                return $@"SELECT sp.""Id"", sp.""Title"", sp.""Order"", sp.""IsPremium""
                    FROM ""comic"".""ComicSubPosts"" sp
                    INNER JOIN ""comic"".""ComicPosts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
                    WHERE p.""HashId"" = @PostHashId AND sp.""IsDelete"" = false AND (sp.""Status"" = ANY (@PostStatus) OR sp.""UserId"" = @UserId)
                    AND sp.""PublishDate"" < @CurrentDate AND sp.""Permission"" != {(int)PostPermission.Private}
                    ORDER BY sp.""Sort"";

                        SELECT COUNT(*) AS TotalItems 
                        FROM (
                                SELECT sp.""Id"" 
                                FROM ""comic"".""ComicSubPosts"" sp
                                INNER JOIN ""comic"".""ComicPosts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
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
    
                    UPDATE ""comic"".""ComicPostFavorites""
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
        #endregion

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
                    p.""UserId"",
                    u.""ProfileName"",
                    u.""UserName"",
                    p.""ViewCount"",
                    p.""IsMature"",
                    p.""CreatedOn"",
                    GREATEST(p.""CreatedOn"", COALESCE(sp_max.""LatestSubPostPublishDate"", p.""CreatedOn"")) AS ""LatestCreatedOn"",
                    p.""Hide"",
                    p.""Status"",
                    p.""ExternalResource"",
                    to_json(array_agg(distinct(sp.*)) FILTER (WHERE sp.* IS NOT NULL AND sp.""PublishDate"" < @CurrentDate))AS ""SubPosts"",
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
                           ""PublishDate"",
                           ROW_NUMBER() OVER (PARTITION BY ""PostId"" ORDER BY ""Order"" desc) AS rn
                    FROM ""comic"".""ComicSubPosts""
                    WHERE ""IsDelete"" = false AND ""Permission"" = @Permission 
                    AND (""PublishDate"" IS NULL OR ""PublishDate"" < TIMEZONE('UTC', now()))
                ) sp ON p.""Id"" = sp.""PostId""
                LEFT JOIN (
                    SELECT ""PostId"", MAX(""PublishDate"") AS ""LatestSubPostPublishDate""
                    FROM ""comic"".""ComicSubPosts""
                    WHERE ""IsDelete"" = false AND ""PublishDate"" < @CurrentDate AND ""Permission"" = @Permission
                    AND (""PublishDate"" IS NULL OR ""PublishDate"" < TIMEZONE('UTC', now()))
                    GROUP BY ""PostId""
                ) sp_max ON p.""Id"" = sp_max.""PostId""
                WHERE p.""HashId"" = ANY(@HashIds) AND NOT (p.""Hide"" = ANY (@Hide) AND p.""Hide"" = ANY (@Hide) IS NOT NULL)
                AND p.""Status"" = ANY (@PostStatus)
                GROUP BY  
                          p.""HashId"",
                          p.""Id"",
                          p.""ThumbnailUrl"",
                          p.""Body"",
                          p.""Title"",
                          p.""AuthorId"",
                          p.""AuthorName"",
                          p.""UserId"",
                          u.""ProfileName"",
                          u.""UserName"",
                          p.""ViewCount"",
                          sp_max.""LatestSubPostPublishDate"",
                          p.""Hide"",
                          p.""Status"",
                          p.""ExternalResource""
            ";
            }
        }
    }
}
