namespace Mcsg.Story.Api.Interfaces;

using Common.SeedWork.Responses;
using Dtos;
using Enums;
using Models;
using Requests;

public interface IFeedService
{
    Task<PagedResponse<FeedDto>> GetFeedsAsync(FeedLoadReq feedLoadReq, LoadFeedType loadFeedType);
    Task<PagedResponse<FeedDto>> GetFeedsByTagAsync(string tagName, FeedLoadReq feedLoadReq);
    Task<FeedDto> GetFeedAsync(string hashId, Guid userId);
    Task<PagedResponse<FeedDto>> GetFeedByKeywordAsync(string tagName, FeedSearchKeywordR feedLoadReq);
    FeedDisplayConfig GetFeedDisplayConfig();
    Task<bool> DeleteFeedAsync(Guid postId);
    FeedDto MappingFeedInListRespone(FeedsListQueryDbDto item);
    Task<SubPostFeedResponse> GetFeedSubPostAsync(string hashId, Guid userId);
    Task<List<FeedBoxResponse>> GetFeedsByIds(string hashIds);
}
