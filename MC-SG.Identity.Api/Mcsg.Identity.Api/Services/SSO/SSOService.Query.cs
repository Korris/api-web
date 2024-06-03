namespace Mcsg.Identity.Api.Services.SSO
{
    public partial class SSOService
    {
        private string GetUserSocialQuery
        {
            get
            {
                return @$"SELECT ""Id"", ""UserId"", ""SocialId"" FROM {_userSocialRepository.TableName}
                      WHERE ""SocialId"" = @SocialId AND ""Type"" = @Type ";
            }
        }
    }
}
