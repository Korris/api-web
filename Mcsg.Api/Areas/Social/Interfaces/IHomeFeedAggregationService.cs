namespace Mcsg.Api.Areas.Social.Interfaces;

using Mcsg.Api.Areas.Social.Models;

/// <summary>
/// Builds the home page list in one call: ranked ids from Analytic + hydrated details from every content area
/// </summary>
public interface IHomeFeedAggregationService
{
    Task<LatestPostsDetailResponse> GetLatestPostsWithDetail(HttpContext hc);
}
