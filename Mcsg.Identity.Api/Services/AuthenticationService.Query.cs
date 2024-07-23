namespace Mcsg.Identity.Api.Services
{
    public partial class AuthenticationService
    {
        private string GetUserByPhoneQuery
        {
            get
            {
                return @$"SELECT ""Id"", ""PhoneNumber"" FROM {_userRepository.TableName}
                      WHERE ""PhoneNumber"" = @Phone ";
            }
        }
        private string GeAllPostsIdByUser
        {
            get
            {
                return @$"SELECT ""Id"" FROM social.""Posts""
                      WHERE ""UserId"" = @UserId AND ""IsDelete"" = false";
            }
        }

        private string ExecSoftDeletePost
        {
            get
            {
                return @"UPDATE social.""Posts""
					SET ""IsDelete"" = true	, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					WHERE ""Id"" = @PostId;

					UPDATE social.""Resources""
					SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM social.""SubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE social.""Resources"".""SubPostId"" = sp.""Id"";
	
					UPDATE social.""SubPostReactions"" spr
					SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM social.""SubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE spr.""TargetId"" = sp.""Id"";
	
					UPDATE social.""SubPostComments"" spr
					SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM social.""SubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE spr.""PostId"" = sp.""Id"";
	
					UPDATE social.""SubPostCommentReactions"" spr
					SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM social.""SubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE spr.""TargetId"" = sp.""Id"";
	
					UPDATE social.""TagPosts""
					SET ""IsDelete"" = true	, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					WHERE ""PostId"" = @PostId;
	
					UPDATE social.""SubPosts""
					SET ""IsDelete"" = true	, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					WHERE ""PostId"" = @PostId;
	
					UPDATE social.""PostReactions""
					SET ""IsDelete"" = true	, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					WHERE ""TargetId"" = @PostId;
	
					UPDATE social.""PostComments""
					SET ""IsDelete"" = true	, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					WHERE ""PostId"" = @PostId;";
            }
        }
    }
}
