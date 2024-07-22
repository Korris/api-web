namespace Mcsg.Realtime.Api.Services
{
    public partial class StoryReplyService
    {
        private string DeleteReplyCommand
        {
            get
            {
                return @$"UPDATE {{0}}
                                SET ""IsDelete"" = true,
                                    ""ModifiedBy"" = @ModifiedBy,
                                    ""ModifiedDate"" = @ModifiedDate
                                WHERE ""Id"" = @Id ;
                            
                            UPDATE {{1}}
                                SET ""IsDelete"" = true,
                                    ""ModifiedBy"" = @ModifiedBy,
                                    ""ModifiedDate"" = @ModifiedDate
                                WHERE ""Id"" = (SELECT ""ResourceId"" FROM {{0}} WHERE ""Id"" = @Id) ;

                            UPDATE {{2}}
                                SET ""IsDelete"" = true,
                                    ""ModifiedBy"" = @ModifiedBy,
                                    ""ModifiedDate"" = @ModifiedDate
                                WHERE ""LocationId"" = @Id AND ""LocationType"" = @LocationType; ";
            }
        }
    }
}
