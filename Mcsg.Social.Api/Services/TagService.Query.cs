namespace Mcsg.Social.Api.Services
{
    public partial class TagService
    {
        private string GetSuggestTagsByNameQuery
        {
            get
            {
                return @"SELECT 
                        t.""Name"",COUNT( t.""Id"") as Count
                        FROM ""Tags"" t
                        LEFT JOIN social.""SocialTagPosts"" tp ON tp.""TagId"" = t.""Id"" 
                        WHERE t.""Name"" ILIKE @TagSearch
                        AND t.""IsDelete"" = false 
                        AND tp.""IsDelete"" = false 
                        GROUP BY 
                        t.""Name"", t.""Id""
                        ORDER BY Count DESC
                        LIMIT @PageSize
                        OFFSET 0;";
            }
        }

        private string GetPopularTagsQuery
        {
            get
            {
                return @$"SELECT tag.""Id"", tag.""Name"", COUNT(post.""Id"") AS count
                                FROM {_postRepository.TableName} post
                                INNER JOIN {_tagPostRepository.TableName} tagpost ON post.""Id"" = tagpost.""PostId"" 
                                INNER JOIN {_tagRepository.TableName} tag ON tagpost.""TagId"" = tag.""Id"" 
                                WHERE [AddPostType] post.""IsDelete"" = false AND tagpost.""IsDelete"" = false 
                                GROUP BY  tag.""Id"", tag.""Name"" 
                                ORDER BY ""count"" DESC
                                LIMIT @PageSize
                                OFFSET @Offet;

                        SELECT COUNT(*) AS TotalItems
                            FROM (
                            select distinct  tag.""Id""
                                    FROM {_postRepository.TableName} post
                                    INNER JOIN {_tagPostRepository.TableName} tagpost ON post.""Id"" = tagpost.""PostId"" 
                                    INNER JOIN {_tagRepository.TableName} tag ON tagpost.""TagId"" = tag.""Id"" 
                                    WHERE [AddPostType] post.""IsDelete"" = false AND tagpost.""IsDelete"" = false
                            ) q";
            }
        }

        private string GetTodayTrendingTagsQuery
        {
            get
            {
                return @$" SELECT tag.""Id"", tag.""Name""
                                , MAX(today.""Count"") AS ""Count""
                                , COUNT(tagpost.""PostId"") AS ""TotalCount""
                                FROM {_tagRepository.TableName} tag
                                LEFT JOIN {_tagPostRepository.TableName} tagpost 
                                    ON tagpost.""TagId"" = tag.""Id"" AND tagpost.""IsDelete"" = false
                                INNER JOIN
                                (
                                    SELECT tagpost.""TagId"", COUNT(tagpost.""PostId"") AS ""Count"" 
                                    FROM {_postRepository.TableName} post
                                    INNER JOIN {_tagPostRepository.TableName} tagpost ON post.""Id"" = tagpost.""PostId"" 
                                    WHERE tagpost.""ModifiedOn"" BETWEEN @FromDate AND @ToDate [AddPostType] 
                                    AND post.""IsDelete"" = false AND tagpost.""IsDelete"" = false
                                    GROUP BY tagpost.""TagId""
                                ) today ON tag.""Id"" = today.""TagId""
                                GROUP BY tag.""Id"", tag.""Name""
                                ORDER BY ""Count"" DESC, ""TotalCount"" DESC 
                                LIMIT @PageSize
                                OFFSET @Offet;

                        SELECT COUNT(*) AS TotalItems
                            FROM (
                            select distinct  tag.""Id""
                                    FROM {_postRepository.TableName} post
                                    INNER JOIN {_tagPostRepository.TableName} tagpost ON post.""Id"" = tagpost.""PostId"" 
                                    INNER JOIN {_tagRepository.TableName} tag ON tagpost.""TagId"" = tag.""Id"" 
                                    WHERE post.""ModifiedOn"" BETWEEN @FromDate AND @ToDate [AddPostType]
                                        AND  post.""IsDelete"" = false AND tagpost.""IsDelete"" = false 
                            ) q";
            }
        }

        private string SearchTagsByNameQuery
        {
            get
            {
                return @"   SELECT  t.""Name"", t.""Id"" , COUNT( t.""Id"") 
                            FROM {0} t
                            LEFT JOIN social.""SocialTagPosts"" tp  
                            ON tp.""TagId""  = t.""Id"" 
                            LEFT JOIN social.""SocialPosts"" p 
                            ON p.""Id""  = tp.""PostId"" 
                            WHERE t.""IsDelete"" = false 
                            AND tp.""IsDelete"" = false 
                            AND (""Name"" LIKE @ExactKeyword  
                                OR ""Name"" LIKE @StartsWithKeyword 
                                OR ""Name"" LIKE @ContainsKeyword )
                            GROUP BY t.""Name"", t.""Id""
                            ORDER BY CASE 
                                WHEN ""Name"" LIKE @ExactKeyword THEN 0 
                                WHEN ""Name"" LIKE @StartsWithKeyword THEN 1 
                                ELSE 2 
                            END 
                            LIMIT 4";
            }
        }

        private string SearchTagsRandomQuery
        {
            get
            {
                return @"   SELECT t.""Name"", t.""Id"" , COUNT( t.""Id"") 
                            FROM {0} t
                            LEFT JOIN social.""SocialTagPosts"" tp  
                            ON tp.""TagId""  = t.""Id"" 
                            LEFT JOIN social.""SocialPosts"" p 
                            ON p.""Id""  = tp.""PostId"" 
                            WHERE t.""IsDelete"" = false 
                            AND tp.""IsDelete"" = false 
                            GROUP BY t.""Name"", t.""Id""
                            ORDER BY RANDOM()
                            LIMIT 4";
            }
        }

        private string SearchTagWithPostCount
        {
            get
            {
                return @"SELECT t.""Name"",
                                COALESCE(tp.post_count, 0) + COALESCE(tc.comic_count, 0) + COALESCE(stp.story_count, 0) + COALESCE(dcp.document_count, 0) AS Count
                        FROM ""Tags"" t
                        LEFT JOIN (
                            SELECT  tp.""TagId"", COUNT(*) AS post_count
                            FROM social.""SocialTagPosts"" tp
                            JOIN social.""SocialPosts"" p ON tp.""PostId"" = p.""Id""
                            AND p.""Status"" = ANY (@PostStatus)
                            WHERE tp.""IsDelete"" = false AND p.""IsDelete"" = false
                            GROUP BY tp.""TagId""
                        ) tp ON t.""Id"" = tp.""TagId""
                        LEFT JOIN (
                        SELECT ""TagId"", COUNT(*) AS comic_count
                        FROM comic.""ComicTagPosts"" ctp
                        JOIN comic.""ComicPosts"" cp ON ctp.""PostId"" = cp.""Id""
                        AND cp.""Status"" = ANY (@PostStatus)
                        WHERE ctp.""IsDelete"" = false AND cp.""IsDelete"" = false AND cp.""Permission"" != 1
                        GROUP BY  ""TagId""
                        ) tc ON t.""Id"" = tc.""TagId""
                        LEFT JOIN (
                        SELECT stp.""TagId"", COUNT(*) AS story_count
                        FROM story.""StoryTagPosts"" stp
                        JOIN story.""StoryPosts"" sp ON stp.""PostId"" = sp.""Id""
                        AND sp.""Status"" = ANY (@PostStatus)
                        WHERE stp.""IsDelete"" = false AND sp.""IsDelete"" = false AND sp.""Permission"" != 1
                        GROUP BY stp.""TagId""
                        ) stp ON t.""Id"" = stp.""TagId""
                        LEFT JOIN (
                        SELECT dcp.""TagId"", COUNT(*) AS document_count
                        FROM Document.""DocumentTagPosts"" dcp
                        JOIN Document.""DocumentPosts"" sp ON dcp.""PostId"" = sp.""Id""
                        AND sp.""Status"" = ANY (@PostStatus)
                        WHERE dcp.""IsDelete"" = false AND sp.""IsDelete"" = false AND sp.""Permission"" != 1
                        GROUP BY dcp.""TagId""
                        ) dcp ON t.""Id"" = dcp.""TagId""
                        [QueryCondition]
                        OFFSET @Offset
                        LIMIT @PageSize;
                        
                        SELECT COUNT(*)
                        FROM ""Tags"" t
                        LEFT JOIN (
                            SELECT  tp.""TagId""
                            FROM social.""SocialTagPosts"" tp
                            JOIN social.""SocialPosts"" p ON tp.""PostId"" = p.""Id""
                            AND p.""Status"" = ANY (@PostStatus)
                            WHERE tp.""IsDelete"" = false AND p.""IsDelete"" = false
                            GROUP BY tp.""TagId""
                        ) tp ON t.""Id"" = tp.""TagId""
                        LEFT JOIN (
                        SELECT ""TagId""
                        FROM comic.""ComicTagPosts"" ctp
                        JOIN comic.""ComicPosts"" cp ON ctp.""PostId"" = cp.""Id""
                        AND cp.""Status"" = ANY (@PostStatus)
                        WHERE ctp.""IsDelete"" = false AND cp.""IsDelete"" = false AND cp.""Permission"" != 1
                        GROUP BY  ""TagId""
                        ) tc ON t.""Id"" = tc.""TagId""
                        LEFT JOIN (
                        SELECT stp.""TagId""
                        FROM story.""StoryTagPosts"" stp
                        JOIN story.""StoryPosts"" sp ON stp.""PostId"" = sp.""Id""
                        AND sp.""Status"" = ANY (@PostStatus)
                        WHERE stp.""IsDelete"" = false AND sp.""IsDelete"" = false AND sp.""Permission"" != 1
                        GROUP BY stp.""TagId""
                        ) stp ON t.""Id"" = stp.""TagId""
                        LEFT JOIN (
                        SELECT dcp.""TagId""
                        FROM document.""DocumentTagPosts"" dcp
                        JOIN document.""DocumentPosts"" sp ON dcp.""PostId"" = sp.""Id""
                        AND sp.""Status"" = ANY (@PostStatus)
                        WHERE dcp.""IsDelete"" = false AND sp.""IsDelete"" = false AND sp.""Permission"" != 1
                        GROUP BY dcp.""TagId""
                        ) dcp ON t.""Id"" = dcp.""TagId""
                        [QueryCondition]
                        ";
            }
        }
    }
}
