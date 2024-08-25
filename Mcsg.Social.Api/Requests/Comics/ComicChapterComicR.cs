namespace Mcsg.Social.Api.Requests;

using Common.Domain.Dtos;

public class ComicChapterComicR : StoryChapterPostR
{
    public List<ResourcePostDto> Files { get; set; }
}
