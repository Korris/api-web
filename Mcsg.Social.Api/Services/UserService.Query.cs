using Mcsg.Lib.Data.Enums;

namespace Mcsg.Social.Api.Services
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

        private string CheckExistProfileName
        {
            get
            {
                return @$"SELECT ""Id"" FROM {_userRepository.TableName}
                     WHERE ""ProfileName"" ilike @Name AND ""IsDelete"" = false";
            }
        }

        private string GetUserProfileByName
        {
            get
            {
                return @$"SELECT ""Id"",
""ProfileName"",
""Avatar"",
""CreatedDate"",
""FirstName"",
""LastName"",
""CoverPhoto"",
""Location"",
""ProfileId"" FROM {_userRepository.TableName}
                     WHERE ""ProfileName"" = @ProfileName AND ""IsDelete"" = false";
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

        private string UpdateSmartLookupProfileName
        {
            get
            {
                return $@"UPDATE {_smartLookupRepository.TableName} 
                          SET ""Keyword"" = @newKeyword
                          WHERE ""Keyword"" = @oldKeyword AND ""KeywordType"" = {LookupKeywordType.People.GetHashCode()}";
            }
        }
    }
}
