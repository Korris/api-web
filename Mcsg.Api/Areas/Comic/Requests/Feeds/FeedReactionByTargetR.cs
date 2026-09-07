namespace Mcsg.Api.Areas.Comic.Requests;

using Common.Core.Requests;

public class FeedReactionByTargetR : PaginatedR
{
    public string? Type { get; set; }
}
