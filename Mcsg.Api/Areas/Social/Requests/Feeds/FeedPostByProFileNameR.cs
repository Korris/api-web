namespace Mcsg.Api.Areas.Social.Requests;

using Common.Core.Requests;

public class FeedPostByProFileNameR : PaginatedR
{
    public string? Keyword { get; set; }
    public string? SearchBy { get; set; }
}
