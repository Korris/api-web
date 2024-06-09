using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Model.Enums;

namespace Mcsg.Social.Api.Services
{
    public partial class FavoriteService
    {
        private string GetFavoriteTagQuery
        {
            get
            {
                return @$"select tag.""Id"", tag.""Name"" 
                            from {_tagRepository.TableName} tag 
                            inner join {_tagFavoriteRepository.TableName} tagfavorites on tag.""Id"" = tagfavorites.""TagId"" 
                            where tagfavorites.""UserId"" = '{_currentUserService.Session.UserId}' AND tag.""IsDelete"" = false 
                            order by tag.""Name""
                            LIMIT @PageSize
                            OFFSET @Offet;

                          select count(tagfavorites.*) AS TotalItems 
                            from {_tagRepository.TableName} tag 
                            inner join {_tagFavoriteRepository.TableName} tagfavorites on tag.""Id"" = tagfavorites.""TagId"" 
                            where tagfavorites.""UserId"" = '{_currentUserService.Session.UserId}' AND tag.""IsDelete"" = false ;
                        ";
            }
        }

        private string DeletePostFavoriteByUserIdAndPostIdQuery
        {
            get
            {
                return @$"DELETE FROM {_postFavoriteRepository.TableName} 
                          WHERE ""PostId"" = @postId AND ""UserId"" = @userId";
            }
        }

        private string DeleteTagFavoriteByUserIdAndTagIdQuery
        {
            get
            {
                return @$"DELETE FROM {_tagFavoriteRepository.TableName} 
                          WHERE ""TagId"" = @tagId AND ""UserId"" = @userId";
            }
        }

        private string GetTagFavoriteByTagIdAndUserId
        {
            get
            {
                return $@"SELECT ""Id"" FROM {_tagFavoriteRepository.TableName}
                          WHERE ""TagId"" = @tagId AND ""UserId"" = @userId";
            }
        }

        private string GetPostFavoriteByPostIdAndUserId
        {
            get
            {
                return $@"SELECT ""Id"" FROM {_postFavoriteRepository.TableName}
                          WHERE ""PostId"" = @postId AND ""UserId"" = @userId";
            }
        }

        private string GetFavoritePostQuery
        {
            get
            {
                return @$"SELECT post.""Id"", post.""Body"" , us.""Id"" AS ""AuthorId""
                            , (CASE WHEN us.""ProfileName"" IS NULL THEN us.""UserName""  ELSE us.""ProfileName"" END) AS AuthorName
                            , us.""Avatar"" AS AuthorAvatar
                            FROM {_postRepository.TableName} post 
                            INNER JOIN {_postFavoriteRepository.TableName} postFavorites ON post.""Id"" = postFavorites.""PostId"" 
                            LEFT JOIN {_userRepository.TableName} us ON post.""CreatedBy"" = us.""Id""
                            WHERE postFavorites.""UserId"" = '{_currentUserService.Session.UserId}' AND post.""IsDelete"" = false 
                            ORDER BY post.""Title""
                            LIMIT @PageSize
                            OFFSET @Offet;

                          SELECT count(postFavorites.*) AS TotalItems 
                            FROM {_postRepository.TableName} post 
                            INNER JOIN {_postFavoriteRepository.TableName} postFavorites ON post.""Id"" = postFavorites.""PostId"" 
                            WHERE postFavorites.""UserId"" = '{_currentUserService.Session.UserId}' AND post.""IsDelete"" = false ;
                        ";
            }
        }

		private string GetFavoritePostByUserQuery
		{
			get
			{
				return @$"SELECT post.""Id"",post.""Title"", post.""Body"", post.""HashId"",
						post.""Avatar"" AS UserAvatar,post.""UserId"", post.""ProfileName"",post.""ProfileId"", post.""ThumbnailUrl"", 
						post.""Status"", post.""Type"",
						post.""CreatedDate"",
						post.""TotalResource"",
						post.""SubPostStr"",
						post.""SubPostResourceStr"",
						post.""MetaTitle"",
						post.""MetaDescription"",
						post.""MetaUrl"",
						post.""MetaDomain"",
						post.""LinkHashId"",
						post.""LinkUrl"",
						post.""LinkType"", 
						post.""CustomNote"", array_agg(tag.""Name"") as Tags from
							(SELECT  p.""Id"",
							p.""Title"", p.""Body"",  
							p.""HashId""
							,p.""UserId"",u.""Avatar"", u.""ProfileName"",u.""ProfileId"", p.""ThumbnailUrl"", 
							p.""Status"", p.""Type"", 
							p.""CreatedDate"",
							p.""CustomNote"",
							sp.""Total"" AS ""TotalResource"",
							to_jsonb(array_agg(sp.*)) AS ""SubPostStr"",
							to_jsonb(array_agg(spr.*)) AS ""SubPostResourceStr"",
							md.""Title"" AS ""MetaTitle"",
							md.""Description"" AS ""MetaDescription"",
							md.""Url"" AS ""MetaUrl"",
							md.""Domain"" AS ""MetaDomain"",
							pl.""HashId"" AS ""LinkHashId"",
							pl.""Url"" AS ""LinkUrl"",
							pl.""Type"" AS ""LinkType""
							FROM {_postRepository.TableName} p
							INNER JOIN {_postFavoriteRepository.TableName} pf ON p.""Id"" = pf.""PostId""
							LEFT JOIN {_userRepository.TableName} u ON p.""UserId"" = u.""Id""						
							LEFT JOIN {_metaDataRepository.TableName} md ON md.""PostId"" = p.""Id""
							LEFT JOIN {_postLinkRepository.TableName} pl ON pl.""PostId"" = p.""Id"" AND pl.""IsDelete"" = false
							LEFT JOIN LATERAL 
							(
								SELECT ""Id"",""HashId"",""PostId"",""Order"", count(*) OVER() AS ""Total"" 
								FROM {_subPostRepository.TableName} sp 
								WHERE ""PostId"" = p.""Id"" AND ""IsDelete"" = false
								GROUP BY ""Id"", ""PostId""
								ORDER BY ""Order""
								LIMIT 5
							) sp ON sp.""PostId"" = p.""Id""
							LEFT JOIN LATERAL
							(
								SELECT ""SubPostId"",""Type"",""Status"",""ShareUrl"",""Url"",""Name"",""HashId"",""Width"",""Height"",sp.""Order""
							 	FROM {_resourceRepository.TableName} 
							 	WHERE ""SubPostId"" = sp.""Id"" AND ""IsDelete"" = false
								LIMIT 1
							) spr ON spr.""SubPostId"" = sp.""Id""
							WHERE 
								pf.""UserId"" = @UserId
								AND p.""IsDelete"" = false AND p.""Type"" = @Type AND p.""Status"" = {(int)PostStatus.PUBLIC}
							GROUP BY p.""Id"",p.""Title"", p.""Body"", p.""HashId"", p.""UserId"", 
							u.""Avatar"",u.""ProfileName"",u.""ProfileId"", p.""ThumbnailUrl"", 
							p.""Status"", p.""Type"",
							p.""CreatedDate"",
							p.""CustomNote"",
							sp.""Total"",
							md.""Title"",
							md.""Description"",
							md.""Url"",
							md.""Domain"",
							pl.""HashId"",
							pl.""Url"",
							pl.""Type""
							ORDER BY p.""CreatedDate"" DESC
						LIMIT @PageSize
						OFFSET @Offet) 
						AS post
						LEFT JOIN {_tagPostRepository.TableName} tp ON tp.""PostId"" = post.""Id""
						LEFT JOIN {_tagRepository.TableName} tag ON tp.""TagId"" = tag.""Id"" 
						GROUP BY post.""Id"",post.""Title"", post.""Body"", post.""HashId"", 
						post.""Avatar"",post.""UserId"",post.""ProfileName"",post.""ProfileId"", post.""ThumbnailUrl"", 
						post.""Status"", post.""Type"", 
						post.""CreatedDate"",
						post.""TotalResource"",
						post.""SubPostStr"",
						post.""SubPostResourceStr"",
						post.""MetaTitle"",
						post.""MetaDescription"",
						post.""MetaUrl"",
						post.""MetaDomain"",
						post.""CustomNote"",
						post.""LinkHashId"",
						post.""LinkUrl"",
						post.""LinkType""
						ORDER BY post.""CreatedDate"" DESC;

                        SELECT count(postFavorites.*) AS TotalItems 
                        FROM {_postRepository.TableName} post 
                        INNER JOIN {_postFavoriteRepository.TableName} postFavorites ON post.""Id"" = postFavorites.""PostId"" 
                        WHERE postFavorites.""UserId"" = '{_currentUserService.Session.UserId}' AND post.""IsDelete"" = false ;
                        ";
			}
		}
	}
}
