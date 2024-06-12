using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Models;
using Mcsg.Lib.Data.Entities.Common;

namespace Mcsg.Social.Api.Services.Interfaces
{
    public interface IStoryService
    {
        Task<PostSeriesResponse> PostStory(PostSeriesReq comicPostReq);
        Task<PostSeriesResponse> GetStory(string hashId, bool isLoadChapters);
        Task<PagedResults<ChapterResponse>> GetChapters(string hashId, ChapterListReq request);
        Task<PostSeriesResponse> UpdateStory(string hashId, PostUpdateSeriesReq storyPostReq);
        Task<ChapterResponse> PostChapterToStory(string comicHashId, ChapterStoryReq chapterPostReq);
        Task<ChapterResponse> UpdateChapterToStory(string comicHashId, int order, ChapterStoryReq chapterPostReq);
        Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, ChapterOrderSwapReq orders);
        Task<bool> DeleteChapter(string comicHashId, int order);
        Task<bool> Delete(Guid postId);

        Task<ChapterResponse> GetChapter(string hashId, int order);
        Task<PagedResults<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
        Task<PostSeriesAllTopResponse> GetTopStory();
        Task<PagedResults<PostSeriesTopResponse>> GetTopStoryAsync(PostListSeriesReq request);
        Task<PagedResults<PostSeriesTopResponse>> GetMyStories(PostListSeriesReq loadReq);
        Task<PagedResults<PostSeriesTopResponse>> GetTopHitListStory(TopPostReq req);
        Task<PagedResults<PostSeriesTopResponse>> GetTopLatestListStory(TopPostReq req);
        Task<PagedResults<PostSeriesTopResponse>> GetTopCompletedListStory(TopPostReq req);
        Task<PagedResults<PostSeriesTopResponse>> GetRelationStoriesAsync(RelationPostSeriesReq request);
        Task<PagedResults<PostBoxResposne>> GetStoryByUserProfileName(PostByProFileNameInput request);
        Task<PagedResults<PostBoxResposne>> GetStoryByTagName(PostByTagNameInput request);
    }
}
