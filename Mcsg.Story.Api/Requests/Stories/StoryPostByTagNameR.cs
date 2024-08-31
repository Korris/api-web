namespace Mcsg.Story.Api.Requests;

using Common.Core.Requests;

public class StoryPostByTagNameR : PaginatedR
{
    public string? TagName { get; set; }
}
