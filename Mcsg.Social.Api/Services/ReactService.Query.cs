namespace Mcsg.Social.Api.Services;

public partial class ReactService<T>
{
    private string GetReactByUsersQuery
    {
        get
        {
            return @"SELECT ""Id"", ""ParentId"", 
                ""TargetId"", ""AuthorId"", ""Type"",
                ""CreatedOn"", ""CreatedBy"",
                ""ModifiedOn"", ""ModifiedBy"", ""IsDelete""
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
                    (SELECT r.""Type"", COUNT(*) AS ""Count"", CASE
                      WHEN r.""AuthorId"" = @UserId THEN 1
                      ELSE 0
                     END AS ""ReactByCurrent""
                    FROM {0} r
                    INNER JOIN ""identity"".""Users"" u ON r.""AuthorId"" = u.""Id"" AND u.""IsDelete"" = false
                    WHERE ""TargetId"" = @TargetId
                    AND r.""IsDelete"" = false
                    GROUP BY r.""Type"", r.""AuthorId"") react
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
                        , u.""Avatar"" AS ""AuthorAvatar"", u.""UserName""
                        FROM {0} r
                        INNER JOIN identity.""Users"" u ON r.""AuthorId"" = u.""Id"" 
                        WHERE r.""TargetId"" = @TargetId AND r.""IsDelete"" = false AND u.""IsDelete"" = false
                        AND r.""Type"" = (CASE WHEN @Type IS NULL THEN r.""Type"" ELSE @Type END)
                        ORDER BY r.""ModifiedOn"" DESC 
                        LIMIT @PageSize
                        OFFSET @Offet ;

                        SELECT COUNT(""Id"") FROM {0} r
                        WHERE r.""TargetId"" = @TargetId AND r.""IsDelete"" = false
                        AND r.""Type"" = (CASE WHEN @Type IS NULL THEN r.""Type"" ELSE @Type END) ; ";
        }
    }
}
