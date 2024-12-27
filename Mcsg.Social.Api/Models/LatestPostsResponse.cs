namespace Mcsg.Social.Api.Models;

using Common.Core.Enums;

public class LatestPostsResponse
{
    public string HashId { get; set; }
    public PostType Type { get; set; }
    public Guid? PostId { get; set; }
    public float Point { get; set; }
}

public class ListIdForHomePage
{
    public int TotalItems { get; set; }
    public List<LatestPostsResponse> LatestPostsResponses { get; set; }
}