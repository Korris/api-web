namespace Mcsg.Comic.Api.Interfaces;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Dtos;
using Models;
using Models.Earning;
using Requests;

public interface IPostService
{
    Task<bool> Delete(IdBaseR request);
    Task<PostSeriesResponse> PostCreate(ComicPostCreateR request);
    Task<PostSeriesResponse> PostUpdate(ComicPostUpdateR request);
    Task<PostSeriesQueryDbResponse> GetSeries(ComicHashIdR req);
    Task<ChapterResponse> GetSeriesChapter(ChapterOrderR req);
    Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, ComicChapterListR request);
    Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(ComicHashIdR hashId);
    Task<PagedResponse<ChapterTOCExtendResponse>> GetChaptersListSimple(Guid userId, string hashId, ComicChapterListR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetMySeries(PostType type, ComicPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopSeriesByPage(PostType type, PostSeriesSelectedType selectedType, ComicTopPostR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByTagByPage(PostType type, string tagName, ComicTopPostR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByUserByPage(PostType type, string profileName, ComicTopPostR loadReq);
    Task<List<PostSeriesTopResponse>> GetTopNewSeries(PostType type, ComicRecommendedR req);
    Task<PostSeriesAllTopResponse> GetTopSeries(PostType type);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopSeriesAsync(PostType type, ComicPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetRelationSeriesAsync(PostType type, ComicRelationPostSeriesR request);

    Task<ChapterResponse> SubPostCreate(ComicSubPostCreateR request);
    Task<ChapterResponse> SubPostUpdate(ComicSubPostUpdateR request);
    Task<List<ChapterResponse>> SwapChapterOrder(string hashId, ComicChapterOrderSwapR orders);
    Task<bool> DeleteChapter(string hashId, float order, BaseR request);
    ChapterResponse MappingChapterResponse(ComicSubPost newChapter);
    Task<List<RewardDto>> CheckRewardsForPost(Guid currentUserId, PostType type);
    Task<List<MyPostSeriesResponse>> GetMyAllSeries(Guid userId);
    Task<PagedResponse<PostBoxResposne>> GetPostByUserProfileName(PostType type, ComicPostByProFileNameR input);
    Task<PagedResponse<PostBoxResposne>> GetPostByTagName(PostType type, ComicPostByTagNameR input);
    Task<IEnumerable<string>> GetPostRandomIdsAsync(PostRandomIdsR req);
    Task<List<PostBoxResponse>> GetPostDetails(PaginatedR req);
    Task<IEnumerable<string>> GetSubPostRandomIdsAsync(PostRandomIdsR input);
    Task<List<NewsFeedDto>> GetNewsFeed(UserNamePagingR input);
    Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(PaginatedR loadReq);
    Task<FavoritePostResponse> FollowPost(IdBaseR request);
    Task<List<RewardDto>> CheckRewardsForSubPost(Guid currentUserId);
    Task MoveChapterOrder(string hashId, ComicChapterOrderSwapR orders);
    Task<List<ChapterList>> GetAllChapters(string hashId);
}
