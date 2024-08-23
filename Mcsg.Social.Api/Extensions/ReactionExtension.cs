namespace Mcsg.Social.Api.Extensions
{
    public static class ReactionExtension
    {
        public static string GetReactionByTargetIdsQuery = @"SELECT ""Type"", ""TargetId"", SUM(""Count"") AS ""Count"", SUM(""ReactByCurrent"") AS ""ReactByCurrent""
                                                        FROM (
                                                            SELECT ""Type"", ""TargetId"", COUNT(*) AS ""Count"", CASE
                                                                WHEN ""AuthorId"" = @UserId THEN 1
                                                                ELSE 0
                                                            END AS ""ReactByCurrent""
                                                            FROM {0}
                                                            WHERE ""TargetId"" = ANY(@TargetIds)
                                                            AND ""IsDelete"" = false
                                                            GROUP BY ""Type"", ""TargetId"", ""AuthorId""
                                                        ) react	
                                                        GROUP BY ""Type"", ""TargetId""
                                                        ORDER BY ""Count"" DESC;";
    }
}
