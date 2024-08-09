namespace Mcsg.Identity.Api.Services
{
    public partial class UserService
    {
        private string UpdateEmailConfirmedCommand
        {
            get
            {
                return @"UPDATE {_userRepository.TableName}
                        SET ""Email"" = @email, 
                            ""EmailConfirmed"" = true
                        WHERE ""Id"" = @id AND ""EmailConfirmed"" = false";
            }
        }

        private string UpdatePhoneNumberConfirmedCommand
        {
            get
            {
                return @$"UPDATE {_userRepository.TableName}
                        SET ""PhoneNumber"" = @phone, 
                            ""PhoneNumberConfirmed"" = true
                        WHERE ""Id"" = @id AND ""PhoneNumberConfirmed"" = false";
            }
        }

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
