namespace Mcsg.Story.Api.Interfaces;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Dtos;
using Enums;
using Models;
using Models.Earning;
using Requests;

public interface IPostService
{
    Task<bool> Delete(Guid postId);
    Task<PostSeriesResponse> PostSeries(PostType type, StoryPostCreateR postReq);
    Task<PostSeriesResponse> UpdateSeries(string hashId, StoryPostUpdateR postReq);
    Task<PostSeriesResponse> GetSeries(StoryHashIdR req);
    Task<ChapterResponse> GetSeriesChapter(string hashId, float order);
    Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, StoryChapterListR request);
    Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
    Task<PagedResponse<ChapterTOCExtendResponse>> GetChaptersListSimple(Guid userId, string hashId, StoryChapterListR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetMySeries(PostType type, StoryPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopSeriesByPage(PostType type, PostSeriesSelectedType selectedType, StoryTopPostR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByTagByPage(PostType type, string tagName, StoryTopPostR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByUserByPage(PostType type, string profileName, StoryTopPostR loadReq);
    Task<List<PostSeriesTopResponse>> GetTopNewSeries(PostType type, StoryRecommendedR req);
    Task<PostSeriesAllTopResponse> GetTopSeries(PostType type);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopSeriesAsync(PostType type, StoryPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetRelationSeriesAsync(PostType type, StoryRelationPostSeriesR request);

    Task<StorySubPost> SubPostChapterToSeries(string hashId, StorySubPostFormBaseR chapterPostReq);
    Task<StorySubPost> SubPostUpdateChapterToSeries(string hashId, float order, StorySubPostFormBaseR chapterPostReq);
    Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, StoryChapterOrderSwapR orders);
    Task<bool> DeleteChapter(string hashId, int order);
    ChapterResponse MappingChapterResponse(StorySubPost newChapter);
    Task<List<RewardDto>> CheckRewardsForPost(Guid currentUserId, PostType type);
    Task<List<MyPostSeriesResponse>> GetMyAllSeries();
    Task UpdateKeyWordForComicAndStoryToSmartLookup();
    Task<PagedResponse<PostBoxResposne>> GetPostByUserProfileName(PostType type, StoryPostByProFileNameR input);
    Task<PagedResponse<PostBoxResposne>> GetPostByTagName(PostType type, StoryPostByTagNameR input);
    Task<IEnumerable<string>> GetPostRandomIdsAsync(PostRandomIdsR req);
    Task<ListIdForHomePage> GetLatestPostsByType();
    Task<ListIdForHomePage> GetLatestPostsByTag(string nameTag);
    Task<List<PostBoxResponse>> GetPostDetails(StoryHashIdsR req);
    Task<IEnumerable<string>> GetSubPostRandomIdsAsync(PostRandomIdsR input);
    Task<List<NewsFeedDto>> GetNewsFeed(UserNamePagingR input);
    Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(PaginatedR loadReq);
    Task<bool> FollowPost(Guid postId);
}
