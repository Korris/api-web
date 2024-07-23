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
                return @$"SELECT ""Id"" FROM social.""SocialPosts""
                      WHERE ""UserId"" = @UserId AND ""IsDelete"" = false";
            }
        }

        private string ExecSoftDeletePost
        {
            get
            {
                return @"UPDATE social.""SocialPosts""
					SET ""IsDelete"" = true	, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					WHERE ""Id"" = @PostId;

					UPDATE social.""SocialResources""
					SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM social.""SocialSubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE social.""SocialResources"".""SubPostId"" = sp.""Id"";
	
					UPDATE social.""SocialSubPostReactions"" spr
					SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM social.""SocialSubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE spr.""TargetId"" = sp.""Id"";
	
					UPDATE social.""SocialSubPostComments"" spr
					SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM social.""SocialSubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE spr.""PostId"" = sp.""Id"";
	
					UPDATE social.""SocialSubPostCommentReactions"" spr
					SET ""IsDelete"" = true, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					FROM (SELECT ""Id""
						  FROM social.""SocialSubPosts"" WHERE ""PostId"" = @PostId) AS sp
					WHERE spr.""TargetId"" = sp.""Id"";
	
					UPDATE social.""SocialTagPosts""
					SET ""IsDelete"" = true	, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					WHERE ""PostId"" = @PostId;
	
					UPDATE social.""SocialSubPosts""
					SET ""IsDelete"" = true	, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					WHERE ""PostId"" = @PostId;
	
					UPDATE social.""SocialPostReactions""
					SET ""IsDelete"" = true	, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					WHERE ""TargetId"" = @PostId;
	
					UPDATE social.""SocialPostComments""
					SET ""IsDelete"" = true	, ""ModifiedOn"" = @Date, ""ModifiedBy"" = @UserId
					WHERE ""PostId"" = @PostId;";
            }
        }
    }
}
