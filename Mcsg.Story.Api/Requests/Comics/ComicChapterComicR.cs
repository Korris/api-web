namespace Mcsg.Story.Api.Requests;

using Dtos;

public class ComicChapterComicR : StoryChapterPostR
{
    public List<ResourcePostDto> Files { get; set; }
}
