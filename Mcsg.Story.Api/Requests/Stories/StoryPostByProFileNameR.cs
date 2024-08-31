namespace Mcsg.Story.Api.Requests;

using Common.Core.Requests;

public class StoryPostByProFileNameR : PaginatedR
{
    public string? Keyword { get; set; }
    public string? SearchBy { get; set; }
}
