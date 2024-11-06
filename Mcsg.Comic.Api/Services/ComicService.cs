using Microsoft.EntityFrameworkCore;

namespace Mcsg.Comic.Api.Services;

using Api.Constants;
using Common.Core.Enums;
using Common.Core.Requests;
using Common.Domain;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Enums;
using Interfaces;
using Models;
using Requests;

public partial class ComicService : IComicService
{
    #region -- Methods --

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="fileService"></param>
    /// <param name="postService"></param>
    public ComicService(IMcsgContext context, IFileService fileService, IPostService postService)
    {
        _context = context;
        _type = PostType.Comic;
        _fileService = fileService;
        _postService = postService;
    }

    public async Task<PostSeriesResponse> Get(ComicHashIdR req)
    {
        return await _postService.GetSeries(req);
    }

    public async Task<List<ChapterResponse>> SwapChapterOrder(string hashId, ComicChapterOrderSwapR orders)
    {
        return await _postService.SwapChapterOrder(hashId, orders);
    }

    public async Task<ChapterResponse> GetChapter(ChapterOrderR req)
    {
        return await _postService.GetSeriesChapter(req);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopHitList(ComicTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.HIT, req);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopLatestList(ComicTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.LATEST, req);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopCompletedList(ComicTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.COMPLETED, req);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetMy(ComicPostListSeriesR loadReq)
    {
        return await _postService.GetMySeries(_type, loadReq);
    }

    public async Task<PostSeriesAllTopResponse> GetTop()
    {
        return await _postService.GetTopSeries(_type);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopAsync(ComicPostListSeriesR request)
    {
        return await _postService.GetTopSeriesAsync(_type, request);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetRelationAsync(ComicRelationPostSeriesR request)
    {
        return await _postService.GetRelationSeriesAsync(_type, request);
    }

    public async Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, ComicChapterListR request)
    {
        return await _postService.GetChapters(hashId, request);
    }

    public async Task<List<ChapterList>> GetAllChapters(string hashId)
    {
        return await _postService.GetAllChapters(hashId);
    }

    public async Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId)
    {
        return await _postService.GetChaptersListSimple(hashId);
    }

    public async Task<PagedResponse<PostBoxResposne>> GetByUserProfileName(ComicPostByProFileNameR request)
    {
        return await _postService.GetPostByUserProfileName(_type, request);
    }

    public async Task<PagedResponse<PostBoxResposne>> GetByTagName(ComicPostByTagNameR request)
    {
        return await _postService.GetPostByTagName(_type, request);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(PaginatedR input)
    {
        return await _postService.GetFollowedPost(input);
    }

    public async Task<bool> FollowPost(IdBaseR request)
    {
        return await _postService.FollowPost(request);
    }

    public async Task<float> GetLatestOrderChapter(string hashPostId)
    {
        var postId = await _context.ComicPostAvailable.AsNoTracking()
            .Where(p => p.HashId == hashPostId)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();

        if (postId == Guid.Empty)
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }

        var latestOrder = await _context.ComicSubPostAvailable.AsNoTracking()
            .Where(p => p.PostId == postId)
            .OrderByDescending(p => p.Order)
            .Select(p => p.Order)
            .FirstOrDefaultAsync();

        return (int)latestOrder + 1;
    }

    public async Task MoveChapterOrder(string hashId, ComicChapterOrderSwapR orders)
    {
        await _postService.MoveChapterOrder(hashId, orders);
    }

    public async Task<List<PostSeriesTopResponse>> GetRecommended(ComicRecommendedR req)
    {
        return await _postService.GetTopNewSeries(_type, req);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    private readonly PostType _type;
    private readonly IFileService _fileService;
    private readonly IPostService _postService;

    #endregion
}
