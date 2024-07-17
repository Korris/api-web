namespace Mcsg.Comic.Api.Interfaces;

using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IStoryService
{
    Task<PostSeriesResponse> PostStory(ComicPostSeriesR comicPostReq);
    Task<PostSeriesResponse> GetStory(string hashId, bool isLoadChapters);
    Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, ComicChapterListR request);
    Task<PostSeriesResponse> UpdateStory(string hashId, ComicPostUpdateSeriesR storyPostReq);
    Task<ChapterResponse> PostChapterToStory(string comicHashId, StoryChapterR chapterPostReq);
    Task<ChapterResponse> UpdateChapterToStory(string comicHashId, int order, StoryChapterR chapterPostReq);
    Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, ComicChapterOrderSwapR orders);
    Task<bool> DeleteChapter(string comicHashId, int order);
    Task<bool> Delete(Guid postId);

    Task<ChapterResponse> GetChapter(string hashId, int order);
    Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
    Task<PostSeriesAllTopResponse> GetTopStory();
    Task<PagedResponse<PostSeriesTopResponse>> GetTopStoryAsync(ComicPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetMyStories(ComicPostListSeriesR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopHitListStory(ComicTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopLatestListStory(ComicTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopCompletedListStory(ComicTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetRelationStoriesAsync(ComicRelationPostSeriesR request);
    Task<PagedResponse<PostBoxResposne>> GetStoryByUserProfileName(ComicPostByProFileNameR request);
    Task<PagedResponse<PostBoxResposne>> GetStoryByTagName(ComicPostByTagNameR request);
}
