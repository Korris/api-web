namespace Mcsg.Api.Areas.Social.Requests;

using Common.Core.Requests;

public class FeedLoadReq : PaginatedR
{
    public string? NewUserName { get; set; }
}
