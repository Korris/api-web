namespace Mcsg.Api.Areas.Social.Models;

using Common.Core.Enums;

/// <summary>
/// One home-page item: the ranking info from Analytic plus the hydrated box model of its area
/// </summary>
public class LatestPostDetailItem
{
    public string HashId { get; set; } = "";

    public PostType Type { get; set; }

    public Guid? PostId { get; set; }

    public float Point { get; set; }

    /// <summary>
    /// Story/Comic/Document: that area's PostBoxResponse. Feed: Social FeedBoxResponse.
    /// Null when the post was not found or its area failed to hydrate.
    /// </summary>
    public object? Data { get; set; }
}

/// <summary>
/// Response of latest-posts-by-type/detail: ids already hydrated, ordered as Analytic ranked them
/// </summary>
public class LatestPostsDetailResponse
{
    public int TotalItems { get; set; }

    public List<LatestPostDetailItem> Items { get; set; } = new();
}
