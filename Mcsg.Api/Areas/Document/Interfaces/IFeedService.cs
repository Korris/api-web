namespace Mcsg.Api.Areas.Document.Interfaces;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Document.Dtos;
using Mcsg.Api.Areas.Document.Models;
using Mcsg.Api.Areas.Document.Requests;

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
