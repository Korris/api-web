using Mcsg.Lib.Data.Enums;

namespace Mcsg.Social.Api.Services
{
    public partial class SmartLookupService
    {
        private string GetTagNamesByPostIdQuery
        {
            get
            {
                return @$"SELECT t.""Name"" FROM {_tagPostRepository.TableName} tp
                          INNER JOIN {_tagRepository.TableName} t ON tp.""TagId"" = t.""Id""
                          WHERE tp.""PostId"" = @postId";
            }
        }
        private string GetRecentSearchQuery
        {
            get
            {
                return @$"SELECT ""Id"",""Keyword"", 
                             CASE 
                               WHEN ""KeywordType"" = {LookupKeywordType.Tag.GetHashCode()} THEN '{nameof(LookupKeywordType.Tag)}'
                               WHEN ""KeywordType"" = {LookupKeywordType.People.GetHashCode()} THEN '{nameof(LookupKeywordType.People)}'
                               WHEN ""KeywordType"" = {LookupKeywordType.None.GetHashCode()} THEN ''
                             END AS ""KeywordType""
                        FROM {_smartLookupUserRepository.TableName} 
                        WHERE ""UserId"" = '{_currentUserService.Session.UserId}'
                        ORDER BY ""CreatedDate"" DESC
                        LIMIT 6";
            }
        }

        private string GetSmartLookupQuery
        {
            get
            {
                return @$"SELECT sm.""Keyword"",
                             CASE 
                               WHEN sm.""KeywordType"" = {LookupKeywordType.Tag.GetHashCode()} THEN '{nameof(LookupKeywordType.Tag)}'
                               WHEN sm.""KeywordType"" = {LookupKeywordType.People.GetHashCode()} THEN '{nameof(LookupKeywordType.People)}'
                               WHEN sm.""KeywordType"" = {LookupKeywordType.None.GetHashCode()} THEN '{nameof(LookupKeywordType.None)}'
                             END AS ""KeywordType""
                            , us.""Avatar""
                         FROM {_smartLookupRepository.TableName} sm
                          LEFT JOIN {_userRepository.TableName} us ON sm.""Keyword"" = us.""ProfileName"" AND sm.""KeywordType"" = {LookupKeywordType.People.GetHashCode()}
                        {{0}}
                        ORDER BY sm.""CountCriteria"" DESC
                        LIMIT 6";
            }
        }

        private string GetSmartLookupUserByKeywordAndTypeQuery
        {
            get
            {
                return @$"SELECT ""Id"" FROM {_smartLookupUserRepository.TableName}
                          WHERE ""Keyword"" = @keyword AND ""KeywordType"" = @type AND ""UserId"" = @userid";
            }
        }
    }
}
