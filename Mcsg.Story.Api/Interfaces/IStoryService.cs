namespace Mcsg.Story.Api.Interfaces;

using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IStoryService
{
    Task<PostSeriesResponse> PostStory(StoryPostSeriesR request);
    Task<PostSeriesResponse> GetStory(string hashId, bool isLoadChapters);
    Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, ComicChapterListR request);
    Task<PostSeriesResponse> UpdateStory(string hashId, ComicPostUpdateSeriesR request);
    Task<ChapterResponse> PostChapterToStory(string comicHashId, StoryChapterR chapterPostReq);
    Task<ChapterResponse> UpdateChapterToStory(string comicHashId, int order, StoryChapterR chapterPostReq);
    Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, ComicChapterOrderSwapR orders);
    Task<bool> DeleteChapter(string comicHashId, int order);
    Task<bool> Delete(Guid postId);

    Task<ChapterResponse> GetChapter(string hashId, float order);
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
    Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(BasePageResultR input);
    Task<bool> FollowPost(Guid postId);
    Task<float> GetLatestOrderChapter(string hashPostId);
}
