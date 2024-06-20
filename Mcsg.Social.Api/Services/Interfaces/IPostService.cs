using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Enums;
using Mcsg.Social.Api.Models;
using Mcsg.Social.Api.Models.Earning;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Entities.Common;
using Mcsg.Lib.Model.Enums;

namespace Mcsg.Social.Api.Services.Interfaces
{
    public interface IPostService
    {
        Task<bool> Delete(Guid postId);
        Task<PostSeriesResponse> PostSeries(PostType type, PostSeriesReq postReq);
        Task<PostSeriesResponse> UpdateSeries(string hashId, PostUpdateSeriesReq postReq);
        Task<PostSeriesResponse> GetSeries(string hashId, bool isLoadChapters);
        Task<ChapterResponse> GetSeriesChapter(string hashId, int order);
        Task<PagedResults<ChapterResponse>> GetChapters(string hashId, ChapterListReq request);
        Task<PagedResults<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
        Task<PagedResults<ChapterTOCExtendResponse>> GetChaptersListSimple(Guid userId, string hashId, ChapterListReq loadReq);
        Task<PagedResults<PostSeriesTopResponse>> GetMySeries(PostType type, PostListSeriesReq request);
        Task<PagedResults<PostSeriesTopResponse>> GetTopSeriesByPage(PostType type, PostSeriesSelectedType selectedType, TopPostReq loadReq);
        Task<PagedResults<PostSeriesTopResponse>> GetSeriesByTagByPage(PostType type, string tagName, TopPostReq loadReq);
        Task<PagedResults<PostSeriesTopResponse>> GetSeriesByUserByPage(PostType type, string profileName, TopPostReq loadReq);
        Task<List<PostSeriesTopResponse>> GetTopNewSeries(PostType type, int number);
        Task<PostSeriesAllTopResponse> GetTopSeries(PostType type);
        Task<PagedResults<PostSeriesTopResponse>> GetTopSeriesAsync(PostType type, PostListSeriesReq request);
        Task<PagedResults<PostSeriesTopResponse>> GetRelationSeriesAsync(PostType type, RelationPostSeriesReq request);

        Task<SubPost> SubPostChapterToSeries(string hashId, ChapterPostReq chapterPostReq);
        Task<SubPost> SubPostUpdateChapterToSeries(string hashId, int order, ChapterPostReq chapterPostReq);
        Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, ChapterOrderSwapReq orders);
        Task<bool> DeleteChapter(string hashId, int order);
        void VerifyBasicInfo(string title);
        ChapterResponse MappingChapterResponse(SubPost newChapter);
        Task<List<RewardRespone>> CheckRewardsForPost(Guid currentUserId, PostType type);
        Task<List<MyPostSeriesResponse>> GetMyAllSeries();
        Task<bool> ReportPostAsync(ReportPostReq req);
        Task UpdateKeyWordForComicAndStoryToSmartLookup();
        Task<PagedResults<PostBoxResposne>> GetPostByUserProfileName(PostType type, PostByProFileNameInput input);
        Task<PagedResults<PostBoxResposne>> GetPostByTagName(PostType type, PostByTagNameInput input);
        Task<IEnumerable<Guid>> GetPostRandomIdsAsync(GetPostRandomIdsReq req);
        Task<ListIdForHomePage> GetLatestPostsByType();
        Task<ListIdForHomePage> GetLatestPostsByTag(string nameTag);
        Task<List<PostBoxResponse>> GetPostDetails(string hashIds);
    }
}
