using Mcsg.Lib.Model.Enums;

namespace Mcsg.Social.Api.Services
{
    public partial class PostService
    {
        private string GetTotalCommentQuery => $@"SELECT 
														(SELECT COUNT(*)
														 FROM ""PostComments""  pc
														 JOIN ""Posts"" p ON pc.""PostId""= p.""Id""
														 WHERE p.""HashId"" = @HashId) 
														+
														(SELECT COUNT(*)
														 FROM ""SubPostComments"" spc
														 JOIN ""SubPosts"" sp ON spc.""PostId""= sp.""Id""
														 JOIN ""Posts"" p ON sp.""PostId""= p.""Id""
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
						u.""ProfileId"",
						u.""Avatar"" as ""UserAvatar"",
						p.""Status"", p.""Type"", 
						array_agg(tag.""Name"") as Tags,
						p.""CreatedDate"",
						sp.""Id"", 
						sp.""HashId"",sp.""IsExclusive"",
						sp.""Title"",
						sp.""Order"",
						sp.""Status"",
						sp.""Permission"",
						COUNT(DISTINCT spcm.""Id"") as ""CommentCount"",
						subpostview.""ViewCount"",
						sp.""CreatedDate"",
						sp.""PublishDate"",
						ux.""Id"" as ""UserExclusiveId"" ,
						sp.""CreatorNote"",
						sp.""IsEnableComment""
						FROM ""Posts"" p
						LEFT JOIN identity.""Users"" u ON p.""UserId"" = u.""Id""
						LEFT JOIN ""TagPosts"" tp ON tp.""PostId"" = p.""Id""
						LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id""
						LEFT JOIN ""SubPosts"" sp ON sp.""PostId"" = p.""Id"" AND sp.""IsDelete"" = false [WithPermission]  [Not-load-chapter]	
						LEFT JOIN ""UserExclusiveSubPosts"" ux ON ux.""SubPostId"" = sp.""Id"" AND ux.""UserId"" = @UserId
						LEFT JOIN ""SubPostComments"" spcm ON spcm.""PostId"" = sp.""Id"" AND spcm.""IsDelete"" = false
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
						p.""AuthorId"",p.""AuthorName"",u.""ProfileName"", u.""ProfileId"",u.""Avatar"", p.""CreatedDate"",
						p.""Status"", p.""Type"", p.""CreatedDate"",sp.""Id"",sp.""HashId"",sp.""Title"",sp.""Order"", sp.""Status"",ux.""Id"",
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
	                SELECT ""Id"", ""Title"", ""HashId"", ""Type"", ""Body"", ""Status"", ""CreatedDate"", ""CreatedBy"", ""LastModifiedDate"", ""LastModifiedBy"", ""IsDelete"", ""Permission"", ""ThumbnailUrl"", ""AuthorName"", ""CoverUrl"", ""IsMature"",""IsCompleted"", ""ViewCount"", ""AuthorId"", ""UserId""
	                FROM public.""Posts""
	                WHERE ""HashId"" = @HashId;";
            }
        }

        private string GetPostAndLastSubPostOrder

        {
            get
            {
                return @"
	                SELECT ""Id"", ""Title"", ""HashId"", ""Type"", ""Body"", ""Status"", ""CreatedDate"", ""CreatedBy"", ""LastModifiedDate"", ""LastModifiedBy"", ""IsDelete"", ""Permission"", ""ThumbnailUrl"", ""AuthorName"", ""CoverUrl"", ""IsMature"", ""IsCompleted"", ""ViewCount"", ""AuthorId"", ""UserId""
	                FROM public.""Posts""
	                WHERE ""HashId"" = @HashId;

	                SELECT MAX(""Order"")
		            FROM ""SubPosts"" sp
		            INNER JOIN ""Posts"" p
		            ON sp.""PostId"" = p.""Id""
		            WHERE p.""HashId"" = @HashId AND p.""IsDelete"" = false AND sp.""IsDelete"" = false
";
            }
        }
        private string GetTopAllPostAllTypeByTagQuery
        {
            get
            {
                return @"SELECT post.""SelectType"",post.""Id"",post.""Title"", post.""Body"", post.""HashId"",
						post.""UserId"", post.""ProfileName"",post.""ProfileId"",post.""Avatar"" as ""UserAvatar"", post.""ThumbnailUrl"", 
						post.""ChapterCount"",
						post.""Status"", post.""Type"",post.""ViewCount"",post.""TotalSubPostComment"",
						post.""CreatedDate"",post.""AuthorName"", post.""CoverUrl"", post.""IsMature"", post.""IsCompleted"", post.""Permission"", post.""AuthorId"",
						post.""SubPostStr"", 
						array_agg(tag.""Name"") as Tags from
							(SELECT  p.""Id"",
							p.""Title"", p.""Body"",  
							p.""HashId"",p.""UserId"", sp.""Total"" AS ""ChapterCount"",
							u.""ProfileName"",u.""ProfileId"",u.""Avatar"", p.""ThumbnailUrl"", 
							p.""AuthorName"", p.""CoverUrl"", p.""IsMature"", p.""IsCompleted"", p.""Permission"",p.""AuthorId"",
							 postid.""SelectType"",
							p.""Status"", p.""Type"", postview.""ViewCount"",
							p.""CreatedDate"",--sp.""Id"" as ""SPID"",
SUM(""CommentCount"") as ""TotalSubPostComment"",
							--sp.""ChapterCount"" AS ""ChapterCount"",
							to_jsonb(array_agg(sp.*)) AS ""SubPostStr""	
							 
							FROM ""Posts"" p
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
							p.""CreatedDate""
							) 
						AS post
						LEFT JOIN ""TagPosts"" tp ON tp.""PostId"" = post.""Id""
						LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id"" 
						GROUP BY post.""SelectType"", post.""Id"",post.""Title"", post.""Body"", post.""HashId"", 
						post.""AuthorName"", post.""CoverUrl"", post.""IsMature"",post.""IsCompleted"", post.""Permission"",post.""AuthorId"",
						post.""UserId"",post.""ProfileName"",post.""ProfileId"",post.""Avatar"", post.""ThumbnailUrl"", post.""ChapterCount"",post.""TotalSubPostComment"",
						post.""Status"", post.""Type"", post.""ViewCount"",
						post.""CreatedDate"",
						post.""SubPostStr""
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
								SELECT sp.""Id"",sp.""HashId"",sp.""PostId"",sp.""CreatedDate"",sp.""Title"",sp.""Order"", sp.""IsExclusive"",
COUNT(spcm.""Id"") as ""CommentCount"",subpostview.""ViewCount"",
count(*) OVER() AS ""Total"" 
								FROM ""SubPosts"" sp 
--Comment count
LEFT JOIN ""SubPostComments"" spcm ON spcm.""PostId"" = sp.""Id"" AND spcm.""IsDelete"" = false
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
								SELECT DISTINCT qpost.""Id"", COUNT(pcm.""Id"") as COUNTCM, qpost.""CreatedDate"", 0 AS ""SelectType""

								FROM ""Posts"" qpost
							 	INNER JOIN ""PostComments"" pcm ON pcm.""PostId"" = qpost.""Id"" 
								AND pcm.""CreatedDate"" > @LastWeek
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
								FROM ""Posts"" qpost
							 	INNER JOIN ""PostComments"" pcm ON pcm.""PostId"" = qpost.""Id"" 
								AND pcm.""CreatedDate"" > @LastWeek
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
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, psp1.""CreatedDate"", 1 AS ""SelectType""
								 FROM ""Posts"" qpost1
							 	INNER JOIN LATERAL (
								--Lastest subpost									 
									SELECT sp1.""Id"", sp1.""PostId"", sp1.""CreatedDate"" 
									FROM ""SubPosts"" sp1 
									WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  AND sp1.""Status"" = @PostStatus
									GROUP BY sp1.""Id"",sp1.""PostId"",sp1.""CreatedDate""
									ORDER BY sp1.""CreatedDate"" DESC
									LIMIT 1
								) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
								WHERE  qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
								AND qpost1.""IsDelete"" = false 								
								GROUP BY qpost1.""Id"", psp1.""CreatedDate""
								ORDER BY psp1.""CreatedDate"" DESC
								LIMIT @PageSize
								OFFSET @Offet";
            }
        }
        private string GetTopLatestPostToCountQuery
        {
            get
            {
                return @"SELECT qpost1.""Id""
								 FROM ""Posts"" qpost1
							 	INNER JOIN LATERAL (
								--Lastest subpost									 
									SELECT sp1.""Id"", sp1.""PostId""
									FROM ""SubPosts"" sp1 
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
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, psp1.""CreatedDate"", 1 AS ""SelectType""
								 FROM ""Posts"" qpost1
							 	INNER JOIN LATERAL (
								--Lastest subpost									 
									SELECT sp1.""Id"", sp1.""PostId"", sp1.""CreatedDate"" 
									FROM ""SubPosts"" sp1 
									WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  AND sp1.""Status"" = @PostStatus
									GROUP BY sp1.""Id"",sp1.""PostId"",sp1.""CreatedDate""
									ORDER BY sp1.""CreatedDate"" DESC
									LIMIT 1
								) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
								LEFT JOIN ""TagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
								LEFT JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 

								WHERE (@TagName IS NULL OR qtag.""Name"" = @TagName) AND qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
								AND qpost1.""IsDelete"" = false 								
								GROUP BY qpost1.""Id"", psp1.""CreatedDate""
								ORDER BY psp1.""CreatedDate"" DESC
								LIMIT @PageSize
								OFFSET @Offet";
            }
        }

        private string GetTopLatestPostByMultiTagQuery
        {
            get
            {
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, psp1.""CreatedDate"", 1 AS ""SelectType""
								 FROM ""Posts"" qpost1
							 	INNER JOIN LATERAL (
								--Lastest subpost									 
									SELECT sp1.""Id"", sp1.""PostId"", sp1.""CreatedDate"" 
									FROM ""SubPosts"" sp1 
									WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  AND sp1.""Status"" = @PostStatus
									GROUP BY sp1.""Id"",sp1.""PostId"",sp1.""CreatedDate""
									ORDER BY sp1.""CreatedDate"" DESC
									LIMIT 1
								) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
								LEFT JOIN ""TagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
								LEFT JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
								[WhereMainQuery] 								
								GROUP BY qpost1.""Id"", psp1.""CreatedDate""
								ORDER BY psp1.""CreatedDate"" DESC
								LIMIT @PageSize
								OFFSET @Offet";
            }
        }
        private string GetTopLatestPostByTagToCountQuery
        {
            get
            {
                return @"SELECT qpost1.""Id""
								 FROM ""Posts"" qpost1
							 	INNER JOIN LATERAL (
								--Lastest subpost									 
									SELECT sp1.""Id"", sp1.""PostId""
									FROM ""SubPosts"" sp1 
									WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  AND sp1.""Status"" = @PostStatus
									GROUP BY sp1.""Id"", sp1.""PostId""
									-- LIMIT 1
								) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
								LEFT JOIN ""TagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
								LEFT JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
								WHERE (@TagName IS NULL OR qtag.""Name"" = @TagName) AND qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
								AND qpost1.""IsDelete"" = false 								
								GROUP BY qpost1.""Id""";
            }
        }
        private string GetTopLatestPostByMultiTagToCountQuery
        {
            get
            {
                return @"SELECT qpost1.""Id""
								 FROM ""Posts"" qpost1
							 	INNER JOIN LATERAL (
								--Lastest subpost									 
									SELECT sp1.""Id"", sp1.""PostId""
									FROM ""SubPosts"" sp1 
									WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  AND sp1.""Status"" = @PostStatus
									GROUP BY sp1.""Id"", sp1.""PostId""
									-- LIMIT 1
								) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
								LEFT JOIN ""TagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
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
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, psp1.""CreatedDate"", 1 AS ""SelectType""
								 FROM ""Posts"" qpost1
							 	INNER JOIN LATERAL (
								--Lastest subpost									 
									SELECT sp1.""Id"", sp1.""PostId"", sp1.""CreatedDate"" 
									FROM ""SubPosts"" sp1 
									WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  AND sp1.""Status"" = @PostStatus
									GROUP BY sp1.""Id"",sp1.""PostId"",sp1.""CreatedDate""
									ORDER BY sp1.""CreatedDate"" DESC
									LIMIT 1
								) psp1 ON psp1.""PostId"" = qpost1.""Id"" 

								INNER JOIN ""TagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
INNER JOIN ""TagFavorites"" tagfa ON qtp.""TagId"" = tagfa.""TagId""
								INNER JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 

								WHERE tagfa.""UserId"" = @UserId AND qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
								AND qpost1.""IsDelete"" = false 								
								GROUP BY qpost1.""Id"", psp1.""CreatedDate""
								ORDER BY psp1.""CreatedDate"" DESC
								LIMIT @PageSize
								OFFSET @Offet";
            }
        }
        private string GetTopLatestPostByFavoriteToCountQuery
        {
            get
            {
                return @"SELECT qpost1.""Id""
								 FROM ""Posts"" qpost1
							 	INNER JOIN LATERAL (
								--Lastest subpost									 
									SELECT sp1.""Id"", sp1.""PostId""
									FROM ""SubPosts"" sp1 
									WHERE sp1.""PostId"" = qpost1.""Id"" AND sp1.""IsDelete"" = false  AND sp1.""Status"" = @PostStatus
									GROUP BY sp1.""Id"", sp1.""PostId""
									-- LIMIT 1
								) psp1 ON psp1.""PostId"" = qpost1.""Id"" 
								LEFT JOIN ""TagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
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
                return @"SELECT qpost1.""Id"", 0 as COUNTCM, qpost1.""CreatedDate"", 2 AS ""SelectType""
								 FROM ""Posts"" qpost1

								WHERE  qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
								AND qpost1.""IsDelete"" = false AND qpost1.""IsCompleted"" = true					

								ORDER BY qpost1.""CreatedDate"" DESC
								LIMIT @PageSize
								OFFSET @Offet";
            }
        }
        private string GetTopLatestCompletePostToCountQuery
        {
            get
            {
                return @"SELECT qpost2.""Id""
								 FROM ""Posts"" qpost2
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
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, qpost1.""CreatedDate"", 4 AS ""SelectType""
								FROM ""Posts"" qpost1							 	
								WHERE  qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
								AND qpost1.""IsDelete"" = false 								
								GROUP BY qpost1.""Id"", qpost1.""CreatedDate""
								ORDER BY qpost1.""CreatedDate"" DESC
								LIMIT @PageSize";
            }
        }
        #endregion

        #region By tag

        private string GetLatestPostByTagHitQuery
        {
            get
            {
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, qpost1.""LastModifiedDate"" AS ""CreatedDate"", 3 AS ""SelectType""
								 FROM ""Posts"" qpost1		
							 	INNER JOIN ""TagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
								INNER JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
								WHERE  qtag.""Name"" = @TagName AND qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
								AND qpost1.""IsDelete"" = false 								
															
								GROUP BY qpost1.""Id"", qpost1.""LastModifiedDate""
								ORDER BY qpost1.""LastModifiedDate"" DESC
								LIMIT @PageSize
								OFFSET @Offet";
            }
        }
        private string GetLatestPostByTagToCountQuery
        {
            get
            {
                return @"SELECT qpost1.""Id""
								 FROM ""Posts"" qpost1
							 	INNER JOIN ""TagPosts"" qtp ON qtp.""PostId"" = qpost1.""Id""
								INNER JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
								WHERE  qtag.""Name"" = @TagName AND qpost1.""Type"" = @PostType 
AND qpost1.""Status"" = @PostStatus AND qpost1.""IsDelete"" = false							
								GROUP BY qpost1.""Id""";
            }
        }

        #endregion
        #region My series
        private string GetMyPostIdsQuery
        {
            get
            {
                return @" --My post
								SELECT qpost.""Id"", 0 AS ""SelectType""
                                FROM ""Posts"" qpost							 	
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
								FROM ""Posts"" qpost							 	
								WHERE qpost.""CreatedBy"" = @UserId AND qpost.""Type"" = @PostType AND qpost.""IsDelete"" = false";
            }
        }
        private string GetMyAllComicStoryQuery
        {
            get
            {
                return $@" SELECT ""HashId"", ""Title"", ""Type"", ""Status""
								FROM {_postRepository.TableName}							 	
								WHERE ""CreatedBy"" = @UserId AND ""IsDelete"" = false AND ""Type"" IN ({(int)PostType.STORY}, {(int)PostType.COMIC}) 
								ORDER BY ""CreatedDate"" DESC;";
            }
        }

        #endregion

        #region By User
        private string GetLatestPostByUserHitQuery
        {
            get
            {
                return @"SELECT DISTINCT qpost1.""Id"", 0 as COUNTCM, qpost1.""LastModifiedDate"" AS ""CreatedDate"", 3 AS ""SelectType""
								 FROM ""Posts"" qpost1		
							 	INNER JOIN identity.""Users"" user1 ON user1.""Id"" = qpost1.""UserId""								
								WHERE  user1.""ProfileName"" = @ProfileName AND qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
								AND qpost1.""IsDelete"" = false 								
															
								GROUP BY qpost1.""Id"", qpost1.""LastModifiedDate""
								ORDER BY qpost1.""LastModifiedDate"" DESC
								LIMIT @PageSize
								OFFSET @Offet";
            }
        }

        private string GetLatestPostByUserToCountQuery
        {
            get
            {
                return @"SELECT qpost1.""Id""
								 FROM ""Posts"" qpost1
							 	INNER JOIN identity.""Users"" user1 ON user1.""Id"" = qpost1.""UserId""								
								WHERE  user1.""ProfileName"" = @ProfileName AND qpost1.""Type"" = @PostType AND qpost1.""Status"" = @PostStatus
								AND qpost1.""IsDelete"" = false
								GROUP BY qpost1.""Id""";
            }
        }

        private string CheckUserFirstPost
        {
            get
            {
                return @"SELECT ""Id"" FROM ""Posts"" WHERE ""UserId"" = @UserId AND ""Type"" = @PostType LIMIT 1;";
            }
        }
        #endregion

        #region SubPost - View

        private string GetSeriesChapterByHashIdWithJoinOrder
        {
            get
            {
                return @"SELECT sp.""Id"",sp.""HashId"",sp.""Name"", sp.""Title"", sp.""PostId"", sp.""Order"", sp.""Body"", sp.""IsExclusive"", ux.""Id"" as ""UserExclusiveId"",
					sp.""Status"", sp.""CreatedDate"", sp.""CreatedBy"", sp.""LastModifiedDate"", 
					sp.""LastModifiedBy"", sp.""IsDelete"", count.""ViewCount"", sp.""AuthorId"", u.""ProfileName"",
					sp.""UserId"", sp.""PublishDate"", sp.""Permission"",sp.""CreatorNote"",
					sp.""IsEnableComment"", spcmc.""CommentCount"",
					rs.""Id"", rs.""AuthorId"", rs.""Title"", rs.""Name"", rs.""Url"", rs.""Type"", rs.""CreatedDate"", 
					rs.""CreatedBy"", rs.""LastModifiedDate"", rs.""LastModifiedBy"", rs.""IsDelete"", rs.""HashId"", rs.""SubPostId"", 
					rs.""Status"", rs.""Size"", rs.""LocationType"", rs.""Height"", rs.""Width"", rs.""Order""
					FROM public.""SubPosts"" sp
					INNER JOIN ""Posts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
LEFT JOIN ""UserExclusiveSubPosts"" ux ON ux.""SubPostId"" = sp.""Id"" AND ux.""UserId"" = @UserId
INNER JOIN identity.""Users"" u ON u.""Id"" = sp.""CreatedBy""
					LEFT JOIN ""Resources"" rs ON sp.""Id"" = rs.""SubPostId"" AND rs.""IsDelete"" = false
LEFT JOIN LATERAL 
							(
								SELECT COUNT(spcm.""Id"") as ""CommentCount"", spcm.""PostId"" as ""SubPostId""
FROM ""SubPostComments"" spcm 
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
				sp.""Status"", sp.""CreatedDate"", sp.""CreatedBy"", sp.""LastModifiedDate"", 
				sp.""LastModifiedBy"", sp.""IsDelete"", count.""ViewCount"", sp.""AuthorId"", 
				sp.""UserId"", sp.""PublishDate"", sp.""Permission"",sp.""CreatorNote"",
sp.""IsEnableComment""
					FROM public.""SubPosts"" sp
					INNER JOIN ""Posts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
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
                return @"SELECT sp.""Id"",sp.""HashId"",sp.""Name"", sp.""Title"", sp.""PostId"", sp.""Order"", sp.""Body"", sp.""IsExclusive"",ux.""Id"" as ""UserExclusiveId"",
				sp.""Status"", sp.""CreatedDate"", sp.""CreatedBy"", sp.""LastModifiedDate"", 
				sp.""LastModifiedBy"", sp.""IsDelete"", count.""ViewCount"", sp.""AuthorId"", 
				sp.""UserId"", sp.""PublishDate"", sp.""Permission"", sp.""CreatorNote"",
sp.""IsEnableComment"", spcmc.""CommentCount""
					FROM public.""SubPosts"" sp
LEFT JOIN ""UserExclusiveSubPosts"" ux ON ux.""SubPostId"" = sp.""Id"" AND ux.""UserId"" = @UserId
					INNER JOIN ""Posts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
LEFT JOIN LATERAL 
							(
								SELECT COUNT(spcm.""Id"") as ""CommentCount"", spcm.""PostId"" as ""SubPostId""
FROM ""SubPostComments"" spcm 
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
								FROM public.""SubPosts"" sp
								INNER JOIN ""Posts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
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
					FROM public.""SubPosts"" sp
					INNER JOIN ""Posts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
					WHERE p.""HashId"" = @PostHashId AND sp.""IsDelete"" = false
					ORDER BY sp.""Order"";

						SELECT COUNT(*) AS TotalItems 
						FROM (
								SELECT sp.""Id""			
								FROM public.""SubPosts"" sp
								INNER JOIN ""Posts"" p ON sp.""PostId"" = p.""Id"" AND p.""IsDelete"" = false
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
						post.""CreatedDate"",post.""AuthorName"", post.""CoverUrl"", post.""IsMature"",post.""IsCompleted"", post.""Permission"", post.""AuthorId"",
						post.""SubPostStr"", 
						array_agg(tag.""Name"") as Tags from
							(SELECT  p.""Id"",
							p.""Title"", p.""Body"",  
							p.""HashId"",p.""UserId"", sp.""Total"" AS ""ChapterCount"",
							u.""ProfileName"",u.""ProfileId"", p.""ThumbnailUrl"", 
							p.""AuthorName"", p.""CoverUrl"", p.""IsMature"",p.""IsCompleted"", p.""Permission"",p.""AuthorId"",
							 postid.""SelectType"",
							p.""Status"", p.""Type"", p.""ViewCount"",
							p.""CreatedDate"",--sp.""Id"" as ""SPID"",
							--sp.""ChapterCount"" AS ""ChapterCount"",
							to_jsonb(array_agg(sp.*)) AS ""SubPostStr""	
							 
							FROM ""Posts"" p
							  INNER JOIN-- Select Id
							 (
								[SelectPostIdsQuery] 
							) postid 
							 ON postid.""Id"" = p.""Id""
							LEFT JOIN identity.""Users"" u ON p.""UserId"" = u.""Id""		
							LEFT JOIN LATERAL 
							(
								SELECT ""Id"",""PostId"",""CreatedDate"",""Title"",""Order"", count(*) OVER() AS ""Total"" 
								FROM ""SubPosts"" sp 
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
							p.""CreatedDate""
							) 
						AS post
						LEFT JOIN ""TagPosts"" tp ON tp.""PostId"" = post.""Id""
						LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id"" 
						GROUP BY post.""SelectType"", post.""Id"",post.""Title"", post.""Body"", post.""HashId"", 
						post.""AuthorName"", post.""CoverUrl"", post.""IsMature"",post.""IsCompleted"", post.""Permission"",post.""AuthorId"",
						post.""UserId"",post.""ProfileName"",post.""ProfileId"", post.""ThumbnailUrl"", post.""ChapterCount"",
						post.""Status"", post.""Type"", post.""ViewCount"",
						post.""CreatedDate"",
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
                return @"UPDATE ""Posts""
					SET ""IsDelete"" = true	, ""LastModifiedDate"" = @Date, ""LastModifiedBy"" = @UserId
					WHERE ""Id"" = @PostId;

					UPDATE ""Resources""
					SET ""IsDelete"" = true, ""LastModifiedDate"" = @Date, ""LastModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM ""SubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE ""Resources"".""SubPostId"" = sp.""Id"";
	
					UPDATE ""SubPostReactions"" spr
					SET ""IsDelete"" = true, ""LastModifiedDate"" = @Date, ""LastModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM ""SubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE spr.""TargetId"" = sp.""Id"";
	
					UPDATE ""SubPostComments"" spr
					SET ""IsDelete"" = true, ""LastModifiedDate"" = @Date, ""LastModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM ""SubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE spr.""PostId"" = sp.""Id"";
	
					UPDATE ""SubPostCommentReactions"" spr
					SET ""IsDelete"" = true, ""LastModifiedDate"" = @Date, ""LastModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM ""SubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE spr.""TargetId"" = sp.""Id"";
	
					UPDATE ""TagPosts""
					SET ""IsDelete"" = true	, ""LastModifiedDate"" = @Date, ""LastModifiedBy"" = @UserId
					WHERE ""PostId"" = @PostId;
	
					UPDATE ""SubPosts""
					SET ""IsDelete"" = true	, ""LastModifiedDate"" = @Date, ""LastModifiedBy"" = @UserId
					WHERE ""PostId"" = @PostId;
	
					UPDATE ""PostReactions""
					SET ""IsDelete"" = true	, ""LastModifiedDate"" = @Date, ""LastModifiedBy"" = @UserId
					WHERE ""TargetId"" = @PostId;
	
					UPDATE ""PostComments""
					SET ""IsDelete"" = true	, ""LastModifiedDate"" = @Date, ""LastModifiedBy"" = @UserId
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
			FROM public.""SubPosts"" sp
			INNER JOIN ""Posts"" p ON sp.""PostId"" = p.""Id""
			WHERE p.""HashId"" = @HashId AND sp.""Order"" = @Order
			AND p.""IsDelete"" = false AND sp.""IsDelete"" = false;";
            }
        }
        private string GetSubPostsWithHashIdAndOrders

        {
            get
            {
                return @"SELECT sp.""Id"", sp.""Title"", sp.""PostId"", sp.""Order"", sp.""Body"", sp.""IsExclusive"",
				sp.""Status"", sp.""CreatedDate"", sp.""CreatedBy"", sp.""LastModifiedDate"", 
				sp.""LastModifiedBy"", sp.""IsDelete"", sp.""ViewCount"", sp.""AuthorId"", 
				sp.""UserId"", sp.""PublishDate"", sp.""Permission"", sp.""CreatorNote"",
sp.""IsEnableComment""				
			FROM public.""SubPosts"" sp
			INNER JOIN ""Posts"" p ON sp.""PostId"" = p.""Id""
			WHERE p.""HashId"" = @HashId AND (sp.""Order"" = @Order1 OR sp.""Order"" = @Order2 )
			AND p.""IsDelete"" = false AND sp.""IsDelete"" = false;";
            }
        }
        private string ExecSoftDeleteSubPost
        {
            get
            {
                return @"UPDATE ""SubPosts""
					SET ""Order"" = 0, ""IsDelete"" = true, ""LastModifiedDate"" = @Date, ""LastModifiedBy"" = @UserId
					WHERE ""Id"" = @SubPostId;

					UPDATE ""Resources""
					SET ""IsDelete"" = true, ""LastModifiedDate"" = @Date, ""LastModifiedBy"" = @UserId
					WHERE ""SubPostId"" = @SubPostId;
	
					UPDATE ""SubPostReactions"" 
					SET ""IsDelete"" = true, ""LastModifiedDate"" = @Date, ""LastModifiedBy"" = @UserId
					WHERE ""TargetId"" = @SubPostId;
	
					UPDATE ""SubPostComments""
					SET ""IsDelete"" = true, ""LastModifiedDate"" = @Date, ""LastModifiedBy"" = @UserId
					WHERE ""PostId"" = @SubPostId;
	
					UPDATE ""SubPostCommentReactions"" 
					SET ""IsDelete"" = true, ""LastModifiedDate"" = @Date, ""LastModifiedBy"" = @UserId
					WHERE ""TargetId"" = @SubPostId;";
            }
        }

        private string GetLatestPostsByTypeQuery
        {
            get
            {
                return $@"
                      WITH ranked_posts AS (
          SELECT ""Id"", ""Type"", ""CreatedDate"", ""HashId"",
              ROW_NUMBER() OVER (PARTITION BY ""Type"" ORDER BY ""CreatedDate"" DESC) AS type_rank
          FROM ""Posts""
          WHERE ""Type"" IN (0, 1, 2)
          AND ""IsDelete"" = false
          AND ""Status"" = {(int)PostStatus.PUBLIC}
         ),
         limited_posts AS (
          SELECT ""Id"", ""Type"", ""CreatedDate"", ""HashId""
          FROM ranked_posts
          WHERE (""Type"" = 0 AND type_rank <= @feed)
             OR (""Type"" = 1 AND type_rank <= @story)
             OR (""Type"" = 2 AND type_rank <= @comic)
         ),
         numbered_posts AS (
          SELECT ""Id"", ""Type"", ""CreatedDate"", ""HashId"",
              ROW_NUMBER() OVER (PARTITION BY ""Type"" ORDER BY ""CreatedDate"" DESC) AS num
          FROM limited_posts
         ),
         grouped_posts AS (
          SELECT ""Id"", ""Type"", ""CreatedDate"", ""HashId"",
              CEILING(CAST(num AS FLOAT) / 
              CASE
               WHEN ""Type"" = 0 THEN @feedPercent *10
               WHEN ""Type"" = 1 THEN @storyPercent *10
               WHEN ""Type"" = 2 THEN @comicPercent *10
              END) AS group_number
          FROM numbered_posts
         ),
         final_grouped_posts AS (
          SELECT ""Id"", ""Type"", ""CreatedDate"", ""HashId"", group_number,
              ROW_NUMBER() OVER (PARTITION BY group_number ORDER BY RANDOM())  AS random_row_num
          FROM grouped_posts
         )
         SELECT ""Id"", ""Type"", ""CreatedDate"", ""HashId"", group_number
         FROM final_grouped_posts
         WHERE random_row_num <= 10
         ORDER BY group_number, random_row_num;

          [GetTotalCount]";
            }
        }

        private string GetLatestPostsByTagQuery
        {
            get
            {
                return $@"
                     WITH ranked_posts AS (
    SELECT p.""Id"", p.""Type"", p.""CreatedDate"", p.""HashId"",
           ROW_NUMBER() OVER (PARTITION BY ""Type"" ORDER BY p.""CreatedDate"" DESC) AS type_rank
    FROM ""Posts"" p
LEFT JOIN ""TagPosts"" tp on p.""Id"" = tp.""PostId""				
LEFT JOIN ""Tags"" t on t.""Id"" = tp.""TagId""
    WHERE ""Type"" IN (0, 1, 2)
    AND t.""Name"" ILIKE @ExactKeyword   
    AND p.""IsDelete"" = false
    AND p.""Status"" = 1
),
limited_posts AS (
    SELECT ""Id"", ""Type"", ""CreatedDate"", ""HashId""
    FROM ranked_posts
     WHERE (""Type"" = 0 AND type_rank <= @feed)
               OR (""Type"" = 1 AND type_rank <= @story)
               OR (""Type"" = 2 AND type_rank <= @comic)
),
numbered_posts AS (
    SELECT ""Id"", ""Type"", ""CreatedDate"", ""HashId"",
           ROW_NUMBER() OVER (PARTITION BY ""Type"" ORDER BY ""CreatedDate"" DESC) AS num
    FROM limited_posts
),
grouped_posts AS (
    SELECT ""Id"", ""Type"", ""CreatedDate"", ""HashId"",
           CEIL(num / CASE
          WHEN ""Type"" = 0 THEN @feedPercent *10
               WHEN ""Type"" = 1 THEN @storyPercent *10
               WHEN ""Type"" = 2 THEN @comicPercent *10
           END) AS group_number
    FROM numbered_posts
),
final_grouped_posts AS (
    SELECT ""Id"", ""Type"", ""CreatedDate"", ""HashId"", group_number,
           ROW_NUMBER() OVER (PARTITION BY group_number ORDER BY RANDOM()) AS random_row_num
    FROM grouped_posts
)
SELECT ""Id"", ""Type"", ""CreatedDate"", ""HashId"", group_number
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
												   FROM ""Posts""
												   WHERE ""IsDelete"" = false 
												   AND ""Status"" = {(int)PostStatus.PUBLIC}";

        private string GetCountPostByTagQuery => $@"SELECT COUNT(*) 
												   FROM ""Posts"" p
												   LEFT JOIN ""TagPosts"" tp on p.""Id"" = tp.""PostId""
												   LEFT JOIN ""Tags"" t on t.""Id"" = tp.""TagId""
												   WHERE p.""IsDelete"" = false 
												   AND p.""Status"" = {(int)PostStatus.PUBLIC}
												   AND t.""Name"" ILIKE @ExactKeyword";
        private string PremiumWhereQuery
        {
            get
            {
                return @" AND (0). ";
            }
        }

        #region Report post
        private string GetPostReportByPostIdAndUserId
        {
            get
            {
                return $@"SELECT ""Id"" FROM {_postReportRepository.TableName}
                          WHERE ""PostId"" = @postId AND ""UserId"" = @userId";
            }
        }
        private string GetPostReportByPostId
        {
            get
            {
                return $@"SELECT ""Id"" FROM {_postReportRepository.TableName}
                          WHERE ""PostId"" = @postId";
            }
        }
        #endregion

        private string GetPostRandomIdsQuery
        {
            get
            {
                return @"
                        WITH newtable 
                        AS
                        (
                        SELECT * FROM ""Posts"" p
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
                    p.""ViewCount"",
					p.""IsMature"",
					p.""CreatedDate"",
                    to_json(array_agg(distinct(sp.*)) FILTER (WHERE sp.* IS NOT NULL))AS ""SubPosts"",
				    to_json(array_agg(distinct (t.""Name""))  FILTER (WHERE t.""Name"" IS NOT NULL)) AS ""Tags""
                FROM ""Posts"" p
                LEFT JOIN ""TagPosts"" tp ON p.""Id"" = tp.""PostId""
                LEFT JOIN ""Tags"" t ON tp.""TagId"" = t.""Id""
                LEFT JOIN (
                    SELECT ""PostId"",
                           ""Title"",
                           ""Order"",
                           ""CreatedDate"",
                           ROW_NUMBER() OVER (PARTITION BY ""PostId"" ORDER BY ""Order"" desc) AS rn
                    FROM ""SubPosts""
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
						  p.""ViewCount""
            ";
            }
        }
    }
}
