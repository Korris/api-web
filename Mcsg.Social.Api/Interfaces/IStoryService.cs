namespace Mcsg.Social.Api.Interfaces
{
    using Lib.Data.Entities.Common;
    using Models;
    using Requests;

    public interface IStoryService
    {
        Task<PostSeriesResponse> PostStory(ComicPostSeriesR comicPostReq);
        Task<PostSeriesResponse> GetStory(string hashId, bool isLoadChapters);
        Task<PagedResults<ChapterResponse>> GetChapters(string hashId, ComicChapterListR request);
        Task<PostSeriesResponse> UpdateStory(string hashId, ComicPostUpdateSeriesR storyPostReq);
        Task<ChapterResponse> PostChapterToStory(string comicHashId, ChapterStoryReq chapterPostReq);
        Task<ChapterResponse> UpdateChapterToStory(string comicHashId, int order, ChapterStoryReq chapterPostReq);
        Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, ComicChapterOrderSwapR orders);
        Task<bool> DeleteChapter(string comicHashId, int order);
        Task<bool> Delete(Guid postId);

        Task<ChapterResponse> GetChapter(string hashId, int order);
        Task<PagedResults<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
        Task<PostSeriesAllTopResponse> GetTopStory();
        Task<PagedResults<PostSeriesTopResponse>> GetTopStoryAsync(ComicPostListSeriesR request);
        Task<PagedResults<PostSeriesTopResponse>> GetMyStories(ComicPostListSeriesR loadReq);
        Task<PagedResults<PostSeriesTopResponse>> GetTopHitListStory(ComicTopPostR req);
        Task<PagedResults<PostSeriesTopResponse>> GetTopLatestListStory(ComicTopPostR req);
        Task<PagedResults<PostSeriesTopResponse>> GetTopCompletedListStory(ComicTopPostR req);
        Task<PagedResults<PostSeriesTopResponse>> GetRelationStoriesAsync(ComicRelationPostSeriesR request);
        Task<PagedResults<PostBoxResposne>> GetStoryByUserProfileName(ComicPostByProFileNameR request);
        Task<PagedResults<PostBoxResposne>> GetStoryByTagName(ComicPostByTagNameR request);
    }
}
