namespace Mcsg.Api.Areas.TapShow.Requests;

using Common.Core.Requests;

/// <summary>
/// Paged list request for TapShow posts
/// </summary>
public class TapShowPostListR : PaginatedR
{
    /// <summary>
    /// Optional: only posts created by this profile name
    /// </summary>
    public string? ProfileName { get; set; }

    /// <summary>
    /// Optional: search in title
    /// </summary>
    public string? Keyword { get; set; }
}
