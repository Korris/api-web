namespace Mcsg.Story.Api.Interfaces;

using Common.Core.Requests;
using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IStoryService
{
    Task<PostSeriesResponse> PostStory(StoryPostCreateR request);
    Task<PostSeriesResponse> GetStory(StoryHashIdR req);
    Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, StoryChapterListR request);
    Task<PostSeriesResponse> UpdateStory(string hashId, StoryPostUpdateR request);
    Task<ChapterResponse> PostChapterToStory(string comicHashId, StorySubPostCreateR request);
    Task<ChapterResponse> UpdateChapterToStory(string comicHashId, int order, StorySubPostUpdateR request);
    Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, StoryChapterOrderSwapR orders);
    Task<bool> DeleteChapter(string comicHashId, int order);
    Task<bool> Delete(Guid postId);

    Task<ChapterResponse> GetChapter(string hashId, float order);
    Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
    Task<PostSeriesAllTopResponse> GetTopStory();
    Task<PagedResponse<PostSeriesTopResponse>> GetTopStoryAsync(StoryPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetMyStories(StoryPostListSeriesR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopHitListStory(StoryTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopLatestListStory(StoryTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopCompletedListStory(StoryTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetRelationStoriesAsync(StoryRelationPostSeriesR request);
    Task<PagedResponse<PostBoxResposne>> GetStoryByUserProfileName(StoryPostByProFileNameR request);
    Task<PagedResponse<PostBoxResposne>> GetStoryByTagName(StoryPostByTagNameR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(PaginatedR input);
    Task<bool> FollowPost(Guid postId);
    Task<float> GetLatestOrderChapter(string hashPostId);
}
