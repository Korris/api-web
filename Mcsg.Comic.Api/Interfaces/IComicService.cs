namespace Mcsg.Comic.Api.Interfaces;

using Common.Core.Requests;
using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IComicService
{
    Task<PostSeriesResponse> PostCreate(ComicPostCreateR request);
    Task<PostSeriesResponse> PostUpdate(string hashId, ComicPostUpdateR request);
    Task<ChapterResponse> SubPostCreate(string comicHashId, ComicSubPostCreateR request);
    Task<ChapterResponse> SubPostUpdate(string comicHashId, float order, ComicSubPostUpdateR request);

    Task<PostSeriesResponse> GetComic(ComicHashIdR req);
    Task<ChapterResponse> GetChapter(string hashId, float order);
    Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, ComicChapterListR request);
    Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
    Task<PostSeriesAllTopResponse> GetTopComic();
    Task<PagedResponse<PostSeriesTopResponse>> GetTopComicAsync(ComicPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetMyComics(ComicPostListSeriesR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopHitListComic(ComicTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopLatestListComic(ComicTopPostR req);
    Task<List<PostSeriesTopResponse>> GetRecommendedComic(ComicRecommendedR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopCompletedListComic(ComicTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetRelationComicsAsync(ComicRelationPostSeriesR request);
    Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, ComicChapterOrderSwapR orders);
    Task<bool> DeleteChapter(string comicHashId, float order);
    Task<bool> Delete(Guid postId);
    Task<PagedResponse<PostBoxResposne>> GetComicByUserProfileName(ComicPostByProFileNameR request);
    Task<PagedResponse<PostBoxResposne>> GetComicByTagName(ComicPostByTagNameR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(PaginatedR input);
    Task<bool> FollowPost(Guid postId);
    Task<float> GetLatestOrderChapter(string hashPostId);
}
