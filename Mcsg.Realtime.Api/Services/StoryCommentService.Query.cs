namespace Mcsg.Realtime.Api.Services
{
    public partial class StoryCommentService
    {
        private string DeleteCommentCommand
        {
            get
            {
                return @$"UPDATE {{0}}
                                SET ""IsDelete"" = true,
                                    ""ModifiedBy"" = @ModifiedBy,
                                    ""ModifiedDate"" = @ModifiedDate
                                WHERE ""Id"" = @Id ;

                            UPDATE {{0}}
                                SET ""IsDelete"" = true,
                                    ""ModifiedBy"" = @ModifiedBy,
                                    ""ModifiedDate"" = @ModifiedDate
                                WHERE ""ParentId"" = @Id ;
                            
                            UPDATE {{1}}
                                SET ""IsDelete"" = true,
                                    ""ModifiedBy"" = @ModifiedBy,
                                    ""ModifiedDate"" = @ModifiedDate
                                WHERE ""Id"" = (SELECT ""ResourceId"" FROM {{0}} WHERE ""Id"" = @Id) ;

                            UPDATE {{1}}
                                SET ""IsDelete"" = true,
                                    ""ModifiedBy"" = @ModifiedBy,
                                    ""ModifiedDate"" = @ModifiedDate
                                WHERE ""Id"" IN (SELECT ""ResourceId"" FROM {{0}} WHERE ""ParentId"" = @Id) ;

                            UPDATE {{2}}
                                SET ""IsDelete"" = true,
                                    ""ModifiedBy"" = @ModifiedBy,
                                    ""ModifiedDate"" = @ModifiedDate
                                WHERE ""LocationId"" = @Id AND ""LocationType"" = @LocationType;";
            }
        }
    }
}
