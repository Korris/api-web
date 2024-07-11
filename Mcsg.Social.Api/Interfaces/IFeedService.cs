namespace Mcsg.Social.Api.Interfaces;

using Common.SeedWork.Responses;
using Enums;
using Models;
using Requests;

public interface IFeedService
{
    Task<PagedResponse<FeedResponse>> GetFeedsAsync(FeedLoadReq feedLoadReq, LoadFeedType loadFeedType);
    Task<PagedResponse<FeedResponse>> GetFeedsByTagAsync(string tagName, FeedLoadReq feedLoadReq);
    Task<FeedResponse> GetFeedAsync(string hashId);
    Task<PagedResponse<FeedResponse>> GetFeedByKeywordAsync(string tagName, FeedSearchKeywordR feedLoadReq);
    FeedDisplayConfig GetFeedDisplayConfig();
    Task<FeedResponse> PostFeedAsync(PostCreateR req);
    Task<FeedResponse> UpdateFeedAsync(string hashId, PostUpdateR feedPostReq);
    Task<bool> DeleteFeedAsync(Guid postId);

    Task<bool> ReportFeedAsync(FeedReportPostReq req);
    FeedResponse MappingFeedInListRespone(FeedsListQueryDbResponse item);
    Task<SubPostFeedResponse> GetFeedSubPostAsync(string hashId);
    Task<List<FeedBoxResponse>> GetFeedsByIds(string hashIds);
}
