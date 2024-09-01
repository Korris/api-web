namespace Mcsg.Comic.Api.Interfaces;

using Common.Core.Requests;
using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IComicService
{
    Task<PostSeriesResponse> Get(ComicHashIdR req);
    Task<List<ChapterResponse>> SwapChapterOrder(string hashId, ComicChapterOrderSwapR orders);
    Task<ChapterResponse> GetChapter(string hashId, float order);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopHitList(ComicTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopLatestList(ComicTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopCompletedList(ComicTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetMy(ComicPostListSeriesR loadReq);
    Task<PostSeriesAllTopResponse> GetTop();
    Task<PagedResponse<PostSeriesTopResponse>> GetTopAsync(ComicPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetRelationAsync(ComicRelationPostSeriesR request);
    Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, ComicChapterListR request);
    Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
    Task<PagedResponse<PostBoxResposne>> GetByUserProfileName(ComicPostByProFileNameR request);
    Task<PagedResponse<PostBoxResposne>> GetByTagName(ComicPostByTagNameR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(PaginatedR input);
    Task<bool> FollowPost(Guid postId);
    Task<float> GetLatestOrderChapter(string hashPostId);
    Task<List<PostSeriesTopResponse>> GetRecommended(ComicRecommendedR req);
}
