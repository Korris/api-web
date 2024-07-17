namespace Mcsg.Story.Api.Services
{
    public partial class TagService
    {
        private string GetAllTagsByPostHashIdQuery
        {
            get
            {
                return @$"SELECT post.""HashId"" AS ""PostHashId"", tp.""PostId"", tag.""Id"", tag.""Title"", tag.""Name""
						FROM {_postRepository.TableName} post
						INNER JOIN {_tagPostRepository.TableName} tp ON tp.""PostId"" = post.""Id""
                        INNER JOIN {_tagRepository.TableName} tag ON tp.""TagId"" = tag.""Id"" 
						WHERE post.""HashId"" = @PostHashId AND post.""IsDelete"" = false; ";
            }
        }

        private string GetSuggestTagsByNameQuery
        {
            get
            {
                return @"SELECT 
						t.""Name"",COUNT( t.""Id"") as Count
						FROM ""Tags"" t
						LEFT JOIN ""TagPosts"" tp ON tp.""TagId"" = t.""Id"" 
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
	                                WHERE tagpost.""LastModifiedDate"" BETWEEN @FromDate AND @ToDate [AddPostType] 
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
                                    WHERE post.""LastModifiedDate"" BETWEEN @FromDate AND @ToDate [AddPostType]
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
                            LEFT JOIN ""TagPosts"" tp  
                            ON tp.""TagId""  = t.""Id"" 
                            LEFT JOIN ""Posts"" p 
                            ON p.""Id""  = tp.""PostId"" 
                            WHERE t.""IsDelete"" = false 
                            AND tp.""IsDelete"" = false 
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
                return @"   SELECT t.""Name"", t.""Id"" , COUNT( t.""Id"") 
                            FROM {0} t
                            LEFT JOIN ""TagPosts"" tp  
                            ON tp.""TagId""  = t.""Id"" 
                            LEFT JOIN ""Posts"" p 
                            ON p.""Id""  = tp.""PostId"" 
                            WHERE t.""IsDelete"" = false 
                            AND tp.""IsDelete"" = false 
                            GROUP BY t.""Name"", t.""Id""
                            ORDER BY RANDOM()
                            LIMIT 4";
            }
        }

    }
}
