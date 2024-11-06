namespace Mcsg.Comic.Api.Interfaces;

using Common.Core.Requests;
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
    Task<bool> DeleteFeedAsync(IdBaseR request);
    FeedDto MappingFeedInListRespone(FeedsListQueryDbDto item);
    Task<SubPostFeedResponse> GetFeedSubPostAsync(IdBaseR request);
    Task<List<FeedBoxResponse>> GetFeedsByIds(PaginatedR request);
}
