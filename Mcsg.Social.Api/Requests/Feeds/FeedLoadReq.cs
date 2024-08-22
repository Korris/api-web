namespace Mcsg.Social.Api.Requests;

using Common.Core.Requests;

public class FeedLoadReq : PaginatedR
{
    public string? UserName { get; set; }
}
