namespace Mcsg.Api.Areas.Social.Interfaces;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Dtos;
using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Areas.Social.Requests;

public interface IFeedService
{
    Task<PagedResponse<FeedDto>> GetFeedsAsync(FeedLoadReq feedLoadReq, LoadFeedType loadFeedType);
    Task<PagedResponse<FeedDto>> GetFeedsByTagAsync(string tagName, FeedLoadReq feedLoadReq);
    Task<FeedDto> GetFeedAsync(FeedHashIdR req);
    Task<PagedResponse<FeedDto>> GetFeedByKeywordAsync(string tagName, FeedSearchKeywordR feedLoadReq);
    FeedDisplayConfig GetFeedDisplayConfig();
    Task<bool> DeleteFeedAsync(IdBaseR request);
    FeedDto MappingFeedInListRespone(FeedsListQueryDbDto item, List<Guid>? postIds, bool? isMySelf);
    Task<SubPostFeedResponse> GetFeedSubPostAsync(IdBaseR request);
    Task<List<FeedBoxResponse>> GetFeedsByIds(PaginatedR req);
    Task<PagedResponse<FeedDto>> GetFeedByUserNameOrKeyword(FeedPostByProFileNameR feedLoadReq);
    Task<List<SharePostResponse>> GetSharePosts(BaseR req, List<SharePostInput>? ids);
}
