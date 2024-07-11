namespace Mcsg.Social.Api.Interfaces;

using Common.Core.Enums;
using Enums;
using Lib.Data.Domain.Entities;
using Lib.Data.Entities.Common;
using Models;
using Models.Earning;
using Requests;

public interface IPostService
{
    Task<bool> Delete(Guid postId);
    Task<PostSeriesResponse> PostSeries(PostType type, ComicPostSeriesR postReq);
    Task<PostSeriesResponse> UpdateSeries(string hashId, ComicPostUpdateSeriesR postReq);
    Task<PostSeriesResponse> GetSeries(string hashId, bool isLoadChapters);
    Task<ChapterResponse> GetSeriesChapter(string hashId, int order);
    Task<PagedResults<ChapterResponse>> GetChapters(string hashId, ComicChapterListR request);
    Task<PagedResults<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
    Task<PagedResults<ChapterTOCExtendResponse>> GetChaptersListSimple(Guid userId, string hashId, ComicChapterListR loadReq);
    Task<PagedResults<PostSeriesTopResponse>> GetMySeries(PostType type, ComicPostListSeriesR request);
    Task<PagedResults<PostSeriesTopResponse>> GetTopSeriesByPage(PostType type, PostSeriesSelectedType selectedType, ComicTopPostR loadReq);
    Task<PagedResults<PostSeriesTopResponse>> GetSeriesByTagByPage(PostType type, string tagName, ComicTopPostR loadReq);
    Task<PagedResults<PostSeriesTopResponse>> GetSeriesByUserByPage(PostType type, string profileName, ComicTopPostR loadReq);
    Task<List<PostSeriesTopResponse>> GetTopNewSeries(PostType type, int number);
    Task<PostSeriesAllTopResponse> GetTopSeries(PostType type);
    Task<PagedResults<PostSeriesTopResponse>> GetTopSeriesAsync(PostType type, ComicPostListSeriesR request);
    Task<PagedResults<PostSeriesTopResponse>> GetRelationSeriesAsync(PostType type, ComicRelationPostSeriesR request);

    Task<SubPost> SubPostChapterToSeries(string hashId, StoryChapterPostR chapterPostReq);
    Task<SubPost> SubPostUpdateChapterToSeries(string hashId, int order, StoryChapterPostR chapterPostReq);
    Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, ComicChapterOrderSwapR orders);
    Task<bool> DeleteChapter(string hashId, int order);
    void VerifyBasicInfo(string title);
    ChapterResponse MappingChapterResponse(SubPost newChapter);
    Task<List<RewardRespone>> CheckRewardsForPost(Guid currentUserId, PostType type);
    Task<List<MyPostSeriesResponse>> GetMyAllSeries();
    Task<bool> ReportPostAsync(FeedReportPostReq req);
    Task UpdateKeyWordForComicAndStoryToSmartLookup();
    Task<PagedResults<PostBoxResposne>> GetPostByUserProfileName(PostType type, ComicPostByProFileNameR input);
    Task<PagedResults<PostBoxResposne>> GetPostByTagName(PostType type, ComicPostByTagNameR input);
    Task<IEnumerable<string>> GetPostRandomIdsAsync(PostRandomIdsR req);
    Task<ListIdForHomePage> GetLatestPostsByType();
    Task<ListIdForHomePage> GetLatestPostsByTag(string nameTag);
    Task<List<PostBoxResponse>> GetPostDetails(string hashIds);
    Task<IEnumerable<string>> GetSubPostRandomIdsAsync(PostRandomIdsR input);
    Task<PagedResults<RelatedBoxResponse>> GetPostMaybeYouLike(BasePageResultR input);
    Task<List<NewsFeedDto>> GetNewsFeed(int amount);
}
