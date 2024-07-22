namespace Mcsg.Realtime.Api.Services
{
    public partial class SocialReplyService
    {
        private string DeleteReplyCommand
        {
            get
            {
                return @$"UPDATE {{0}}
                                SET ""IsDelete"" = true,
                                    ""LastModifiedBy"" = @LastModifiedBy,
                                    ""LastModifiedDate"" = @LastModifiedDate
                                WHERE ""Id"" = @Id ;
                            
                            UPDATE {{1}}
                                SET ""IsDelete"" = true,
                                    ""LastModifiedBy"" = @LastModifiedBy,
                                    ""LastModifiedDate"" = @LastModifiedDate
                                WHERE ""Id"" = (SELECT ""ResourceId"" FROM {{0}} WHERE ""Id"" = @Id) ;

                            UPDATE {{2}}
                                SET ""IsDelete"" = true,
                                    ""LastModifiedBy"" = @LastModifiedBy,
                                    ""LastModifiedDate"" = @LastModifiedDate
                                WHERE ""LocationId"" = @Id AND ""LocationType"" = @LocationType; ";
            }
        }
    }
}
