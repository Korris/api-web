namespace Mcsg.Social.Api.Services
{
    public partial class FeedService
    {
        private string GetAllFeedsQuery
        {
            get
            {
                return @"SELECT post.""Id"",post.""Title"", post.""Body"", post.""HashId"",
						post.""Avatar"" AS UserAvatar,post.""UserId"", post.""ProfileName"", u.""UserName"",post.""ProfileId"", post.""ThumbnailUrl"", 
						post.""Status"", post.""Type"",
						post.""CreatedOn"",
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
						post.""CustomNote"", array_agg(tag.""Name"") as Tags, u.""UserName"" from
							(SELECT  p.""Id"",
							p.""Title"", p.""Body"",  
							p.""HashId"",p.""UserId"",u.""Avatar"", u.""ProfileName"",u.""ProfileId"", p.""ThumbnailUrl"", 
							p.""Status"", p.""Type"", 
							p.""CreatedOn"",
							p.""CustomNote"",--sp.""Id"" as ""SPID"",
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
							FROM social.""Posts"" p
							LEFT JOIN identity.""Users"" u ON p.""UserId"" = u.""Id""						
							LEFT JOIN ""MetaDatas"" md ON md.""PostId"" = p.""Id""
							LEFT JOIN ""PostLinks"" pl ON pl.""PostId"" = p.""Id"" AND pl.""IsDelete"" = false
							LEFT JOIN LATERAL 
							(
								SELECT ""Id"",""HashId"",""PostId"",""Order"",""Body"", count(*) OVER() AS ""Total"" FROM social.""SubPosts"" sp 
								WHERE ""PostId"" = p.""Id"" AND ""IsDelete"" = false
								GROUP BY ""Id"", ""PostId""
								ORDER BY ""Order""
								LIMIT 5
							) sp ON sp.""PostId"" = p.""Id""
							LEFT JOIN LATERAL
							(
								SELECT ""SubPostId"",""Type"",""Status"",""ShareUrl"",""Url"",""Name"",r.""HashId"",""Width"",""Height"",sp.""Order"",sp.""Body"",sp.""HashId"" AS SubPostHashId 
								FROM social.""Resources"" r
								WHERE ""SubPostId"" = sp.""Id"" AND ""IsDelete"" = false
								LIMIT 1
							) spr ON spr.""SubPostId"" = sp.""Id""
							WHERE 
							 p.""IsDelete"" = false [AdditionalCondition] and  p.""Type"" = @Type AND p.""Status"" = @Status
							-- TODO AND (@IsAccessPrivate = true OR p.""IsPrivate"" = false )
							GROUP BY p.""Id"",p.""Title"", p.""Body"", p.""HashId"", p.""UserId"", 
							u.""Avatar"",u.""ProfileName"",u.""UserName"",u.""ProfileId"", p.""ThumbnailUrl"",
							p.""Status"", p.""Type"",
							p.""CreatedOn"",
							p.""CustomNote"",
							sp.""Total"",
							md.""Title"",
							md.""Description"",
							md.""Url"",
							md.""Domain"",
							pl.""HashId"",
							pl.""Url"",
							pl.""Type""
							ORDER BY p.""{1}"" DESC
						LIMIT @PageSize
						OFFSET @Offet) 
						AS post
						LEFT JOIN social.""TagPosts"" tp ON tp.""PostId"" = post.""Id""
						LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id"" 
						LEFT JOIN identity.""Users"" u ON u.""Id"" = post.""UserId""
						GROUP BY post.""Id"",post.""Title"", post.""Body"", post.""HashId"", 
						post.""Avatar"",post.""UserId"",post.""ProfileName"",post.""ProfileId"", post.""ThumbnailUrl"", 
						post.""Status"", post.""Type"", 
						post.""CreatedOn"",
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
						post.""LinkType"",
						u.""UserName""
						ORDER BY post.""{1}"" DESC;

						SELECT COUNT(*) AS TotalItems FROM {0} p [AdditionalTotalQuery] WHERE p.""Type"" = @Type 
						AND p.""IsDelete"" = false [AdditionalTotalCondition]
						-- TODO AND (@IsAccessPrivate = true OR p.""IsPrivate"" = false);";
            }
        }
        private string GetAllFeedsByTagQuery
        {
            get
            {
                return @"SELECT post.""Id"",post.""Title"", post.""Body"", post.""HashId"",
						post.""Avatar"" AS UserAvatar,post.""UserId"", post.""ProfileName"",post.""ProfileId"", post.""ThumbnailUrl"", 
						post.""Status"", post.""Type"",
						post.""CreatedOn"",
						post.""TotalResource"",
						post.""SubPostStr"",
						post.""SubPostResourceStr"",
						post.""MetaTitle"",
						post.""MetaDescription"",
						post.""MetaUrl"",
						post.""MetaDomain"", 
						post.""CustomNote"", array_agg(tag.""Name"") as Tags from
							(SELECT  p.""Id"",
							p.""Title"", p.""Body"",  
							p.""HashId"",p.""UserId"",u.""Avatar"", u.""ProfileName"",u.""ProfileId"", p.""ThumbnailUrl"", 
							p.""Status"", p.""Type"", 
							p.""CreatedOn"",
							p.""CustomNote"", --sp.""Id"" as ""SPID"",
							sp.""Total"" AS ""TotalResource"",
							to_jsonb(array_agg(sp.*)) AS ""SubPostStr"",
							to_jsonb(array_agg(spr.*)) AS ""SubPostResourceStr"",
							md.""Title"" AS ""MetaTitle"",
							md.""Description"" AS ""MetaDescription"",
							md.""Url"" AS ""MetaUrl"",
							md.""Domain"" AS ""MetaDomain""
							FROM (
								SELECT DISTINCT qpost.* FROM social.""Posts"" qpost
							 	INNER JOIN social.""TagPosts"" qtp ON qtp.""PostId"" = qpost.""Id""
								INNER JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
								WHERE  qtag.""Name"" = @TagName AND qpost.""Type"" = @PostType
								AND qpost.""IsDelete"" = false 
								-- TODO AND (@IsAccessPrivate = true OR qpost.""IsPrivate"" = false )
								ORDER BY qpost.""{1}"" DESC
								LIMIT @PageSize
								OFFSET @Offet
							) p
							LEFT JOIN identity.""Users"" u ON p.""UserId"" = u.""Id""						
							LEFT JOIN ""MetaDatas"" md ON md.""PostId"" = p.""Id""
							LEFT JOIN LATERAL 
							(
								SELECT ""Id"",""HashId"",""PostId"",""Order"", count(*) OVER() AS ""Total"" FROM social.""SubPosts"" sp 
								WHERE ""PostId"" = p.""Id""
								GROUP BY ""Id"", ""PostId""
								ORDER BY ""Order""
								LIMIT 5
							) sp ON sp.""PostId"" = p.""Id""
							LEFT JOIN LATERAL
							(
								SELECT ""SubPostId"",""Type"",""Status"",""ShareUrl"",""Url"",""Name"",""HashId"",""Width"",""Height"",sp.""Order""
							 	FROM social.""Resources"" 
							 	WHERE ""SubPostId"" = sp.""Id""
								LIMIT 1
							) spr ON spr.""SubPostId"" = sp.""Id""
							
							GROUP BY p.""Id"",p.""Title"", p.""Body"", p.""HashId"", p.""UserId"", 
							u.""Avatar"",u.""ProfileName"", u.""ProfileId"", p.""ThumbnailUrl"", 
							p.""Status"", p.""Type"",
							p.""CreatedOn"",
							p.""CustomNote"",
							sp.""Total"",
							md.""Title"",
							md.""Description"",
							md.""Url"",
							md.""Domain""
							) 
						AS post
						LEFT JOIN social.""TagPosts"" tp ON tp.""PostId"" = post.""Id""
						LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id"" 
						GROUP BY post.""Id"",post.""Title"", post.""Body"", post.""HashId"", 
						post.""Avatar"",post.""UserId"",post.""ProfileName"",post.""ProfileId"", post.""ThumbnailUrl"", 
						post.""Status"", post.""Type"", 
						post.""CreatedOn"",
						post.""TotalResource"",
						post.""SubPostStr"",
						post.""SubPostResourceStr"",
						post.""MetaTitle"",
						post.""MetaDescription"",
						post.""MetaUrl"",
						post.""MetaDomain"",
						post.""CustomNote""
						ORDER BY post.""{1}"" DESC;

						SELECT COUNT(*) AS TotalItems 
						FROM (SELECT DISTINCT qpost.""Id"" FROM {0} qpost
							 	INNER JOIN social.""TagPosts"" qtp ON qtp.""PostId"" = qpost.""Id""
								INNER JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
								WHERE qtag.""Name"" = @TagName AND qpost.""Type"" = @PostType
								AND qpost.""IsDelete"" = false) p;";
            }
        }

        private string GetAllFeedsWithTopCommentQuery
        {
            get
            {
                return @"SELECT post.""Id"",post.""Title"", post.""Body"", post.""HashId"",
						post.""Avatar"" AS UserAvatar,post.""UserId"", post.""ProfileName"",post.""ProfileId"", post.""ThumbnailUrl"", 
						post.""Status"", post.""Type"", post.commentcount,
						post.""CreatedOn"",
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
							p.""HashId"",p.""UserId"",u.""Avatar"", u.""ProfileName"",u.""ProfileId"", p.""ThumbnailUrl"", 
							p.""Status"", p.""Type"", p.commentcount,
							p.""CreatedOn"",
							p.""CustomNote"", --sp.""Id"" as ""SPID"",
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
							FROM (
								SELECT  smart.""Count"" as commentcount, qpost.*
								FROM social.""Posts"" qpost
								INNER JOIN LATERAL (
								SELECT 
								""EntityType"", 
								""EntityId"", 
								SUM(""Count"") as ""Count""
									FROM ""SmartCountActions"" 
								WHERE ""EntityId"" = qpost.""Id"" AND ""Date"" >= @DateOnly AND (""ActionType"" = 0 OR ""ActionType"" = 1)
																	AND ""EntityType"" = 0 AND ""SubType"" = @Type
									GROUP BY ""EntityId"", ""EntityType"", ""EntityId"",""SubType""
LIMIT @PageSize
								OFFSET @Offet
								) smart
								ON smart.""EntityId"" = qpost.""Id"" 
								WHERE qpost.""Type"" = @Type
								ORDER BY smart.""Count"" DESC
								LIMIT @PageSize
								OFFSET @Offet
							) p
							LEFT JOIN identity.""Users"" u ON p.""UserId"" = u.""Id""						
							LEFT JOIN ""MetaDatas"" md ON md.""PostId"" = p.""Id""
							LEFT JOIN ""PostLinks"" pl ON pl.""PostId"" = p.""Id"" AND pl.""IsDelete"" = false
							LEFT JOIN LATERAL 
							(
								SELECT ""Id"",""PostId"",""Order"", count(*) OVER() AS ""Total"" FROM social.""SubPosts"" sp 
								WHERE ""PostId"" = p.""Id"" AND ""IsDelete"" = false
								GROUP BY ""Id"", ""PostId""
								ORDER BY ""Order""
								LIMIT 5
							) sp ON sp.""PostId"" = p.""Id""
							LEFT JOIN LATERAL
							(
								SELECT ""SubPostId"",""Type"",""Status"",""ShareUrl"",""Url"",""Name"",""HashId"",""Width"",""Height"",sp.""Order""
							 	FROM social.""Resources"" 
							 	WHERE ""SubPostId"" = sp.""Id"" AND ""IsDelete"" = false
								LIMIT 1
							) spr ON spr.""SubPostId"" = sp.""Id""
							
							GROUP BY p.""Id"", p.commentcount ,p.""Title"", p.""Body"", p.""HashId"", p.""UserId"", 
							u.""Avatar"",u.""ProfileName"", u.""ProfileId"", p.""ThumbnailUrl"", 
							p.""Status"", p.""Type"",
							p.""CreatedOn"",
							p.""CustomNote"",
							sp.""Total"",
							md.""Title"",
							md.""Description"",
							md.""Url"",
							md.""Domain"",
							pl.""HashId"",
							pl.""Url"",
							pl.""Type""
							) 
						AS post
						LEFT JOIN social.""TagPosts"" tp ON tp.""PostId"" = post.""Id""
						LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id"" 
						GROUP BY post.""Id"",post.""Title"", post.""Body"", post.""HashId"", 
						post.""Avatar"",post.""UserId"",post.""ProfileName"",post.""ProfileId"", post.""ThumbnailUrl"", 
						post.""Status"", post.""Type"", post.commentcount,
						post.""CreatedOn"",
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
						ORDER BY post.commentcount DESC;

						SELECT COUNT(*) AS TotalItems 
						FROM (SELECT  qpost.""Id""
								FROM social.""Posts"" qpost
								INNER JOIN LATERAL (
									SELECT 
									""EntityId""
										FROM ""SmartCountActions"" 
									WHERE ""EntityId"" = qpost.""Id"" AND ""Date"" >= @DateOnly AND (""ActionType"" = 0 OR ""ActionType"" = 1)
																		AND ""EntityType"" = 0 AND ""SubType"" = @Type
										GROUP BY ""EntityId"", ""EntityType"", ""EntityId"",""SubType""
									) smart
								ON smart.""EntityId"" = qpost.""Id"" 
								WHERE qpost.""Type"" = 0) p;";
            }
        }
        private string GetFeedQuery
        {
            get
            {
                return @"SELECT 
						p.""Id"", 
						p.""Title"", 
						p.""Body"",
						p.""HashId"", 
						p.""UserId"",
						p.""ThumbnailUrl"", 
						u.""Avatar"" AS UserAvatar,
						u.""ProfileName"", 
						u.""ProfileId"",
						p.""Status"", 
						p.""Type"", 
						p.""CustomNote"",
						array_agg(tag.""Name"") as Tags,
						p.""CreatedOn"",
						sp.""Id"", 
						sp.""HashId"",
						sp.""Title"",
						sp.""Name"",
						sp.""Status"",sp.""CreatedOn"",
						sp.""Permission"",sp.""PublishDate"",sp.""Order"",
						spr.""Id"", 
						spr.""Type"",
						spr.""Url"",
						spr.""ShareUrl"",
						spr.""HashId"",
						spr.""Name"",
						spr.""Width"",
						spr.""Height"",
						spr.""Order"",
						md.""Id"", 
						md.""Title"",
						md.""Description"",
						md.""Url"",
						md.""Domain"",
						pl.""Id"",
						pl.""HashId"",
						pl.""Url"",
						pl.""Type""
						FROM social.""Posts"" p
						LEFT JOIN identity.""Users"" u ON p.""UserId"" = u.""Id""
						LEFT JOIN social.""TagPosts"" tp ON tp.""PostId"" = p.""Id""
						LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id""
						LEFT JOIN ""MetaDatas"" md ON md.""PostId"" = p.""Id""
						LEFT JOIN social.""SubPosts"" sp ON sp.""PostId"" = p.""Id"" AND sp.""IsDelete"" = false 
						LEFT JOIN social.""Resources"" spr ON spr.""SubPostId"" = sp.""Id"" AND spr.""IsDelete"" = false	
						LEFT JOIN ""PostLinks"" pl ON pl.""PostId"" = p.""Id"" AND pl.""IsDelete"" = false 
						WHERE 
						p.""HashId"" = @HashId AND p.""IsDelete"" = false 
						-- TODO AND (@IsAccessPrivate = true OR p.""IsPrivate"" = false )
						GROUP BY p.""Id"",p.""Title"", p.""Body"", p.""HashId"", 
						p.""UserId"",u.""Avatar"",u.""ProfileName"", u.""ProfileId"", p.""CreatedOn"",
						p.""Status"", p.""Type"", p.""CreatedOn"", p.""CustomNote"",
						sp.""Id"",sp.""HashId"",sp.""Title"", sp.""Status"",sp.""CreatedOn"",
						sp.""Permission"",sp.""PublishDate"",sp.""Order"",
						spr.""Id"",
						spr.""Type"",spr.""HashId"",spr.""ShareUrl"",spr.""Url"",spr.""Name"",md.""Title"",
						md.""Id"",
						md.""Description"",
						md.""Url"",
						md.""Domain"",
						pl.""Id"",
						pl.""HashId"",
						pl.""Url"",
						pl.""Type""
						ORDER BY sp.""Order""";
            }
        }

        private string GetFeedBoxQuery
        {
            get
            {
                return @"
					SELECT 
						p.""Id"", 
						p.""Type"",
						p.""Body"",
						p.""HashId"",
						p.""UserId"",
						p.""ThumbnailUrl"",						
						p.""CustomNote"",
						u.""Avatar"" AS ""UserAvatar"",
						u.""ProfileName"" AS ""FullName"",
						u.""UserName"" AS ""UserName"",
						u.""ProfileId"",
						p.""CreatedOn"",
						sp.""Total"" AS ""TotalResources"",
						to_jsonb(ARRAY_AGG(sp.*)) AS social.""SubPosts"",
						to_jsonb(ARRAY_AGG(spr.*)) AS social.""Resources"",
						jsonb_build_object(
							'Description', md.""Description"",
							'Title', md.""Title"",
							'Url', md.""Url"",
							'Domain', md.""Domain""
						) AS ""MetaDatas"",
						CASE 
							WHEN pl.""Url"" IS NULL AND pl.""Type"" IS NULL AND pl.""HashId"" IS NULL THEN NULL
							ELSE jsonb_build_object(
								'HashId', pl.""HashId"",
								'Url', pl.""Url"",
								'Type', pl.""Type""
							)
						END AS ""Link""
					FROM
						social.""Posts"" p
					LEFT JOIN 
						identity.""Users"" u ON p.""UserId"" = u.""Id""
					LEFT JOIN 
						""MetaDatas"" md ON md.""PostId"" = p.""Id""
					LEFT JOIN 
						""PostLinks"" pl ON pl.""PostId"" = p.""Id"" AND pl.""IsDelete"" = FALSE
					LEFT JOIN LATERAL (
						SELECT
							sp.""Id"",
							sp.""HashId"",
							sp.""PostId"",
							sp.""Order"",
							COUNT(*) OVER() AS ""Total""
						FROM
							social.""SubPosts"" sp
						WHERE
							sp.""PostId"" = p.""Id""
							AND sp.""IsDelete"" = FALSE
						GROUP BY
							sp.""Id"",
							sp.""PostId""
						ORDER BY
							sp.""Order""
					) sp ON sp.""PostId"" = p.""Id""
					LEFT JOIN LATERAL (
						SELECT
							spr.""SubPostId"",
							spr.""Type"",
							spr.""Status"",
							spr.""ShareUrl"",
							spr.""Url"",
							spr.""Name"",
							spr.""HashId"",
							spr.""Width"",
							spr.""Height"",
							spr.""Order"",
							sp.""HashId"" as SubPostHashId
						FROM
							social.""Resources"" spr
						WHERE
							spr.""SubPostId"" = sp.""Id""
							AND spr.""IsDelete"" = FALSE
							LIMIT 1
					) spr ON spr.""SubPostId"" = sp.""Id""
					WHERE
						p.""HashId"" = ANY(@HashIds)
						AND p.""IsDelete"" = FALSE
					GROUP BY
						p.""Id"",
						p.""Type"",
						p.""Body"",
						p.""HashId"",
						p.""UserId"",
						p.""ThumbnailUrl"",
						p.""CustomNote"",
						u.""Avatar"",
						u.""ProfileName"",
						u.""UserName"",
						u.""ProfileId"",
						p.""CreatedOn"",
						""TotalResources"",
						md.""Description"",
						md.""Title"",
						md.""Url"",
						md.""Domain"",
						pl.""HashId"",
						pl.""Url"",
						pl.""Type"";
				";
            }
        }

        private string GetAllFeedByKeyword
        {
            get
            {
                return @"SELECT post.""Id"",post.""Title"", post.""Body"", post.""HashId"",
						post.""Avatar"" AS UserAvatar,post.""UserId"", post.""ProfileName"",post.""ProfileId"", post.""ThumbnailUrl"", 
						post.""Status"", post.""Type"",
						post.""CreatedOn"",
						post.""TotalResource"",
						post.""SubPostStr"",
						post.""SubPostResourceStr"",
						post.""MetaTitle"",
						post.""MetaDescription"",
						post.""MetaUrl"",
						post.""MetaDomain"",
						post.""CustomNote"", array_agg(tag.""Name"") as Tags from
							(SELECT  p.""Id"",
							p.""Title"", p.""Body"",  
							p.""HashId"",p.""UserId"",u.""Avatar"", u.""ProfileName"",u.""ProfileId"", p.""ThumbnailUrl"", 
							p.""Status"", p.""Type"", 
							p.""CreatedOn"",
							p.""CustomNote"", --sp.""Id"" as ""SPID"",
							sp.""Total"" AS ""TotalResource"",
							to_jsonb(array_agg(sp.*)) AS ""SubPostStr"",
							to_jsonb(array_agg(spr.*)) AS ""SubPostResourceStr"",
							md.""Title"" AS ""MetaTitle"",
							md.""Description"" AS ""MetaDescription"",
							md.""Url"" AS ""MetaUrl"",
							md.""Domain"" AS ""MetaDomain""
							FROM (
								SELECT DISTINCT subqpost.* FROM 
									(
										SELECT qpost.* FROM social.""Posts"" qpost
								 		INNER JOIN social.""TagPosts"" qtp ON qtp.""PostId"" = qpost.""Id""
										INNER JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
										WHERE  qtag.""Name"" ILIKE '%{2}%' AND qpost.""Type"" = @PostType
										AND qpost.""IsDelete"" = false 
										--TODO AND (@IsAccessPrivate = true OR qpost.""IsPrivate"" = false )
										UNION
										SELECT qpost.* FROM social.""Posts"" qpost
										INNER JOIN identity.""Users"" users ON qpost.""UserId"" = users.""Id"" 
										WHERE users.""ProfileName"" ILIKE '%{2}%'
										AND users.""IsDelete"" = false 
										--TODO AND (@IsAccessPrivate = true OR qpost.""IsPrivate"" = false )
									)subqpost 
								ORDER BY subqpost.""{1}"" DESC
								LIMIT @PageSize
								OFFSET @Offet
							) p
							LEFT JOIN identity.""Users"" u ON p.""UserId"" = u.""Id""						
							LEFT JOIN ""MetaDatas"" md ON md.""PostId"" = p.""Id""
							LEFT JOIN LATERAL 
							(
								SELECT ""Id"",""HashId"",""PostId"",""Order"", count(*) OVER() AS ""Total"" FROM social.""SubPosts"" sp 
								WHERE ""PostId"" = p.""Id""
								GROUP BY ""Id"", ""PostId""
								ORDER BY ""Order""
								LIMIT 5
							) sp ON sp.""PostId"" = p.""Id""
							LEFT JOIN LATERAL
							(
								SELECT ""SubPostId"",""Type"",""ShareUrl"",""Status"",""Url"",""Name"",""HashId"",""Width"",""Height"",sp.""Order""
							 	FROM social.""Resources"" 
							 	WHERE ""SubPostId"" = sp.""Id""
								LIMIT 1
							) spr ON spr.""SubPostId"" = sp.""Id""
							
							GROUP BY p.""Id"",p.""Title"", p.""Body"", p.""HashId"", p.""UserId"", 
							u.""Avatar"", u.""ProfileName"", u.""ProfileId"", p.""ThumbnailUrl"", 
							p.""Status"", p.""Type"",
							p.""CreatedOn"",
							p.""CustomNote"",
							sp.""Total"",
							md.""Title"",
							md.""Description"",
							md.""Url"",
							md.""Domain""
							) 
						AS post
						LEFT JOIN social.""TagPosts"" tp ON tp.""PostId"" = post.""Id""
						LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id"" 
						GROUP BY post.""Id"",post.""Title"", post.""Body"", post.""HashId"", 
						post.""UserId"",post.""Avatar"",post.""ProfileName"",post.""ProfileId"", post.""ThumbnailUrl"", 
						post.""Status"", post.""Type"", 
						post.""CreatedOn"",
						post.""TotalResource"",
						post.""SubPostStr"",
						post.""SubPostResourceStr"",
						post.""MetaTitle"",
						post.""MetaDescription"",
						post.""MetaUrl"",
						post.""MetaDomain"",
						post.""CustomNote""
						ORDER BY post.""{1}"" DESC;

						SELECT COUNT(*) AS TotalItems 
						FROM (
								SELECT DISTINCT subqpost.* FROM 
									(
										SELECT qpost.* FROM social.""Posts"" qpost
								 		INNER JOIN social.""TagPosts"" qtp ON qtp.""PostId"" = qpost.""Id""
										INNER JOIN ""Tags"" qtag ON qtp.""TagId"" = qtag.""Id"" 
										WHERE  qtag.""Name"" ILIKE '%{2}%' AND qpost.""Type"" = @PostType
										AND qpost.""IsDelete"" = false 
										--TODO AND (@IsAccessPrivate = true OR qpost.""IsPrivate"" = false )
										UNION
										SELECT qpost.* FROM social.""Posts"" qpost
										INNER JOIN identity.""Users"" users ON qpost.""UserId"" = users.""Id"" 
										WHERE users.""ProfileName"" ILIKE '%{2}%'
										AND users.""IsDelete"" = false 
										--TODO AND (@IsAccessPrivate = true OR qpost.""IsPrivate"" = false )
									)subqpost 
							) p;";
            }
        }

        private string CheckUserFirstFeed
        {
            get
            {
                return @"SELECT ""Id"" FROM social.""Posts"" WHERE ""UserId"" = @UserId AND ""Type"" = @PostType LIMIT 1;";
            }
        }
    }
}
