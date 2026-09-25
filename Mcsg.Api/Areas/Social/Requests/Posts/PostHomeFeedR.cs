namespace Mcsg.Api.Areas.Social.Requests;

using Common.Core.Requests;

/// <summary>
/// Home page tabs
/// </summary>
public enum HomeFeedTab
{
    /// <summary>
    /// Personalized ranking from Analytic (same source as latest-posts-by-type)
    /// </summary>
    ForYou,

    /// <summary>
    /// Newest posts of the users the caller follows
    /// </summary>
    Following,

    /// <summary>
    /// Newest public posts
    /// </summary>
    New,

    /// <summary>
    /// Most engaged posts of the last days (Analytic, not personalized)
    /// </summary>
    Trending
}

/// <summary>
/// Home page feed across Feed / Story / Comic / Document / TapShow, filtered by tab
/// </summary>
public class PostHomeFeedR : PaginatedR
{
    public HomeFeedTab Type { get; set; } = HomeFeedTab.ForYou;
}
