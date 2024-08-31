namespace Mcsg.Story.Api.Requests;

using Common.Domain.Dtos;

public class StoryChapterComicR : StoryChapterPostR
{
    public List<ResourcePostDto> Files { get; set; }
}
