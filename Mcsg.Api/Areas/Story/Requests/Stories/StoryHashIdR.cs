namespace Mcsg.Api.Areas.Story.Requests;

using Common.Core.Requests;

public class StoryHashIdR : PaginatedR
{
    public string? HashId { get; set; }

    public bool IsLoadChapters { get; set; }
}
