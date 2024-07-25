namespace Mcsg.Comic.Api.Interfaces;

using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IComicService
{
    Task<PostSeriesResponse> GetComic(string hashId, bool isLoadChapters);
    Task<ChapterResponse> GetChapter(string hashId, int order);
    Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, ComicChapterListR request);
    Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
    Task<PostSeriesAllTopResponse> GetTopComic();
    Task<PagedResponse<PostSeriesTopResponse>> GetTopComicAsync(ComicPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetMyComics(ComicPostListSeriesR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopHitListComic(ComicTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopLatestListComic(ComicTopPostR req);
    Task<List<PostSeriesTopResponse>> GetRecommendedComic(int number);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopCompletedListComic(ComicTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetRelationComicsAsync(ComicRelationPostSeriesR request);
    Task<PostSeriesResponse> PostComic(ComicPostSeriesR comicPostReq);
    Task<PostSeriesResponse> UpdateComic(string hashId, ComicPostUpdateSeriesR comicPostReq);
    Task<ChapterResponse> PostChapterToComic(string comicHashId, ComicChapterComicR chapterPostReq);
    Task<ChapterResponse> UpdateChapterToComic(string comicHashId, float order, ComicChapterComicR chapterPostReq);
    Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, ComicChapterOrderSwapR orders);
    Task<bool> DeleteChapter(string comicHashId, int order);
    Task<bool> Delete(Guid postId);
    Task<PagedResponse<PostBoxResposne>> GetComicByUserProfileName(ComicPostByProFileNameR request);
    Task<PagedResponse<PostBoxResposne>> GetComicByTagName(ComicPostByTagNameR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(BasePageResultR input);
    Task<bool> FollowPost(Guid postId);
}
