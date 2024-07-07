namespace Mcsg.Social.Api.Requests;

using Lib.Common.Models;

public class FeedReactionByTargetR : PaginatedR
{
    public string? Type { get; set; }
}
