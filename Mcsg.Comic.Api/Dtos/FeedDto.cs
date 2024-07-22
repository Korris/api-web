namespace Mcsg.Comic.Api.Dtos;

using Common.Core.Enums;
using Common.Domain.Entities;

public class FeedDto : PostDto
{
    public FeedDto()
    {
        Type = PostType.Feed;
    }

    public string UserName { get; set; }
    public string FullName { get; set; }
    public MetaDataDto MetaData { get; set; }
    public PostLinkDto Link { get; set; }
    public int TotalResource { get; set; }
    public List<ResourceDto> Resources { get; set; } = [];
    public BackgroundMedia.SearchDto? BackgroundSound { get; set; }
}
