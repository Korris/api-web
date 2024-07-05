namespace Mcsg.Social.Api.Interfaces
{
    using Lib.Data.Entities.Common;
    using Models;
    using Requests;

    public interface IComicService
    {
        Task<PostSeriesResponse> GetComic(string hashId, bool isLoadChapters);
        Task<ChapterResponse> GetChapter(string hashId, int order);
        Task<PagedResults<ChapterResponse>> GetChapters(string hashId, ComicChapterListR request);
        Task<PagedResults<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
        Task<PostSeriesAllTopResponse> GetTopComic();
        Task<PagedResults<PostSeriesTopResponse>> GetTopComicAsync(ComicPostListSeriesR request);
        Task<PagedResults<PostSeriesTopResponse>> GetMyComics(ComicPostListSeriesR loadReq);
        Task<PagedResults<PostSeriesTopResponse>> GetTopHitListComic(ComicTopPostR req);
        Task<PagedResults<PostSeriesTopResponse>> GetTopLatestListComic(ComicTopPostR req);
        Task<List<PostSeriesTopResponse>> GetRecommendedComic(int number);
        Task<PagedResults<PostSeriesTopResponse>> GetTopCompletedListComic(ComicTopPostR req);
        Task<PagedResults<PostSeriesTopResponse>> GetRelationComicsAsync(ComicRelationPostSeriesR request);
        Task<PostSeriesResponse> PostComic(ComicPostSeriesR comicPostReq);
        Task<PostSeriesResponse> UpdateComic(string hashId, ComicPostUpdateSeriesR comicPostReq);
        Task<ChapterResponse> PostChapterToComic(string comicHashId, ComicChapterComicR chapterPostReq);
        Task<ChapterResponse> UpdateChapterToComic(string comicHashId, int order, ComicChapterComicR chapterPostReq);
        Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, ComicChapterOrderSwapR orders);
        Task<bool> DeleteChapter(string comicHashId, int order);
        Task<bool> Delete(Guid postId);
        Task<PagedResults<PostBoxResposne>> GetComicByUserProfileName(ComicPostByProFileNameR request);
        Task<PagedResults<PostBoxResposne>> GetComicByTagName(ComicPostByTagNameR request);
    }
}
