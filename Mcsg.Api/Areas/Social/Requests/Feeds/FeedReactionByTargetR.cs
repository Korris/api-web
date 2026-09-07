namespace Mcsg.Api.Areas.Social.Requests;

using Common.Core.Requests;

public class FeedReactionByTargetR : PaginatedR
{
    public string? Type { get; set; }
}
