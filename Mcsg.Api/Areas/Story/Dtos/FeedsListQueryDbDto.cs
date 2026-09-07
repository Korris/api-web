namespace Mcsg.Api.Areas.Story.Dtos;

using Common.Core.Enums;

public class FeedsListQueryDbDto : FeedDto
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
