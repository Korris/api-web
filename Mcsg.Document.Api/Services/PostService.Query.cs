namespace Mcsg.Document.Api.Services
{
    using Common.Core.Enums;

    public partial class PostService
    {
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
               FROM ""document"".""DocumentPostComments"" pc 
               WHERE pc.""PostId"" = p.""Id"" AND pc.""IsDelete"" = FALSE 
                    AND (NOT (p.""Hide"" = ANY (@Hide) AND p.""Hide"" = ANY (@Hide) IS NOT NULL) OR (p.""UserId"" = @UserId))
                    AND p.""Status"" = ANY (@PostStatus)
           ) + (
               SELECT COUNT(*)
                FROM ""document"".""DocumentSubPostComments"" spc
               INNER JOIN ""document"".""DocumentSubPosts"" sp ON spc.""PostId"" = sp.""Id""
               WHERE sp.""PostId"" = p.""Id"" AND spc.""IsDelete"" = FALSE 
                    AND (NOT (p.""Hide"" = ANY (@Hide) AND p.""Hide"" = ANY (@Hide) IS NOT NULL) OR (p.""UserId"" = @UserId))
                    AND p.""Status"" = ANY (@PostStatus)
           ) AS ""TotalComment"",
                            to_jsonb(array_agg(sp.*)) AS ""SubPostStr"" 
                             
                            FROM ""document"".""DocumentPosts"" p
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
                        LEFT JOIN ""document"".""DocumentTagPosts"" tp ON tp.""PostId"" = post.""Id""
                        LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id"" 
                        GROUP BY post.""SelectType"", post.""Id"",post.""Title"", post.""Body"", post.""HashId"", 
                        post.""AuthorName"", post.""CoverUrl"", post.""IsMature"",post.""IsCompleted"", post.""Permission"",post.""AuthorId"",
                        post.""UserId"",post.""ProfileName"",post.""ProfileId"",post.""Avatar"",post.""UserName"", post.""ThumbnailUrl"", post.""ChapterCount"", post.""TotalComment"",
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
                            FROM ""document"".""DocumentPosts"" p
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
                        LEFT JOIN ""document"".""DocumentTagPosts"" tp ON tp.""PostId"" = post.""Id""AND tp.""IsDelete"" = false
                        LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id"" 
                        LEFT JOIN ""document"".""DocumentPostComments"" comment ON comment.""PostId"" = post.""Id"" AND comment.""IsDelete"" = false
                        GROUP BY post.""SelectType"", post.""Id"",post.""Title"", post.""Body"", post.""HashId"", 
                        post.""AuthorName"", post.""CoverUrl"", post.""IsMature"",post.""IsCompleted"", post.""Permission"",post.""AuthorId"",
                        post.""UserId"",post.""ProfileName"",post.""ProfileId"",post.""Avatar"", post.""ThumbnailUrl"", post.""ChapterCount"",post.""TotalSubPostComment"",
                        post.""Status"", post.""Type"", post.""ViewCount"", post.""Hide"", post.""ExternalResource"",
                        post.""CreatedOn"",
                        post.""SubPostStr"",
                        u.""UserName"",
                        post.""LatestCreatedOn""
                        ORDER BY ""LatestCreatedOn"" desc;

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
                                FROM ""document"".""DocumentSubPosts"" sp 
--Comment count
LEFT JOIN ""document"".""DocumentSubPostComments"" spcm ON spcm.""PostId"" = sp.""Id"" AND spcm.""IsDelete"" = false
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

                                FROM ""document"".""DocumentPosts"" qpost
                                 INNER JOIN ""document"".""DocumentPostComments"" pcm ON pcm.""PostId"" = qpost.""Id"" 
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
                                FROM ""document"".""DocumentPosts"" qpost
                                 INNER JOIN ""document"".""DocumentPostComments"" pcm ON pcm.""PostId"" = qpost.""Id"" 
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
                                 FROM ""document"".""DocumentPosts"" qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId"", sp1.""CreatedOn"" 
                                    FROM ""document"".""DocumentSubPosts"" sp1 
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
                                 FROM ""document"".""DocumentPosts"" qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId""
                                    FROM ""document"".""DocumentSubPosts"" sp1 
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

        private string GetTopLatestPostByTagQuery
        {
            get
            {
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, psp1.""CreatedOn"", 1 AS ""SelectType"", qpost1.""Hide""
                                 FROM ""document"".""DocumentPosts"" qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId"", sp1.""CreatedOn"" 
                                    FROM ""document"".""DocumentSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false
                                    AND sp1.""Status"" = ANY (@PostStatus)
                                    GROUP BY sp1.""Id"",sp1.""PostId"",sp1.""CreatedOn""
                                    ORDER BY sp1.""CreatedOn"" DESC
                                    LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
                                LEFT JOIN ""document"".""DocumentTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
                                LEFT JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 

                                WHERE (@TagName IS NULL OR qtag.""Name"" = @TagName) AND qpost1.""Type"" = @PostType
                                AND qpost1.""Status"" = ANY (@PostStatus)
                                AND qpost1.""Permission"" = @PostPermission
                                AND qpost1.""IsDelete"" = false
                                AND NOT (qpost1.""Hide"" = ANY (@Hide) AND qpost1.""Hide"" = ANY (@Hide) IS NOT NULL)
                                GROUP BY qpost1.""Id"", psp1.""CreatedOn"", qpost1.""Hide""
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
                                 FROM ""document"".""DocumentPosts"" qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId"", sp1.""CreatedOn"" 
                                    FROM ""document"".""DocumentSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false 
                                    AND sp1.""Status"" = ANY (@PostStatus)
                                    GROUP BY sp1.""Id"",sp1.""PostId"",sp1.""CreatedOn""
                                    ORDER BY sp1.""CreatedOn"" DESC
                                    LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
                                LEFT JOIN ""document"".""DocumentTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
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
                                 FROM ""document"".""DocumentPosts"" qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId""
                                    FROM ""document"".""DocumentSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  
                                    AND sp1.""Status"" = ANY (@PostStatus)
                                    GROUP BY sp1.""Id"", sp1.""PostId""
                                    -- LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
                                LEFT JOIN ""document"".""DocumentTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
                                LEFT JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
                                WHERE (@TagName IS NULL OR qtag.""Name"" = @TagName) AND qpost1.""Type"" = @PostType
                                AND NOT (qpost1.""Hide"" = ANY (@Hide) AND qpost1.""Hide"" = ANY (@Hide) IS NOT NULL) 
                                AND qpost1.""Status"" = ANY (@PostStatus)
                                AND qpost1.""IsDelete"" = false
                                AND qpost1.""Permission"" = @PostPermission
                                GROUP BY qpost1.""Id""";
            }
        }
        private string GetTopLatestPostByMultiTagToCountQuery
        {
            get
            {
                return @"SELECT qpost1.""Id""
                                 FROM ""document"".""DocumentPosts"" qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId""
                                    FROM ""document"".""DocumentSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false 
                                    AND sp1.""Status"" = ANY (@PostStatus)
                                    GROUP BY sp1.""Id"", sp1.""PostId""
                                    -- LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
                                LEFT JOIN ""document"".""DocumentTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
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
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, psp1.""CreatedOn"", 1 AS ""SelectType"", qpost1.""Hide""
                                 FROM ""document"".""DocumentPosts"" qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId"", sp1.""CreatedOn"" 
                                    FROM ""document"".""DocumentSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false 
                                    AND sp1.""Status"" = ANY (@PostStatus)
                                    GROUP BY sp1.""Id"",sp1.""PostId"",sp1.""CreatedOn""
                                    ORDER BY sp1.""CreatedOn"" DESC
                                    LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 

                                INNER JOIN ""document"".""DocumentTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
INNER JOIN ""TagFavorites"" tagfa ON qtp.""TagId"" = tagfa.""TagId""
                                INNER JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 

                                WHERE tagfa.""UserId"" = @UserId AND qpost1.""Type"" = @PostType 
                                AND qpost1.""Status"" = ANY (@PostStatus)
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
                                 FROM ""document"".""DocumentPosts"" qpost1
                                 INNER JOIN LATERAL (
                                --Lastest subpost                                     
                                    SELECT sp1.""Id"", sp1.""PostId""
                                    FROM ""document"".""DocumentSubPosts"" sp1 
                                    WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  
                                    AND sp1.""Status"" = ANY (@PostStatus)
                                    GROUP BY sp1.""Id"", sp1.""PostId""
                                    -- LIMIT 1
                                ) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
                                LEFT JOIN ""document"".""DocumentTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
INNER JOIN ""TagFavorites"" tagfa ON qtp.""TagId"" = tagfa.""TagId""
                                LEFT JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
                                WHERE tagfa.""UserId"" = @UserId AND qpost1.""Type"" = @PostType 
                                AND qpost1.""Status"" = ANY (@PostStatus)
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
                                 FROM ""document"".""DocumentPosts"" qpost1

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
                                 FROM ""document"".""DocumentPosts"" qpost2
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
                                FROM ""document"".""DocumentPosts"" qpost1                                 
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
                                 FROM ""document"".""DocumentPosts"" qpost1        
                                 INNER JOIN ""document"".""DocumentTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id"" AND qtp.""PostId"" = false
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
                                 FROM ""document"".""DocumentPosts"" qpost1
                                 INNER JOIN ""document"".""DocumentTagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id"" AND qtp.""IsDelete"" = false
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
                                FROM ""document"".""DocumentPosts"" qpost
                                LEFT JOIN ""document"".""DocumentPostFavorites"" cfp on qpost.""Id"" = cfp.""PostId""
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
                                FROM ""document"".""DocumentPosts"" qpost 
                                LEFT JOIN ""document"".""DocumentPostFavorites"" cfp on qpost.""Id"" = cfp.""PostId""
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
                                FROM ""document"".""DocumentPosts"" qpost
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
                                FROM ""document"".""DocumentPosts"" qpost 
                                WHERE qpost.""CreatedBy"" = @UserId AND qpost.""Type"" = @PostType AND qpost.""IsDelete"" = false";
            }
        }
        private string GetMyAllQuery
        {
            get
            {
                return $@" SELECT ""HashId"", ""Title"", ""Type"", ""Status""
                                FROM {_postRepository.TableName}
                                WHERE ""CreatedBy"" = @UserId AND ""IsDelete"" = false AND ""Type"" IN ({(int)PostType.Story}, {(int)PostType.Document}) 
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
                                 FROM ""document"".""DocumentPosts"" qpost1        
                                 INNER JOIN identity.""Users"" user1 ON user1.""Id"" = qpost1.""UserId"" 
                                WHERE  user1.""UserName"" = @ProfileName AND qpost1.""Type"" = @PostType 
                                AND qpost1.""Status"" = ANY (@PostStatus)
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
                                 FROM ""document"".""DocumentPosts"" qpost1
                                 INNER JOIN identity.""Users"" user1 ON user1.""Id"" = qpost1.""UserId"" 
                                WHERE  user1.""UserName"" = @ProfileName AND qpost1.""Type"" = @PostType 
                                AND qpost1.""Status"" = ANY (@PostStatus)
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
                return @"SELECT sp.""Id"",sp.""HashId"",sp.""Name"", sp.""Title"", sp.""PostId"", sp.""Order"", sp.""Body"", sp.""IsPremium"", sp.""IsExclusive"", ux.""Id"" as ""UserExclusiveId"",
                    sp.""Status"", sp.""CreatedOn"", sp.""CreatedBy"", sp.""ModifiedOn"", sp.""IsAllowDownload"",
                    sp.""ModifiedBy"", sp.""IsDelete"", count.""ViewCount"", sp.""AuthorId"", u.""ProfileName"",
                    sp.""UserId"", sp.""PublishDate"", sp.""Permission"",sp.""CreatorNote"",
                    sp.""IsEnableComment"", spcmc.""CommentCount"",
                    rs.""Id"", rs.""AuthorId"", rs.""Title"", rs.""Name"", rs.""Url"", rs.""Type"", rs.""CreatedOn"", 
                    rs.""CreatedBy"", rs.""ModifiedOn"", rs.""ModifiedBy"", rs.""IsDelete"", rs.""HashId"", rs.""SubPostId"", 
                    rs.""Status"", rs.""Size"", rs.""LocationType"", rs.""Height"", rs.""Width"", rs.""Order"", rs.""MinioInstance"", rs.""BucketName""
                    FROM ""document"".""DocumentSubPosts"" sp
                    INNER JOIN ""document"".""DocumentPosts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
LEFT JOIN ""UserExclusiveSubPosts"" ux ON ux.""SubPostId"" = sp.""Id"" AND ux.""UserId"" = @UserId
INNER JOIN identity.""Users"" u ON u.""Id"" = sp.""CreatedBy""
                    LEFT JOIN ""document"".""DocumentResources"" rs ON sp.""Id"" = rs.""SubPostId"" AND rs.""IsDelete"" = false
LEFT JOIN LATERAL 
                            (
                                SELECT COUNT(spcm.""Id"") as ""CommentCount"", spcm.""PostId"" as ""SubPostId""
FROM ""document"".""DocumentSubPostComments"" spcm 
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
                            AND sp.""Status"" = ANY (@PostStatus)
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
                    FROM ""document"".""DocumentSubPosts"" sp
LEFT JOIN ""UserExclusiveSubPosts"" ux ON ux.""SubPostId"" = sp.""Id"" AND ux.""UserId"" = @UserId
                    INNER JOIN ""document"".""DocumentPosts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
LEFT JOIN LATERAL 
                            (
                                SELECT COUNT(spcm.""Id"") as ""CommentCount"", spcm.""PostId"" as ""SubPostId""
FROM ""document"".""DocumentSubPostComments"" spcm 
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
                                FROM ""document"".""DocumentSubPosts"" sp
                                INNER JOIN ""document"".""DocumentPosts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
                                WHERE p.""HashId"" = @PostHashId AND sp.""IsDelete"" = false
                            ) p;
";
            }
        }

        private string GetSeriesChaptersSimpleByHashId
        {
            get
            {
                return @"SELECT sp.""Id"", sp.""Title"", sp.""Order"", sp.""IsPremium""
                    FROM ""document"".""DocumentSubPosts"" sp
                    INNER JOIN ""document"".""DocumentPosts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
                    WHERE p.""HashId"" = @PostHashId AND sp.""IsDelete"" = false AND (sp.""Status"" = ANY (@PostStatus) OR sp.""UserId"" = @UserId)
                    AND sp.""IsPremium"" = false AND sp.""PublishDate"" < @CurrentDate
                    ORDER BY sp.""Sort"";

                        SELECT COUNT(*) AS TotalItems 
                        FROM (
                                SELECT sp.""Id"" 
                                FROM ""document"".""DocumentSubPosts"" sp
                                INNER JOIN ""document"".""DocumentPosts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
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
                return @"UPDATE ""document"".""DocumentPosts"" 
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""Id"" = @PostId;

                    UPDATE ""document"".""DocumentResources""
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    FROM (SELECT ""Id""
                          FROM ""document"".""DocumentSubPosts"" WHERE ""PostId"" = @PostId) AS sp
                    WHERE ""document"".""DocumentResources"".""SubPostId"" = sp.""Id"";
    
                    UPDATE ""document"".""DocumentSubPostReactions""spr
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    FROM (SELECT ""Id""
                          FROM ""document"".""DocumentSubPosts"" WHERE ""PostId"" = @PostId) AS sp
                    WHERE spr.""TargetId"" = sp.""Id"";
    
                    UPDATE ""document"".""DocumentSubPostComments"" spr
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    FROM (SELECT ""Id""
                          FROM ""document"".""DocumentSubPosts"" WHERE ""PostId"" = @PostId) AS sp
                    WHERE spr.""PostId"" = sp.""Id"";
    
                    UPDATE ""document"".""DocumentSubPostCommentReactions"" spr
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    FROM (SELECT ""Id""
                          FROM ""document"".""DocumentSubPosts"" WHERE ""PostId"" = @PostId) AS sp
                    WHERE spr.""TargetId"" = sp.""Id"";
    
                    UPDATE ""document"".""DocumentTagPosts""
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""PostId"" = @PostId;
    
                    UPDATE ""document"".""DocumentSubPosts""
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""PostId"" = @PostId;
    
                    UPDATE ""document"".""DocumentPostFavorites""
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""PostId"" = @PostId;
    
                    UPDATE ""document"".""DocumentPostReactions""
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""TargetId"" = @PostId;
    
                    UPDATE ""document"".""DocumentPostComments""
                    SET ""IsDelete"" = true    , ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""PostId"" = @PostId;";
            }
        }

        #region Delete Subpost

        private string ExecSoftDeleteSubPost
        {
            get
            {
                return @"UPDATE ""document"".""DocumentSubPosts""
                    SET ""Order"" = 0, ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""Id"" = @SubPostId;

                    UPDATE ""document"".""DocumentResources""
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""SubPostId"" = @SubPostId;
    
                    UPDATE ""document"".""DocumentSubPostReactions""
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""TargetId"" = @SubPostId;
    
                    UPDATE ""document"".""DocumentSubPostComments""
                    SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
                    WHERE ""PostId"" = @SubPostId;
    
                    UPDATE ""document"".""DocumentSubPostCommentReactions"" 
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
                FROM ""document"".""DocumentPosts"" p
                LEFT JOIN ""document"".""DocumentTagPosts"" tp ON p.""Id"" = tp.""PostId""
                LEFT JOIN ""Tags"" t ON tp.""TagId"" = t.""Id""
                LEFT JOIN identity.""Users"" u on u.""Id"" = p.""UserId""
                LEFT JOIN (
                    SELECT ""PostId"",
                           ""Title"",
                           ""Order"",
                           ""CreatedOn"",
                           ""PublishDate"",
                           ROW_NUMBER() OVER (PARTITION BY ""PostId"" ORDER BY ""Order"" desc) AS rn
                    FROM ""document"".""DocumentSubPosts""
                    WHERE ""IsDelete"" = false
                ) sp ON p.""Id"" = sp.""PostId""
                LEFT JOIN (
                    SELECT ""PostId"", MAX(""PublishDate"") AS ""LatestSubPostPublishDate""
                    FROM ""document"".""DocumentSubPosts""
                    WHERE ""IsDelete"" = false AND ""PublishDate"" < @CurrentDate
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
