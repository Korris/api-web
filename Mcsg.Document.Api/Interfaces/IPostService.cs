namespace Mcsg.Document.Api.Interfaces;

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
    Task<PostSeriesResponse> PostCreate(DocumentPostCreateR request);
    Task<PostSeriesResponse> PostUpdate(DocumentPostUpdateR request);
    Task<PostSeriesResponse> GetSeries(DocumentHashIdR req);
    Task<ChapterResponse> GetSeriesChapter(ChapterOrderR req);
    Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, DocumentChapterListR request);
    Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
    Task<PagedResponse<ChapterTOCExtendResponse>> GetChaptersListSimple(Guid userId, string hashId, DocumentChapterListR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetMySeries(PostType type, DocumentPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopSeriesByPage(PostType type, PostSeriesSelectedType selectedType, DocumentTopPostR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByTagByPage(PostType type, string tagName, DocumentTopPostR loadReq);
    Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByUserByPage(PostType type, string profileName, DocumentTopPostR loadReq);
    Task<List<PostSeriesTopResponse>> GetTopNewSeries(PostType type, DocumentRecommendedR req);
    Task<PostSeriesAllTopResponse> GetTopSeries(PostType type);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopSeriesAsync(PostType type, DocumentPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetRelationSeriesAsync(PostType type, DocumentRelationPostSeriesR request);

    Task<ChapterResponse> SubPostCreate(DocumentSubPostCreateR request);
    Task<ChapterResponse> SubPostUpdate(DocumentSubPostUpdateR request);
    Task<List<ChapterResponse>> SwapChapterOrder(string hashId, DocumentChapterOrderSwapR orders);
    Task<bool> DeleteChapter(string hashId, float order, BaseR request);
    ChapterResponse MappingChapterResponse(DocumentSubPost newChapter);
    Task<List<RewardDto>> CheckRewardsForPost(Guid currentUserId, PostType type);
    Task<List<MyPostSeriesResponse>> GetMyAllSeries(Guid userId);
    Task<PagedResponse<PostBoxResposne>> GetPostByUserProfileName(PostType type, DocumentPostByProFileNameR input);
    Task<PagedResponse<PostBoxResposne>> GetPostByTagName(PostType type, DocumentPostByTagNameR input);
    Task<IEnumerable<string>> GetPostRandomIdsAsync(PostRandomIdsR req);
    Task<List<PostBoxResponse>> GetPostDetails(PaginatedR req);
    Task<IEnumerable<string>> GetSubPostRandomIdsAsync(PostRandomIdsR input);
    Task<List<NewsFeedDto>> GetNewsFeed(UserNamePagingR input);
    Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(PaginatedR loadReq);
    Task<FavoritePostResponse> FollowPost(IdBaseR request);
    Task<List<RewardDto>> CheckRewardsForSubPost(Guid currentUserId);
    Task MoveChapterOrder(string hashId, DocumentChapterOrderSwapR orders);
    Task<List<ChapterList>> GetAllChapters(string hashId);
}
