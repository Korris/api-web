namespace Mcsg.Social.Api.Services
{
    public partial class ReactService<T>
    {
        private string GetReactTypeAndUsersQuery
        {
            get
            {
                return @"SELECT ""Id"", ""ParentId"", 
				""TargetId"", ""AuthorId"", ""Type"",
				""CreatedDate"", ""CreatedBy"",
				""ModifiedDate"", ""ModifiedBy"", ""IsDelete""
					FROM {0}
					WHERE ""TargetId"" = @TargetId
					AND ( ""AuthorId"" IS NULL OR ""AuthorId"" = @AuthorId)
					AND ""Type"" = @Type";
            }
        }
        private string GetReactByUsersQuery
        {
            get
            {
                return @"SELECT ""Id"", ""ParentId"", 
				""TargetId"", ""AuthorId"", ""Type"",
				""CreatedDate"", ""CreatedBy"",
				""ModifiedDate"", ""ModifiedBy"", ""IsDelete""
					FROM {0}
					WHERE ""TargetId"" = @TargetId
					AND ""AuthorId"" = @AuthorId";
            }
        }
        private string GetReactByTargetQuery
        {
            get
            {
                return @"SELECT ""Type"",SUM(""Count"") AS ""Count"", SUM(""ReactByCurrent"")  AS ""ReactByCurrent"" FROM
                    (SELECT ""Type"", COUNT(*) AS ""Count"", CASE
                      WHEN ""AuthorId"" = @UserId THEN 1
                      ELSE 0
                     END AS ""ReactByCurrent""
					FROM {0}
					
					WHERE ""TargetId"" = @TargetId
					AND ""IsDelete"" = false
                    GROUP BY ""Type"", ""AuthorId"") react					
					GROUP BY ""Type"" 
                    ORDER BY ""Count"" DESC";
            }
        }
        private string GetReactionByTargetQuery
        {
            get
            {
                return @"SELECT r.""Type"", r.""AuthorId""
                            , (CASE WHEN u.""ProfileName"" IS NULL THEN u.""UserName""  ELSE u.""ProfileName"" END) AS ""AuthorName""
                            , u.""Avatar"" AS ""AuthorAvatar""
                            FROM {0} r
                            LEFT JOIN identity.""Users"" u ON r.""AuthorId"" = u.""Id""
                            WHERE r.""TargetId"" = @TargetId AND r.""IsDelete"" = false
                            AND r.""Type"" = (CASE WHEN @Type IS NULL THEN r.""Type"" ELSE @Type END)
                            ORDER BY r.""ModifiedDate"" DESC 
                            LIMIT @PageSize
						    OFFSET @Offet ;

                            SELECT COUNT(""Id"") FROM {0} r
                            WHERE r.""TargetId"" = @TargetId AND r.""IsDelete"" = false
                            AND r.""Type"" = (CASE WHEN @Type IS NULL THEN r.""Type"" ELSE @Type END) ; ";
            }
        }
    }
}
