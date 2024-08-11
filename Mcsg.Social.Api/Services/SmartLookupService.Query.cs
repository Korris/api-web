namespace Mcsg.Social.Api.Services
{
    using Common.Core.Enums;

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

        private string GetSmartLookupQuery
        {
            get
            {
                return @$"SELECT sm.""Keyword"",
                             CASE 
                               WHEN sm.""KeywordType"" = {LookupKeywordType.Tag.GetHashCode()} THEN '{nameof(LookupKeywordType.Tag)}'
                               WHEN sm.""KeywordType"" = {LookupKeywordType.People.GetHashCode()} THEN '{nameof(LookupKeywordType.People)}'                               
                               WHEN sm.""KeywordType"" = {LookupKeywordType.Comic.GetHashCode()} THEN '{nameof(LookupKeywordType.Comic)}'                               
                               WHEN sm.""KeywordType"" = {LookupKeywordType.Story.GetHashCode()} THEN '{nameof(LookupKeywordType.Story)}'
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
                          WHERE ""Keyword"" = @keyword AND ""KeywordType"" = @type AND ""UserId"" = @userid AND ""EntityId"" = @entityId";
            }
        }
    }
}
