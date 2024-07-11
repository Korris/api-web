namespace Mcsg.Social.Api.Dtos;

using Common.Core.Enums;

public class FeedQueryDbDto : FeedDto
{
    public string ProfileName { get; set; }

    public List<SubPostQueryDbDto> SubPostDbs { get; set; }

    public MetaDataQueryDto MetaDataDb { get; set; }
    public PostLinkDbDto LinkDb { get; set; }
    public ResourceType ResourceType { get; set; }
    public string ResourceUrl { get; set; }
    public string ResourceName { get; set; }
    public ResourceStatus ResourceStatus { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public string MetaUrl { get; set; }
    public string MetaDomain { get; set; }
}

