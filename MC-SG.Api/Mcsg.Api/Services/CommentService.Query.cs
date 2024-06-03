using Mcsg.Lib.Data.Enums;

namespace Mcsg.Api.Services
{
    public partial class CommentService
    {
        private string GetCommentOfPostQuery
        {
            get
            {
                return @$"WITH RECURSIVE cte AS (
	                               SELECT ""Id"", ""ParentId"", ""PostId""
				                            , ""AuthorId"", ""LastModifiedDate""
				                            , ""Body"", ""ResourceId"", ""GifId"", ""IsDelete"", ""QuoteId"", 1 AS CommentLevel
	                               FROM {_postCommentRepository.TableName}
	                               WHERE ""ParentId"" IS NULL AND ""PostId"" = @PostId
	                               UNION ALL
	                               SELECT post.""Id"", post.""ParentId"", post.""PostId""
				                            , post.""AuthorId"", post.""LastModifiedDate""
				                            , post.""Body"", post.""ResourceId"", post.""GifId"", post.""IsDelete"", post.""QuoteId"", ct.CommentLevel + 1
	                               FROM cte ct
	                               JOIN {_postCommentRepository.TableName} post ON post.""ParentId"" = ct.""Id""
	                            )
	                            SELECT cte.""Id"" , cte.""ParentId"", cte.""PostId"", cte.""Body"", cte.""LastModifiedDate""
				                            , cte.""AuthorId"", (CASE WHEN us.""ProfileName"" IS NULL THEN us.""UserName""  ELSE us.""ProfileName"" END) AS AuthorName
											, us.""Avatar"" AS UserAvatar
				                            , cte.""ResourceId"", res.""HashId"" AS ResourceHashId, res.""Name"" AS ResourceName, res.""Url"" AS ResourceUrl, cte.""GifId""
				                            , cte.CommentLevel
											, cte.""QuoteId""
	                            FROM cte
	                            LEFT JOIN {_userRepository.TableName} us ON cte.""AuthorId"" = us.""Id""
	                            LEFT JOIN {_resourceRepository.TableName} res ON cte.""ResourceId"" = res.""Id""
                                WHERE cte.""IsDelete"" = false
	                            ORDER BY cte.CommentLevel, cte.""{{0}}"" DESC";
            }
        }
        private string GetCommentOfSubPostQuery
        {
            get
            {
                return @$"WITH RECURSIVE cte AS (
	                               SELECT ""Id"", ""ParentId"", ""PostId""
				                            , ""AuthorId"", ""LastModifiedDate""
				                            , ""Body"", ""ResourceId"", ""GifId"", ""IsDelete"", 1 AS CommentLevel
	                               FROM {_subPostCommentRepository.TableName}
	                               WHERE ""ParentId"" IS NULL AND ""PostId"" = @PostId
	                               UNION ALL
	                               SELECT post.""Id"", post.""ParentId"", post.""PostId""
				                            , post.""AuthorId"", post.""LastModifiedDate""
				                            , post.""Body"", post.""ResourceId"", post.""GifId"", post.""IsDelete"", ct.CommentLevel + 1
	                               FROM cte ct
	                               JOIN {_subPostCommentRepository.TableName} post ON post.""ParentId"" = ct.""Id""
	                            )
	                            SELECT cte.""Id"" , cte.""ParentId"", cte.""PostId"", cte.""Body"", cte.""LastModifiedDate""
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
		                        , com.""Body"", com.""LastModifiedDate""
		                        , res.""HashId"" AS ResourceHashId, res.""Name"" AS ResourceName, res.""Url"" AS ResourceUrl
								, com.""GifId""
		                        , rep.""Id"" AS ReplyId, (CASE WHEN repUser.""ProfileName"" IS NULL THEN repUser.""UserName""  ELSE repUser.""ProfileName"" END) AS ReplyAuthorName
								, repUser.""Avatar"" AS ReplyUserAvatar	
								, rep.""Body"" AS ReplyBody, rep.""LastModifiedDate"" AS ReplyLastModifiedDate
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
                        ORDER BY com.""LastModifiedDate"" DESC
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
		                        , com.""Body"", com.""LastModifiedDate""
		                        , res.""HashId"" AS ResourceHashId, res.""Name"" AS ResourceName, res.""Url"" AS ResourceUrl
								, com.""GifId""
		                        , rep.""Id"" AS ReplyId, (CASE WHEN repUser.""ProfileName"" IS NULL THEN repUser.""UserName""  ELSE repUser.""ProfileName"" END) AS ReplyAuthorName
								, repUser.""Avatar"" AS ReplyUserAvatar	
		                        , rep.""Body"" AS ReplyBody, rep.""LastModifiedDate"" AS ReplyLastModifiedDate
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
                        ORDER BY com.""LastModifiedDate"" DESC
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
						, men.""Length"", men.""Offset"", men.""Text""
						FROM public.""Mentions"" men 
						LEFT JOIN public.""Users"" use ON men.""EntityId"" = use.""Id"" 
														AND men.""EntityType"" = {(int)MentionEntityType.User}
						WHERE men.""EntityType"" = {(int)MentionEntityType.User} 
								AND men.""LocationId"" = ANY(@LocationIds) AND men.""IsDelete"" = false";
            }
        }
    }
}
