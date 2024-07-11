namespace Mcsg.Social.Api.Interfaces;

using Common.SeedWork.Responses;
using Dtos;
using Enums;
using Models;
using Requests;

public interface IFeedService
{
    Task<PagedResponse<FeedDto>> GetFeedsAsync(FeedLoadReq feedLoadReq, LoadFeedType loadFeedType);
    Task<PagedResponse<FeedDto>> GetFeedsByTagAsync(string tagName, FeedLoadReq feedLoadReq);
    Task<FeedDto> GetFeedAsync(string hashId);
    Task<PagedResponse<FeedDto>> GetFeedByKeywordAsync(string tagName, FeedSearchKeywordR feedLoadReq);
    FeedDisplayConfig GetFeedDisplayConfig();
    Task<FeedDto> PostFeedAsync(PostCreateR req);
    Task<FeedDto> UpdateFeedAsync(string hashId, PostUpdateR feedPostReq);
    Task<bool> DeleteFeedAsync(Guid postId);

    Task<bool> ReportFeedAsync(FeedReportPostReq req);
    FeedDto MappingFeedInListRespone(FeedsListQueryDbDto item);
    Task<SubPostFeedResponse> GetFeedSubPostAsync(string hashId);
    Task<List<FeedBoxResponse>> GetFeedsByIds(string hashIds);
}
