namespace Mcsg.Document.Api.Interfaces;

using Common.Core.Requests;
using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IDocumentService
{
    Task<PostSeriesResponse> Get(DocumentHashIdR req);
    Task<List<ChapterResponse>> SwapChapterOrder(string hashId, DocumentChapterOrderSwapR orders);
    Task<ChapterResponse> GetChapter(ChapterOrderR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopHitList(DocumentTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopLatestList(DocumentTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetTopCompletedList(DocumentTopPostR req);
    Task<PagedResponse<PostSeriesTopResponse>> GetMy(DocumentPostListSeriesR loadReq);
    Task<PostSeriesAllTopResponse> GetTop();
    Task<PagedResponse<PostSeriesTopResponse>> GetTopAsync(DocumentPostListSeriesR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetRelationAsync(DocumentRelationPostSeriesR request);
    Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, DocumentChapterListR request);
    Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId);
    Task<PagedResponse<PostBoxResposne>> GetByUserProfileName(DocumentPostByProFileNameR request);
    Task<PagedResponse<PostBoxResposne>> GetByTagName(DocumentPostByTagNameR request);
    Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(PaginatedR input);
    Task<bool> FollowPost(IdBaseR request);
    Task<float> GetLatestOrderChapter(string hashPostId);
    Task<List<PostSeriesTopResponse>> GetRecommended(DocumentRecommendedR req);
    Task MoveChapterOrder(string hashId, DocumentChapterOrderSwapR orders);
    Task<List<ChapterList>> GetAllChapters(string hashId);
}
