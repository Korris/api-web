namespace Mcsg.Api.Areas.Story.Requests;

using Common.Core.Requests;

public class StoryPostListSeriesR : PaginatedR
{
    public string? HashTag { get; set; }
}
