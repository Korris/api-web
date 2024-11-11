namespace Mcsg.Realtime.Api.Services;

public partial class MentionService
{
    private string DeleteMentionCommentCommand
    {
        get
        {
            return @$"UPDATE {_mentionRepository.TableName}
                        SET ""IsDelete"" = true,
                            ""ModifiedBy"" = @ModifiedBy,
                            ""ModifiedOn"" = @ModifiedOn
                        WHERE ""LocationId"" = @LocationId";
        }
    }
}
