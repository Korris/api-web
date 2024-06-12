using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Enums;
using Mcsg.Social.Api.Models;
using Mcsg.Lib.Data.Entities.Common;

namespace Mcsg.Social.Api.Services.Interfaces
{
    public interface IFeedService
    {

        Task<PagedResults<FeedResponse>> GetFeedsAsync(FeedLoadReq feedLoadReq, LoadFeedType loadFeedType);
        Task<PagedResults<FeedResponse>> GetFeedsByTagAsync(string tagName, FeedLoadReq feedLoadReq);
        Task<FeedResponse> GetFeedAsync(string hashId);
        Task<PagedResults<FeedResponse>> GetFeedByKeywordAsync(string tagName, SearchKeywordReq feedLoadReq);
        FeedDisplayConfig GetFeedDisplayConfig();
        Task<FeedResponse> PostFeedAsync(FeedPostReq feedPostReq);
        Task<FeedResponse> UpdateFeedAsync(string hashId, UpdateFeedPostReq feedPostReq);
        Task<bool> DeleteFeedAsync(Guid postId);

        Task<bool> ReportFeedAsync(ReportPostReq req);
        FeedResponse MappingFeedInListRespone(FeedsListQueryDbResponse item);
        Task<SubPostFeedResponse> GetFeedSubPostAsync(string hashId);
    }
}
