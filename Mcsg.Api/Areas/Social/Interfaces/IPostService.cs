namespace Mcsg.Api.Areas.Social.Interfaces;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Dtos;
using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Areas.Social.Models.Earning;
using Mcsg.Api.Areas.Social.Requests;

public interface IPostService
{
    Task<bool> Delete(IdBaseR request);
    Task<PagedResponse<ChapterTOCExtendResponse>> GetChaptersListSimple(Guid userId, string hashId, PostChapterListR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByTagByPage(PostType type, string tagName, PostTopR loadReq);
    Task<List<RewardDto>> CheckRewardsForPost(Guid currentUserId, PostType type);
    Task<List<MyPostSeriesResponse>> GetMyAllSeries(BaseR request);
    Task<IEnumerable<string>> GetPostRandomIdsAsync(PostRandomIdsR req);
    Task<ListIdForHomePage> GetLatestPostsByType(BaseR req);
    Task<ListIdForHomePage> GetHomeFeedIds(PostHomeFeedR req);
    Task<ListIdForHomePage> GetLatestPostsByTag(string nameTag);
    Task<List<LatestPostCardResponse>> GetLatestPosts(int take);
    Task<List<UpcomingSubPostResponse>> GetUpcomingSubPosts(int take);
    Task<List<ActiveCommentPostResponse>> GetActiveCommentPosts(Guid userId, int take);
    Task<List<PostBoxResponse>> GetPostDetails(string hashIds, BaseR request);
    Task<IEnumerable<string>> GetSubPostRandomIdsAsync(PostRandomIdsR input);
    Task<PagedResponse<RelatedBoxResponse>> GetPostMaybeYouLike(UserNamePagingR input);
    Task<List<NewsFeedDto>> GetNewsFeed(UserNamePagingR input);
    Task<Tuple<int, int>> GetFollowedPostCount(BaseR req);
}
