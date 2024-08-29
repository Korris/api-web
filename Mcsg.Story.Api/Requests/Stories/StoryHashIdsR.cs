namespace Mcsg.Story.Api.Requests;

using Common.Core.Requests;

public class StoryHashIdsR : PaginatedR
{
    public string? HashIds { get; set; }
}
