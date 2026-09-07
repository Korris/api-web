namespace Mcsg.Api.Areas.Document.Models;

using Common.Core.Enums;
using Mcsg.Api.Areas.Document.Dtos;

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
    public string BucketName { get; set; }
    public ResourceType ResourceType { get; set; }
    public string PrevSubPostHashId { get; set; }
    public string NextSubPostHashId { get; set; }
}
