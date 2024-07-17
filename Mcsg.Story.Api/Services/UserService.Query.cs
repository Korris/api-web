namespace Mcsg.Story.Api.Services
{
    public partial class UserService
    {
        private string GetSimilarProfileName
        {
            get
            {
                return @$"SELECT ""Id"", ""ProfileName"",""ProfileId"" FROM {_userRepository.TableName}
                     WHERE ""ProfileName"" LIKE @Name AND ""IsDelete"" = false LIMIT 10";
            }
        }

        private string GetSimilarProfileNamesMention
        {
            get
            {
                return $@"SELECT ""ProfileName"", ""Avatar"", ""UserName"", ""Id"" FROM {_userRepository.TableName}
                      WHERE ""ProfileName"" ILIKE @Name AND ""IsDelete"" = false LIMIT 4";
            }
        }

        private string GetRandomProfileNames
        {
            get
            {
                return $@"SELECT ""ProfileName"", ""Avatar"", ""UserName"", ""Id"" FROM {_userRepository.TableName}
                      WHERE ""IsDelete"" = false ORDER BY RANDOM() LIMIT 4";
            }
        }

        private string GetUserAvatarById
        {
            get
            {
                return @$"SELECT ""Id"",
""ProfileName"",
""Avatar"",
""ProfileId"" FROM {_userRepository.TableName}
                     WHERE ""Id"" = @UserId AND ""IsDelete"" = false";
            }
        }
    }
}
