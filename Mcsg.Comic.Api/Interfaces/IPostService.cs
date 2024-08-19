namespace Mcsg.Comic.Api.Interfaces;

using Common.Core.Enums;
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
    Task<PostSeriesResponse> PostSeries(PostType type, ComicPostSeriesR postReq);
    Task<PostSeriesResponse> UpdateSeries(string hashId, ComicPostUpdateSeriesR postReq);
    Task<PostSeriesResponse> GetSeries(string hashId, bool isLoadChapters);
    Task<ChapterResponse> GetSeriesChapter(string hashId, float order);
    Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, ComicChapterListR request);
    Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
    Task<PagedResponse<ChapterTOCExtendResponse>> GetChaptersListSimple(Guid userId, string hashId, ComicChapterListR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetMySeries(PostType type, ComicPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopSeriesByPage(PostType type, PostSeriesSelectedType selectedType, ComicTopPostR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByTagByPage(PostType type, string tagName, ComicTopPostR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByUserByPage(PostType type, string profileName, ComicTopPostR loadReq);
    Task<List<PostSeriesTopResponse>> GetTopNewSeries(PostType type, int number);
    Task<PostSeriesAllTopResponse> GetTopSeries(PostType type);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopSeriesAsync(PostType type, ComicPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetRelationSeriesAsync(PostType type, ComicRelationPostSeriesR request);

    Task<ComicSubPost> SubPostChapterToSeries(string hashId, StoryChapterPostR chapterPostReq);
    Task<ComicSubPost> SubPostUpdateChapterToSeries(string hashId, float order, StoryChapterPostR chapterPostReq);
    Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, ComicChapterOrderSwapR orders);
    Task<bool> DeleteChapter(string hashId, int order);
    void VerifyBasicInfo(string title);
    ChapterResponse MappingChapterResponse(ComicSubPost newChapter);
    Task<List<RewardDto>> CheckRewardsForPost(Guid currentUserId, PostType type);
    Task<List<MyPostSeriesResponse>> GetMyAllSeries();
    Task UpdateKeyWordForComicAndStoryToSmartLookup();
    Task<PagedResponse<PostBoxResposne>> GetPostByUserProfileName(PostType type, ComicPostByProFileNameR input);
    Task<PagedResponse<PostBoxResposne>> GetPostByTagName(PostType type, ComicPostByTagNameR input);
    Task<IEnumerable<string>> GetPostRandomIdsAsync(PostRandomIdsR req);
    Task<ListIdForHomePage> GetLatestPostsByType();
    Task<ListIdForHomePage> GetLatestPostsByTag(string nameTag);
    Task<List<PostBoxResponse>> GetPostDetails(string hashIds);
    Task<IEnumerable<string>> GetSubPostRandomIdsAsync(PostRandomIdsR input);
    Task<List<NewsFeedDto>> GetNewsFeed(UserNamePagingR input);
    Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(BasePageResultR loadReq);
    Task<bool> FollowPost(Guid postId);
}
