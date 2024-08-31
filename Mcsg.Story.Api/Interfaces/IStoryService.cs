namespace Mcsg.Story.Api.Interfaces;

using Common.Core.Requests;
using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IStoryService
{
    Task<PostSeriesResponse> PostCreate(StoryPostCreateR request);
    Task<PostSeriesResponse> PostUpdate(string hashId, StoryPostUpdateR request);
    Task<ChapterResponse> SubPostCreate(string comicHashId, StorySubPostCreateR request);
    Task<ChapterResponse> SubPostUpdate(string comicHashId, float order, StorySubPostUpdateR request);

    Task<PostSeriesResponse> Get(StoryHashIdR req);
    Task<List<ChapterResponse>> SwapChapterOrder(string hashId, StoryChapterOrderSwapR orders);
    Task<bool> DeleteChapter(string comicHashId, float order);
    Task<bool> Delete(Guid postId);
    Task<ChapterResponse> GetChapter(string hashId, float order);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopHitList(StoryTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopLatestList(StoryTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopCompletedList(StoryTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetMy(StoryPostListSeriesR loadReq);
    Task<PostSeriesAllTopResponse> GetTop();
    Task<PagedResponse<PostSeriesTopResponse>> GetTopAsync(StoryPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetRelationAsync(StoryRelationPostSeriesR request);
    Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, StoryChapterListR request);
    Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
    Task<PagedResponse<PostBoxResposne>> GetByUserProfileName(StoryPostByProFileNameR request);
    Task<PagedResponse<PostBoxResposne>> GetByTagName(StoryPostByTagNameR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(PaginatedR input);
    Task<bool> FollowPost(Guid postId);
    Task<float> GetLatestOrderChapter(string hashPostId);
}
