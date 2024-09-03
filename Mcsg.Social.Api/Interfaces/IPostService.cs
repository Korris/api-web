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
    Task<bool> Delete(Guid postId);
    Task<PagedResponse<ChapterTOCExtendResponse>> GetChaptersListSimple(Guid userId, string hashId, ComicChapterListR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByTagByPage(PostType type, string tagName, ComicTopPostR loadReq);
    Task<List<RewardDto>> CheckRewardsForPost(Guid currentUserId, PostType type);
    Task<List<MyPostSeriesResponse>> GetMyAllSeries();
    Task UpdateKeyWordForComicAndStoryToSmartLookup();
    Task<IEnumerable<string>> GetPostRandomIdsAsync(PostRandomIdsR req);
    Task<ListIdForHomePage> GetLatestPostsByType();
    Task<ListIdForHomePage> GetLatestPostsByTag(string nameTag);
    Task<List<PostBoxResponse>> GetPostDetails(string hashIds);
    Task<IEnumerable<string>> GetSubPostRandomIdsAsync(PostRandomIdsR input);
    Task<PagedResponse<RelatedBoxResponse>> GetPostMaybeYouLike(UserNamePagingR input);
    Task<List<NewsFeedDto>> GetNewsFeed(UserNamePagingR input);
    Task<Tuple<int, int>> GetFollowedPostCount(BaseR req);
}
