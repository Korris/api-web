namespace Mcsg.Identity.Api.Services
{
    public partial class SessionService
    {
        private string ExpiredUserSessionsQuery
        {
            get
            {
                return @$"UPDATE {_sessionRepository.TableName}
                               SET ""ExpiredDateUtc"" = now() AT TIME ZONE 'UTC' 
                               WHERE ""UserId"" = @userid";
            }
        }

        private static string GetUserRoleQuery
        {
            get
            {
                return @$"SELECT r.""Name"" FROM ""UserRoles"" ur 
                         INNER JOIN ""Roles"" r ON ur.""RoleId"" = r.""Id"" 
                         INNER JOIN ""Users"" u ON u.""Id"" = ur.""UserId""
                         WHERE u.""Id"" = @userId";
            }
        }
    }
}
