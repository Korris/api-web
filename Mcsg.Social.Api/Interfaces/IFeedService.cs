namespace Mcsg.Social.Api.Interfaces;

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
    Task<FeedDto> GetFeedAsync(FeedHashIdR req);
    Task<PagedResponse<FeedDto>> GetFeedByKeywordAsync(string tagName, FeedSearchKeywordR feedLoadReq);
    FeedDisplayConfig GetFeedDisplayConfig();
    Task<bool> DeleteFeedAsync(Guid postId);
    FeedDto MappingFeedInListRespone(FeedsListQueryDbDto item, List<Guid>? postIds);
    Task<SubPostFeedResponse> GetFeedSubPostAsync(IdBaseR request);
    Task<List<FeedBoxResponse>> GetFeedsByIds(FeedHashIdsR req);
    Task<PagedResponse<FeedDto>> GetFeedByUserNameOrKeyword(FeedPostByProFileNameR feedLoadReq);
}
