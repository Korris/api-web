namespace Mcsg.Story.Api.Services
{
    public partial class TagService
    {
        private string GetSuggestTagsByNameQuery
        {
            get
            {
                return @"SELECT 
                        t.""Name"",COUNT( tp.""Id"") as Count
                        FROM ""Tags"" t
                        LEFT JOIN story.""StoryTagPosts"" tp ON tp.""TagId"" = t.""Id"" 
                        AND tp.""IsDelete"" = false 
                        WHERE t.""Name"" ILIKE @TagSearch
                        AND t.""IsDelete"" = false 
                        GROUP BY 
                        t.""Name""
                        ORDER BY Count DESC
                        LIMIT @PageSize
                        OFFSET 0;";
            }
        }

        private string GetPopularTagsQuery
        {
            get
            {
                return @$"SELECT tag.""Id"", tag.""Name"", COUNT(DISTINCT post.""Id"") AS count,  MAX(post.""CreatedOn"") AS latest_created_on
                          FROM {_postRepository.TableName} post
                          INNER JOIN story.""StorySubPosts"" subpost ON post.""Id"" = subpost.""PostId""
                          INNER JOIN {_tagPostRepository.TableName} tagpost ON post.""Id"" = tagpost.""PostId"" 
                          INNER JOIN {_tagRepository.TableName} tag ON tagpost.""TagId"" = tag.""Id"" 
                          WHERE [AddPostType] post.""IsDelete"" = false 
                          AND NOT (post.""Hide"" = ANY (@Hide) AND post.""Hide"" = ANY (@Hide) IS NOT NULL) 
                          AND subpost.""IsDelete"" = false 
                          AND tagpost.""IsDelete"" = false 
                          AND post.""Permission"" = (@Permission)
                          AND post.""Status"" = ANY (@PostStatus)
                          GROUP BY  tag.""Id"", tag.""Name""
                          ORDER BY ""count"" DESC,  latest_created_on DESC
                          LIMIT @PageSize
                          OFFSET @Offet;

                  SELECT COUNT(*) AS TotalItems
                      FROM (
                      select distinct  tag.""Id""
                              FROM {_postRepository.TableName} post
                              INNER JOIN story.""StorySubPosts"" subpost ON post.""Id"" = subpost.""PostId""
                              INNER JOIN {_tagPostRepository.TableName} tagpost ON post.""Id"" = tagpost.""PostId"" 
                              INNER JOIN {_tagRepository.TableName} tag ON tagpost.""TagId"" = tag.""Id"" 
                              WHERE [AddPostType] post.""IsDelete"" = false 
                              AND NOT (post.""Hide"" = ANY (@Hide) AND post.""Hide"" = ANY (@Hide) IS NOT NULL) 
                              AND post.""Status"" = ANY (@PostStatus)                              
                              AND subpost.""IsDelete"" = false 
                              AND tagpost.""IsDelete"" = false 
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
                return @"   SELECT  t.""Name"", t.""Id"" , COUNT( tp.""Id"") 
                            FROM {0} t
                            LEFT JOIN story.""StoryTagPosts"" tp  
                            ON tp.""TagId""  = t.""Id"" 
                            AND tp.""IsDelete"" = false 
                            LEFT JOIN ""story"".""StoryPosts""  p 
                            ON p.""Id""  = tp.""PostId"" 
                            WHERE t.""IsDelete"" = false 
                            AND ""Name"" LIKE @ExactKeyword  
                                OR ""Name"" LIKE @StartsWithKeyword 
                                OR ""Name"" LIKE @ContainsKeyword 
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
                return @"   SELECT t.""Name"", t.""Id"" , COUNT( tp.""Id"") 
                            FROM {0} t
                            LEFT JOIN story.""StoryTagPosts"" tp  
                            ON tp.""TagId""  = t.""Id"" 
                            AND tp.""IsDelete"" = false 
                            LEFT JOIN ""story"".""StoryPosts""  p 
                            ON p.""Id""  = tp.""PostId"" 
                            WHERE t.""IsDelete"" = false 
                            GROUP BY t.""Name"", t.""Id""
                            ORDER BY RANDOM()
                            LIMIT 4";
            }
        }

    }
}
