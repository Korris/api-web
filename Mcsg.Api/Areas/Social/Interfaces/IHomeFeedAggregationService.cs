namespace Mcsg.Api.Areas.Social.Interfaces;

using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Areas.Social.Requests;

/// <summary>
/// Builds the home page list in one call: ranked ids from Analytic + hydrated details from every content area
/// </summary>
public interface IHomeFeedAggregationService
{
    Task<LatestPostsDetailResponse> GetLatestPostsWithDetail(HttpContext hc);

    /// <summary>
    /// Home page tab (For you / Following / New / Trending), paged, every item hydrated like get-post-by-list-id
    /// </summary>
    Task<LatestPostsDetailResponse> GetHomeFeedWithDetail(PostHomeFeedR request);
}
