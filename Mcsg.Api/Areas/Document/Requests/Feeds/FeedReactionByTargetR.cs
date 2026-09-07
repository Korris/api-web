namespace Mcsg.Api.Areas.Document.Requests;

using Common.Core.Requests;

public class FeedReactionByTargetR : PaginatedR
{
    public string? Type { get; set; }
}
