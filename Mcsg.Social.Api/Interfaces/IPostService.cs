namespace Mcsg.Social.Api.Interfaces;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.SeedWork.Responses;
using Dtos;
using Models;
using Models.Earning;
using Requests;

public interface IPostService
{
    Task<bool> Delete(IdBaseR request);
    Task<PagedResponse<ChapterTOCExtendResponse>> GetChaptersListSimple(Guid userId, string hashId, PostChapterListR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByTagByPage(PostType type, string tagName, PostTopR loadReq);
    Task<List<RewardDto>> CheckRewardsForPost(Guid currentUserId, PostType type);
    Task<List<MyPostSeriesResponse>> GetMyAllSeries(BaseR request);
    Task<IEnumerable<string>> GetPostRandomIdsAsync(PostRandomIdsR req);
    Task<ListIdForHomePage> GetLatestPostsByType();
    Task<ListIdForHomePage> GetLatestPostsByTag(string nameTag);
    Task<List<PostBoxResponse>> GetPostDetails(string hashIds, BaseR request);
    Task<IEnumerable<string>> GetSubPostRandomIdsAsync(PostRandomIdsR input);
    Task<PagedResponse<RelatedBoxResponse>> GetPostMaybeYouLike(UserNamePagingR input);
    Task<List<NewsFeedDto>> GetNewsFeed(UserNamePagingR input);
    Task<Tuple<int, int>> GetFollowedPostCount(BaseR req);
}
