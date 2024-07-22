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
                return @$"SELECT ""Id"" FROM ""Posts""
                      WHERE ""UserId"" = @UserId AND ""IsDelete"" = false";
            }
        }

        private string ExecSoftDeletePost
        {
            get
            {
                return @"UPDATE ""Posts""
					SET ""IsDelete"" = true	, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					WHERE ""Id"" = @PostId;

					UPDATE ""Resources""
					SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM ""SubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE ""Resources"".""SubPostId"" = sp.""Id"";
	
					UPDATE ""SubPostReactions"" spr
					SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM ""SubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE spr.""TargetId"" = sp.""Id"";
	
					UPDATE ""SubPostComments"" spr
					SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM ""SubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE spr.""PostId"" = sp.""Id"";
	
					UPDATE ""SubPostCommentReactions"" spr
					SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM ""SubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE spr.""TargetId"" = sp.""Id"";
	
					UPDATE ""TagPosts""
					SET ""IsDelete"" = true	, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					WHERE ""PostId"" = @PostId;
	
					UPDATE ""SubPosts""
					SET ""IsDelete"" = true	, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					WHERE ""PostId"" = @PostId;
	
					UPDATE ""PostReactions""
					SET ""IsDelete"" = true	, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					WHERE ""TargetId"" = @PostId;
	
					UPDATE ""PostComments""
					SET ""IsDelete"" = true	, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					WHERE ""PostId"" = @PostId;";
            }
        }
    }
}
