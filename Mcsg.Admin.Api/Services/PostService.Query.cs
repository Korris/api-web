namespace Mcsg.Admin.Api.Services
{
    public partial class PostService
    {
        private Dictionary<string, string> AliasAndAmbiguousColumns = new Dictionary<string, string>
        {
            //Alias columns
            { "Remove", "users.\"IsDelete\"" },
            {"Enable", "users.\"Status\"" },
            {"LastLogin", "users.\"LastActionDateUtc\"" },
            //Ambiguous columns
            {"Id", "users.\"Id\"" },
            {"UserName", "users.\"UserName\"" },
            {"Email", "users.\"Email\"" }
        };
        private string GetAllPostsQuery
        {
            get
            {
                return @"
						SELECT p.""Id"", 
 p.""Title"", 
 p.""HashId"", 
 p.""Type"", 
 p.""Body"", 
 p.""Status"", 
array_agg(tag.""Name"") as Tags,
 p.""CreatedOn"", 
 p.""CreatedBy"", 
 p.""ModifiedOn"", 
 p.""ModifiedBy"", 
 p.""IsDelete"", 
 p.""ThumbnailUrl"", 
 p.""AuthorName"", 
 p.""CoverUrl"", 
 p.""IsMature"", 
 p.""ViewCount"", 
 p.""AuthorId"", 
 p.""UserId"", 
u.""ProfileName"", 
						u.""ProfileId"",
 p.""Permission"", 
 p.""IsCompleted"", 
 p.""ExternalCode"", 
 p.""ExternalResource""
	FROM {0} p 
LEFT JOIN identity.""Users"" u ON p.""UserId"" = u.""Id""
LEFT JOIN social.""SocialTagPosts"" tp ON tp.""PostId"" = p.""Id""
LEFT JOIN ""Tags"" tag ON tp.""TagId"" = tag.""Id""
	WHERE [HashId] [TitleSearch] [FromDate] [ToDate] [PostType] 
GROUP BY p.""Id"", 
 p.""Title"", 
 p.""HashId"", 
 p.""Type"", 
 p.""Body"", 
 p.""Status"", 
 p.""CreatedOn"", 
 p.""CreatedBy"", 
 p.""ModifiedOn"", 
 p.""ModifiedBy"", 
 p.""IsDelete"", 
 p.""ThumbnailUrl"", 
 p.""AuthorName"", 
 p.""CoverUrl"", 
 p.""IsMature"", 
 p.""ViewCount"", 
 p.""AuthorId"", 
 p.""UserId"", 
u.""ProfileName"", 
						u.""ProfileId"",
 p.""Permission"", 
 p.""IsCompleted"", 
 p.""ExternalCode"", 
 p.""ExternalResource""

						ORDER BY p.{2} {3}
						LIMIT @PageSize
						OFFSET @Offet;

						SELECT COUNT(*) AS TotalItems FROM {0} p WHERE [HashId] [TitleSearch] [FromDate] [ToDate] [PostType];";
            }
        }
    }
}
