namespace Mcsg.Social.Api.Services
{
    using Common.Core.Enums;

    public partial class PostService
    {
        private string GetTopAllPostAllTypeByTagQuery
        {
            get
            {
                return @"SELECT post.""SelectType"",post.""Id"",post.""Title"", post.""Body"", post.""HashId"",
                        post.""UserId"", post.""ProfileName"",post.""ProfileId"",post.""Avatar"" as ""UserAvatar"", post.""ThumbnailUrl"", 
                        post.""ChapterCount"",
                        post.""Status"", post.""Type"",post.""ViewCount"",post.""TotalSubPostComment"",
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
                             
                            FROM social.""SocialPosts"" p
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
                        LEFT JOIN social.""SocialTagPosts"" tp ON tp.""PostId"" = post.""Id""
                        LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id"" 
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
                                FROM social.""SocialSubPosts"" sp 
--Comment count
LEFT JOIN social.""SocialSubPostComments"" spcm ON spcm.""PostId"" = sp.""Id"" AND spcm.""IsDelete"" = false
--View count
LEFT JOIN LATERAL (
                                SELECT 
                                ""EntityId"", 
                                ""Count"" as ""ViewCount""
                                    FROM ""SmartCountActions"" 
                                WHERE ""EntityId"" = sp.""Id"" AND ""EntityType"" = 1 AND ""ActionType"" = 2
LIMIT 1
                                ) subpostview ON subpostview.""EntityId"" = sp.""Id""
                                WHERE sp.""PostId"" = p.""Id"" AND sp.""IsDelete"" = false
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

                                FROM social.""SocialPosts"" qpost
                                 INNER JOIN social.""SocialPostComments"" pcm ON pcm.""PostId"" = qpost.""Id"" 
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
                                FROM social.""SocialPosts"" qpost
                                 INNER JOIN social.""SocialPostComments"" pcm ON pcm.""PostId"" = qpost.""Id"" 
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
                                 FROM social.""SocialPosts"" qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId"", sp1.""CreatedOn"" 
                                    FROM social.""SocialSubPosts"" sp1 
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
                                 FROM social.""SocialPosts"" qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId""
                                    FROM social.""SocialSubPosts"" sp1 
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

        #region Top completed
        private string GetTopLatestCompletePostHitQuery
        {
            get
            {
                return @"SELECT qpost1.""Id"", 0 as COUNTCM, qpost1.""CreatedOn"", 2 AS ""SelectType""
                                 FROM social.""SocialPosts"" qpost1

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
                                 FROM social.""SocialPosts"" qpost2
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
                                FROM social.""SocialPosts"" qpost1                                 
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
                                 FROM social.""SocialPosts"" qpost1        
                                 INNER JOIN social.""SocialTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
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
                                 FROM social.""SocialPosts"" qpost1
                                 INNER JOIN social.""SocialTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
                                INNER JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
                                WHERE  qtag.""Name"" = @TagName AND qpost1.""Type"" = @PostType 
AND qpost1.""Status"" = @PostStatus AND qpost1.""IsDelete"" = false                            
                                GROUP BY qpost1.""Id""";
            }
        }

        #endregion

        #region My series
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
                                 FROM social.""SocialPosts"" qpost1        
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
                                 FROM social.""SocialPosts"" qpost1
                                 INNER JOIN identity.""Users"" user1 ON user1.""Id"" = qpost1.""UserId""                                
                                WHERE  user1.""UserName"" = @ProfileName AND qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
                                AND qpost1.""IsDelete"" = false
                                GROUP BY qpost1.""Id""";
            }
        }
        #endregion

        #region SubPost - View
        private string GetSeriesChaptersWithOffsetSimpleByHashId
        {
            get
            {
                return $@"SELECT sp.""Id"", sp.""Title"", sp.""PublishDate"", sp.""Order""
                    FROM ""social"".""SocialSubPosts"" sp
                    INNER JOIN {_postRepository.TableName} p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
                    WHERE p.""CreatedBy"" = @UserId AND p.""HashId"" = @PostHashId AND sp.""IsDelete"" = false
                    ORDER BY sp.""Order""
                    LIMIT @PageSize
                    OFFSET @Offet;

                    SELECT COUNT(*) AS TotalItems 
                    FROM (
                            SELECT sp.""Id""
                            FROM ""social"".""SocialSubPosts"" sp
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
                return @"UPDATE social.""SocialPosts""
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""Id"" = @PostId;

                    UPDATE social.""SocialResources""
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    FROM (SELECT ""Id""
                          FROM social.""SocialSubPosts"" WHERE ""PostId"" = @PostId) AS sp
                    WHERE social.""SocialResources"".""SubPostId"" = sp.""Id"";
    
                    UPDATE social.""SocialSubPostReactions"" spr
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    FROM (SELECT ""Id""
                          FROM social.""SocialSubPosts"" WHERE ""PostId"" = @PostId) AS sp
                    WHERE spr.""TargetId"" = sp.""Id"";
    
                    UPDATE social.""SocialSubPostComments"" spr
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    FROM (SELECT ""Id""
                          FROM social.""SocialSubPosts"" WHERE ""PostId"" = @PostId) AS sp
                    WHERE spr.""PostId"" = sp.""Id"";
    
                    UPDATE social.""SocialSubPostCommentReactions"" spr
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    FROM (SELECT ""Id""
                          FROM social.""SocialSubPosts"" WHERE ""PostId"" = @PostId) AS sp
                    WHERE spr.""TargetId"" = sp.""Id"";
    
                    UPDATE social.""SocialTagPosts""
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""PostId"" = @PostId;
    
                    UPDATE social.""SocialSubPosts""
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""PostId"" = @PostId;
    
                    UPDATE social.""SocialPostReactions""
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""TargetId"" = @PostId;
    
                    UPDATE social.""SocialPostComments""
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""PostId"" = @PostId;";
            }
        }


        #region Delete Subpost
        private string GetLatestPostsByTagQuery
        {
            get
            {
                return $@"
                     WITH ranked_feed AS (
    SELECT p.""Id"", p.""CreatedOn"", p.""HashId"",
        ROW_NUMBER() OVER (ORDER BY p.""CreatedOn"" DESC) AS type_rank
    FROM social.""SocialPosts"" p
    LEFT JOIN social.""SocialTagPosts"" tp ON p.""Id"" = tp.""PostId"" AND tp.""IsDelete"" = false 
    LEFT JOIN ""Tags"" t ON t.""Id"" = tp.""TagId""
    WHERE p.""IsDelete"" = false
    AND p.""Type"" = 0
    AND p.""Status"" = 1
    AND t.""Name"" ILIKE @ExactKeyword   
),
ranked_story AS (
    SELECT sp.""Id"", 
        GREATEST(sp.""CreatedOn"", COALESCE(MAX(ssp.""CreatedOn""), sp.""CreatedOn"")) AS ""CreatedOn"", 
        sp.""HashId"",
        ROW_NUMBER() OVER (ORDER BY GREATEST(sp.""CreatedOn"", COALESCE(MAX(ssp.""CreatedOn""), sp.""CreatedOn"")) DESC) AS type_rank
    FROM story.""StoryPosts"" sp
    LEFT JOIN story.""StoryTagPosts"" stp ON sp.""Id"" = stp.""PostId"" AND stp.""IsDelete"" = false 
    LEFT JOIN ""Tags"" t ON t.""Id"" = stp.""TagId""   
    LEFT JOIN story.""StorySubPosts"" ssp ON sp.""Id"" = ssp.""PostId""
    WHERE sp.""IsDelete"" = false
    AND ssp.""IsDelete"" = false
    AND sp.""Type"" = 1
    AND sp.""Status"" = 1 AND ssp.""Status"" = ANY(@Status)
    AND sp.""Permission"" = 0 AND ssp.""Permission"" = 0
    AND (ssp.""PublishDate"" IS NULL OR ssp.""PublishDate"" < TIMEZONE('UTC', now()))
    AND t.""Name"" ILIKE @ExactKeyword
    GROUP BY sp.""Id"", sp.""CreatedOn"", sp.""HashId""
),
ranked_comic AS (
    SELECT cp.""Id"", 
        GREATEST(cp.""CreatedOn"", COALESCE(MAX(csp.""CreatedOn""), cp.""CreatedOn"")) AS ""CreatedOn"", 
        cp.""HashId"",
        ROW_NUMBER() OVER (ORDER BY GREATEST(cp.""CreatedOn"", COALESCE(MAX(csp.""CreatedOn""), cp.""CreatedOn"")) DESC) AS type_rank
    FROM comic.""ComicPosts"" cp
    LEFT JOIN comic.""ComicTagPosts"" ctp ON cp.""Id"" = ctp.""PostId"" AND ctp.""IsDelete"" = false 
    LEFT JOIN ""Tags"" t ON t.""Id"" = ctp.""TagId""
    LEFT JOIN comic.""ComicSubPosts"" csp ON cp.""Id"" = csp.""PostId""
    WHERE cp.""IsDelete"" = false
    AND csp.""IsDelete"" = false
    AND cp.""Type"" = 2
    AND cp.""Status"" = 1 AND csp.""Status"" = ANY(@Status)
    AND cp.""Permission"" = 0 AND csp.""Permission"" = 0
    AND (csp.""PublishDate"" IS NULL OR csp.""PublishDate"" < TIMEZONE('UTC', now()))
    AND t.""Name"" ILIKE @ExactKeyword   
    GROUP BY cp.""Id"", cp.""CreatedOn"", cp.""HashId""
),
ranked_document AS (
    SELECT dp.""Id"", 
        GREATEST(dp.""CreatedOn"", COALESCE(MAX(dsp.""CreatedOn""), dp.""CreatedOn"")) AS ""CreatedOn"",
        dp.""HashId"",
        ROW_NUMBER() OVER (ORDER BY GREATEST(dp.""CreatedOn"", COALESCE(MAX(dsp.""CreatedOn""), dp.""CreatedOn"")) DESC) AS type_rank
    FROM document.""DocumentPosts"" dp
    LEFT JOIN document.""DocumentTagPosts"" dtp ON dp.""Id"" = dtp.""PostId"" AND dtp.""IsDelete"" = false
    LEFT JOIN ""Tags"" t ON t.""Id"" = dtp.""TagId""
    LEFT JOIN document.""DocumentSubPosts"" dsp ON dp.""Id"" = dsp.""PostId""
    WHERE dp.""IsDelete"" = false
    AND dsp.""IsDelete"" = false
    AND dp.""Type"" = 4
    AND dp.""Status"" = 1 AND dsp.""Status"" = ANY(@Status)
    AND dp.""Permission"" = 0 AND dsp.""Permission"" = 0
    AND (dsp.""PublishDate"" IS NULL OR dsp.""PublishDate"" < TIMEZONE('UTC', now()))
    AND t.""Name"" ILIKE @ExactKeyword
    GROUP BY dp.""Id"", dp.""CreatedOn"", dp.""HashId""
),
limited_feed AS (
    SELECT ""Id"", ""CreatedOn"", ""HashId""
    FROM ranked_feed
    WHERE type_rank <= @feed
),
limited_story AS (
    SELECT ""Id"", ""CreatedOn"", ""HashId""
    FROM ranked_story
    WHERE type_rank <= @story
),
limited_comic AS (
    SELECT ""Id"", ""CreatedOn"", ""HashId""
    FROM ranked_comic
    WHERE type_rank <= @comic
),
limited_document AS (
    SELECT ""Id"", ""CreatedOn"", ""HashId""
    FROM ranked_document
    WHERE type_rank <= @document
),
combined_posts AS (
    SELECT ""Id"", ""CreatedOn"", ""HashId"", 0 AS ""Type"" FROM limited_feed
    UNION ALL
    SELECT ""Id"", ""CreatedOn"", ""HashId"", 1 AS ""Type"" FROM limited_story
    UNION ALL
    SELECT ""Id"", ""CreatedOn"", ""HashId"", 2 AS ""Type"" FROM limited_comic
    UNION ALL
    SELECT ""Id"", ""CreatedOn"", ""HashId"", 4 AS ""Type"" FROM limited_document
),
numbered_posts AS (
    SELECT ""Id"", ""Type"", ""CreatedOn"", ""HashId"",
        ROW_NUMBER() OVER (PARTITION BY ""Type"" ORDER BY ""CreatedOn"" DESC) AS num
    FROM combined_posts
),
grouped_posts AS (
    SELECT ""Id"", ""Type"", ""CreatedOn"", ""HashId"",
        CEILING(CAST(num AS FLOAT) / 
        CASE
            WHEN ""Type"" = 0 THEN @feedPercent * 10
            WHEN ""Type"" = 1 THEN @storyPercent * 10
            WHEN ""Type"" = 2 THEN @comicPercent * 10
            WHEN ""Type"" = 4 THEN @documentPercent * 10
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
        
        [GetTotalCount]
        ";
            }
        }
        #endregion

        private string GetCountPostByTagQuery => $@"SELECT (
                                                    (SELECT COUNT(*) 
                                                   FROM ""comic"".""ComicPosts"" p
                                                   LEFT JOIN comic.""ComicTagPosts"" tp on p.""Id"" = tp.""PostId"" AND tp.""IsDelete"" = false 
                                                   LEFT JOIN ""Tags"" t on t.""Id"" = tp.""TagId""
                                                   WHERE p.""IsDelete"" = false 
                                                   AND p.""Status"" = {(int)PostStatus.Public}
                                                   AND p. ""Permission"" = 0
                                                   AND t.""Name"" ILIKE @ExactKeyword)
                                                    +
                                                    (SELECT COUNT(*) 
                                                   FROM ""story"".""StoryPosts"" p
                                                     LEFT JOIN story.""StoryTagPosts"" tp on p.""Id"" = tp.""PostId"" AND tp.""IsDelete"" = false 
                                                   LEFT JOIN ""Tags"" t on t.""Id"" = tp.""TagId""
                                                   WHERE p.""IsDelete"" = false 
                                                   AND p.""Status"" = {(int)PostStatus.Public}
                                                   AND p. ""Permission"" = 0
                                                   AND t.""Name"" ILIKE @ExactKeyword)
                                                    +
                                                    (SELECT COUNT(*)
                                                   FROM social.""SocialPosts"" p
                                                   LEFT JOIN social.""SocialTagPosts"" tp on p.""Id"" = tp.""PostId"" AND tp.""IsDelete"" = false 
                                                   LEFT JOIN ""Tags"" t on t.""Id"" = tp.""TagId""
                                                   WHERE p.""IsDelete"" = false 
                                                   AND p.""Status"" = {(int)PostStatus.Public}
                                                   AND p. ""Permission"" = 0
                                                   AND t.""Name"" ILIKE @ExactKeyword))
                                                    +
                                                    (SELECT COUNT(*) 
                                                   FROM ""document"".""DocumentPosts"" p
                                                     LEFT JOIN document.""DocumentTagPosts"" tp on p.""Id"" = tp.""PostId"" AND tp.""IsDelete"" = false 
                                                   LEFT JOIN ""Tags"" t on t.""Id"" = tp.""TagId""
                                                   WHERE p.""IsDelete"" = false 
                                                   AND p.""Status"" = {(int)PostStatus.Public}
                                                   AND p. ""Permission"" = 0
                                                   AND t.""Name"" ILIKE @ExactKeyword)";

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
                    p.""UserId"",
                    p.""AuthorName"",
                    p.""ViewCount"",
                    p.""IsMature"",
                    p.""CreatedOn"",
                    to_json(array_agg(distinct(sp.*)) FILTER (WHERE sp.* IS NOT NULL))AS ""SocialSubPosts"",
                    to_json(array_agg(distinct (t.""Name""))  FILTER (WHERE t.""Name"" IS NOT NULL)) AS ""Tags""
                FROM social.""SocialPosts"" p
                LEFT JOIN social.""SocialTagPosts"" tp ON p.""Id"" = tp.""PostId""
                LEFT JOIN ""Tags"" t ON tp.""TagId"" = t.""Id""
                LEFT JOIN (
                    SELECT ""PostId"",
                           ""Title"",
                           ""Order"",
                           ""CreatedOn"",
                           ROW_NUMBER() OVER (PARTITION BY ""PostId"" ORDER BY ""Order"" desc) AS rn
                    FROM social.""SocialSubPosts""
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
                          p.""UserId"",
                          p.""AuthorName"",
                          p.""ViewCount""
            ";
            }
        }
    }
}
