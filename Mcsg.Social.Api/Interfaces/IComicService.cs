namespace Mcsg.Social.Api.Interfaces
{
    using DTOs;
    using Lib.Data.Entities.Common;
    using Models;

    public interface IComicService
    {
        Task<PostSeriesResponse> GetComic(string hashId, bool isLoadChapters);
        Task<ChapterResponse> GetChapter(string hashId, int order);
        Task<PagedResults<ChapterResponse>> GetChapters(string hashId, ChapterListReq request);
        Task<PagedResults<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
        Task<PostSeriesAllTopResponse> GetTopComic();
        Task<PagedResults<PostSeriesTopResponse>> GetTopComicAsync(PostListSeriesReq request);
        Task<PagedResults<PostSeriesTopResponse>> GetMyComics(PostListSeriesReq loadReq);
        Task<PagedResults<PostSeriesTopResponse>> GetTopHitListComic(TopPostReq req);
        Task<PagedResults<PostSeriesTopResponse>> GetTopLatestListComic(TopPostReq req);
        Task<List<PostSeriesTopResponse>> GetRecommendedComic(int number);
        Task<PagedResults<PostSeriesTopResponse>> GetTopCompletedListComic(TopPostReq req);
        Task<PagedResults<PostSeriesTopResponse>> GetRelationComicsAsync(RelationPostSeriesReq request);
        Task<PostSeriesResponse> PostComic(PostSeriesReq comicPostReq);
        Task<PostSeriesResponse> UpdateComic(string hashId, PostUpdateSeriesReq comicPostReq);
        Task<ChapterResponse> PostChapterToComic(string comicHashId, ChapterComicReq chapterPostReq);
        Task<ChapterResponse> UpdateChapterToComic(string comicHashId, int order, ChapterComicReq chapterPostReq);
        Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, ChapterOrderSwapReq orders);
        Task<bool> DeleteChapter(string comicHashId, int order);
        Task<bool> Delete(Guid postId);
        Task<PagedResults<PostBoxResposne>> GetComicByUserProfileName(PostByProFileNameInput request);
        Task<PagedResults<PostBoxResposne>> GetComicByTagName(PostByTagNameInput request);
    }
}
