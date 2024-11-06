namespace Mcsg.Story.Api.Interfaces;

using Common.Core.Requests;
using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IStoryService
{
    Task<PostSeriesResponse> Get(StoryHashIdR req);
    Task<List<ChapterResponse>> SwapChapterOrder(string hashId, StoryChapterOrderSwapR orders);
    Task<ChapterResponse> GetChapter(ChapterOrderR req);
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
    Task<FavoritePostResponse> FollowPost(IdBaseR request);
    Task<float> GetLatestOrderChapter(string hashPostId);
    Task MoveChapterOrder(string hashId, StoryChapterOrderSwapR orders);
    Task<List<ChapterList>> GetAllChapters(string hashId);
}
