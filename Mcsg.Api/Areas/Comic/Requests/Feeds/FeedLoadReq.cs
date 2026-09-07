namespace Mcsg.Api.Areas.Comic.Requests;

using Common.Core.Requests;

public class FeedLoadReq : PaginatedR
{
    public string? ProfileName { get; set; }
    public string? UserName { get; set; }
}
