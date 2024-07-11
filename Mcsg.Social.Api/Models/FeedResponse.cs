namespace Mcsg.Social.Api.Models;

using Common.Core.Enums;
using Dtos;
using Lib.Data.Domain.Entities;

public class FeedResponse : PostResponse
{
    public FeedResponse()
    {
        Type = PostType.Feed;
    }

    public string UserName { get; set; }
    public string FullName { get; set; }
    public MetaDataDto MetaData { get; set; }
    public PostLinkResponse Link { get; set; }
    public int TotalResource { get; set; }
    public List<ResourceResponse> Resources { get; set; } = new List<ResourceResponse>();
    public BackgroundMedia.SearchDto? BackgroundSound { get; set; }
}

public class FeedQueryDbResponse : FeedResponse
{
    public string ProfileName { get; set; }

    public List<SubPostQueryDbResponse> SubPostDbs { get; set; }

    public MetaDataQueryDto MetaDataDb { get; set; }
    public PostLinkDb LinkDb { get; set; }
    public ResourceType ResourceType { get; set; }
    public string ResourceUrl { get; set; }
    public string ResourceName { get; set; }
    public ResourceStatus ResourceStatus { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public string MetaUrl { get; set; }
    public string MetaDomain { get; set; }
}
public class FeedsListQueryDbResponse : FeedResponse
{
    public string UserName { get; set; }
    public string ProfileName { get; set; }
    public ResourceType ResourceType { get; set; }
    public string ResourceUrl { get; set; }
    public string ResourceName { get; set; }
    public ResourceStatus ResourceStatus { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public string MetaUrl { get; set; }
    public string MetaDomain { get; set; }
    public string SubPostStr { get; set; }
    public string SubPostResourceStr { get; set; }
    public string LinkHashId { get; set; }
    public string LinkUrl { get; set; }
    public PostLinkType LinkType { get; set; }
}

public class FeedPostResponse : FeedResponse
{
    public List<RewardRespone> Rewards { get; set; }
}
