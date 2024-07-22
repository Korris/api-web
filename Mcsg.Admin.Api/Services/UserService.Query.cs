namespace Mcsg.Admin.Api.Services
{
    public partial class UserService
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
        private string GetAllUsersQuery
        {
            get
            {
                return @"SELECT 
						users.""Id"",users.""UserName"", users.""ProfileId"", users.""Email"", users.""PhoneNumber"", users.""ReferralCode"", 
users.""PremiumDate"", users.""Location"",
						users.""Avatar"", roles.""DisplayName"",users.""LastLoginDate"",
                        CASE 
                            WHEN users.""Status"" = 1 THEN 'Active'
                            WHEN users.""Status"" = 2 THEN 'Blocked'
                            WHEN users.""Status"" = 3 THEN 'Banned'
                        END AS ""Status"",
						users.""CreatedDate"", users.""ModifiedDate""
						FROM {0} users
						LEFT JOIN {1}""UserRoles"" userroles on users.""Id"" = userroles.""UserId""
						LEFT JOIN {1}""Roles"" roles on userroles.""RoleId"" = roles.""Id""
                        
						WHERE (@Status IS NULL OR users.""Status"" = @Status) [FromDate] [ToDate] AND ('{4}' ='' OR users.""UserName"" ILIKE '%{4}%' OR users.""Email"" ILIKE '%{4}%' OR users.""PhoneNumber"" ILIKE '%{4}%' OR users.""ProfileId"" ILIKE '%{4}%') 
AND users.""IsDelete"" = false
						ORDER BY {2} {3}
						LIMIT @PageSize
						OFFSET @Offet;

						SELECT COUNT(*) AS TotalItems FROM {0} users WHERE (@Status IS NULL OR users.""Status"" = @Status) [FromDate] [ToDate] AND ('{4}' ='' OR users.""UserName"" ILIKE '%{4}%' OR users.""Email"" ILIKE '%{4}%' OR users.""PhoneNumber"" ILIKE '%{4}%' OR users.""ProfileId"" ILIKE '%{4}%') 
AND ""IsDelete"" = false;";
            }
        }
    }
}
