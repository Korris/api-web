namespace Mcsg.Comic.Api.Interfaces;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Dtos;
using Enums;
using Models;
using Models.Earning;
using Requests;

public interface IPostService
{
    Task<bool> Delete(Guid postId);
    Task<PostSeriesResponse> PostCreate(ComicPostCreateR request);
    Task<PostSeriesResponse> PostUpdate(ComicPostUpdateR request);
    Task<PostSeriesResponse> GetSeries(ComicHashIdR req);
    Task<ChapterResponse> GetSeriesChapter(string hashId, float order);
    Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, ComicChapterListR request);
    Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
    Task<PagedResponse<ChapterTOCExtendResponse>> GetChaptersListSimple(Guid userId, string hashId, ComicChapterListR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetMySeries(PostType type, ComicPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopSeriesByPage(PostType type, PostSeriesSelectedType selectedType, ComicTopPostR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByTagByPage(PostType type, string tagName, ComicTopPostR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByUserByPage(PostType type, string profileName, ComicTopPostR loadReq);
    Task<List<PostSeriesTopResponse>> GetTopNewSeries(PostType type, ComicRecommendedR req);
    Task<PostSeriesAllTopResponse> GetTopSeries(PostType type);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopSeriesAsync(PostType type, ComicPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetRelationSeriesAsync(PostType type, ComicRelationPostSeriesR request);

    Task<ComicSubPost> SubPostCreate(string hashId, ComicSubPostCreateR request);
    Task<ComicSubPost> SubPostUpdate(string hashId, float order, ComicSubPostUpdateR request);
    Task<List<ChapterResponse>> SwapChapterOrder(string hashId, ComicChapterOrderSwapR orders);
    Task<bool> DeleteChapter(string hashId, float order);
    ChapterResponse MappingChapterResponse(ComicSubPost newChapter);
    Task<List<RewardDto>> CheckRewardsForPost(Guid currentUserId, PostType type);
    Task<List<MyPostSeriesResponse>> GetMyAllSeries();
    Task<PagedResponse<PostBoxResposne>> GetPostByUserProfileName(PostType type, ComicPostByProFileNameR input);
    Task<PagedResponse<PostBoxResposne>> GetPostByTagName(PostType type, ComicPostByTagNameR input);
    Task<IEnumerable<string>> GetPostRandomIdsAsync(PostRandomIdsR req);
    Task<ListIdForHomePage> GetLatestPostsByType();
    Task<ListIdForHomePage> GetLatestPostsByTag(string nameTag);
    Task<List<PostBoxResponse>> GetPostDetails(ComicHashIdsR req);
    Task<IEnumerable<string>> GetSubPostRandomIdsAsync(PostRandomIdsR input);
    Task<List<NewsFeedDto>> GetNewsFeed(UserNamePagingR input);
    Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(PaginatedR loadReq);
    Task<bool> FollowPost(Guid postId);
}
