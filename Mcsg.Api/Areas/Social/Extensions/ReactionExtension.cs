namespace Mcsg.Api.Areas.Social.Extensions;

public static class ReactionExtension
{
    public static string GetReactionByTargetIdsQuery = @"SELECT ""Type"", ""TargetId"", SUM(""Count"") AS ""Count"", SUM(""ReactByCurrent"") AS ""ReactByCurrent""
                                                        FROM (
                                                            SELECT r.""Type"", r.""TargetId"", COUNT(*) AS ""Count"", CASE
                                                                WHEN r.""AuthorId"" = @UserId THEN 1
                                                                ELSE 0
                                                            END AS ""ReactByCurrent""
                                                            FROM {0} r
                                                            INNER JOIN ""identity"".""Users"" u ON r.""AuthorId"" = u.""Id""
                                                            WHERE ""TargetId"" = ANY(@TargetIds)
                                                            AND r.""IsDelete"" = false
                                                            GROUP BY r.""Type"", r.""TargetId"", r.""AuthorId""
                                                        ) react	
                                                        GROUP BY ""Type"", ""TargetId""
                                                        ORDER BY ""Count"" DESC;";
}
