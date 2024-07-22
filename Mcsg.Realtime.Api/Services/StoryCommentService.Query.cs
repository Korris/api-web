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
                                    ""ModifiedOn"" = @ModifiedOn
                                WHERE ""Id"" = @Id ;

                            UPDATE {{0}}
                                SET ""IsDelete"" = true,
                                    ""ModifiedBy"" = @ModifiedBy,
                                    ""ModifiedOn"" = @ModifiedOn
                                WHERE ""ParentId"" = @Id ;
                            
                            UPDATE {{1}}
                                SET ""IsDelete"" = true,
                                    ""ModifiedBy"" = @ModifiedBy,
                                    ""ModifiedOn"" = @ModifiedOn
                                WHERE ""Id"" = (SELECT ""ResourceId"" FROM {{0}} WHERE ""Id"" = @Id) ;

                            UPDATE {{1}}
                                SET ""IsDelete"" = true,
                                    ""ModifiedBy"" = @ModifiedBy,
                                    ""ModifiedOn"" = @ModifiedOn
                                WHERE ""Id"" IN (SELECT ""ResourceId"" FROM {{0}} WHERE ""ParentId"" = @Id) ;

                            UPDATE {{2}}
                                SET ""IsDelete"" = true,
                                    ""ModifiedBy"" = @ModifiedBy,
                                    ""ModifiedOn"" = @ModifiedOn
                                WHERE ""LocationId"" = @Id AND ""LocationType"" = @LocationType;";
            }
        }
    }
}
