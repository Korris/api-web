namespace Mcsg.Story.Api.Models;

using Common.Core.Enums;
using Dtos;
using Lib.Data.Domain.Entities;

public class SubPostResponseItem
{
    public string HashId { get; set; }
    public IEnumerable<StoryResource> Resources { get; set; }
}

public class SubPostFeedQuery : SubPostFeedResponse
{
    public string ResourcesStr { get; set; }
}

public class SubPostFeedResponse : FeedDto
{
    public string HashIdPost { get; set; }
    public string Url { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string ResourceName { get; set; }
    public string ShareUrl { get; set; }
    public ResourceType ResourceType { get; set; }
    public string PrevSubPostHashId { get; set; }
    public string NextSubPostHashId { get; set; }
}
