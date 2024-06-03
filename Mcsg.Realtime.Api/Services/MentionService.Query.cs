namespace Mcsg.Realtime.Api.Services
{
    public partial class MentionService
    {
        private string DeleteMentionCommentCommand
        {
            get
            {
                return @$"UPDATE {_mentionRepository.TableName}
                                SET ""IsDelete"" = true,
                                    ""LastModifiedBy"" = @LastModifiedBy,
                                    ""LastModifiedDate"" = @LastModifiedDate
                                WHERE ""LocationId"" = @LocationId  ";
            }
        }
    }
}
