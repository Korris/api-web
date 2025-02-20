using Microsoft.EntityFrameworkCore;

namespace Mcsg.Story.Api.Services;

using Api.Constants;
using Common.Core.Enums;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Interfaces;
using Models;
using Requests;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Validator;

public partial class StoryService : IStoryService
{
    #region -- Methods --

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="postService"></param>
    public StoryService(IMcsgContext context, IPostService postService)
    {
        _context = context;
        _type = PostType.Story;
        _postService = postService;
    }

    public async Task<PostSeriesQueryDbResponse> Get(StoryHashIdR req)
    {
        return await _postService.GetSeries(req);
    }

    public async Task<List<ChapterResponse>> SwapChapterOrder(string hashId, StoryChapterOrderSwapR orders)
    {
        return await _postService.SwapChapterOrder(hashId, orders);
    }

    public async Task<ChapterResponse> GetChapter(ChapterOrderR req)
    {
        return await _postService.GetSeriesChapter(req);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopHitList(StoryTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.Hit, req);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopLatestList(StoryTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.Latest, req);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopCompletedList(StoryTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.Completed, req);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetMy(StoryPostListSeriesR loadReq)
    {
        return await _postService.GetMySeries(_type, loadReq);
    }

    public async Task<PostSeriesAllTopResponse> GetTop()
    {
        return await _postService.GetTopSeries(_type);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopAsync(StoryPostListSeriesR request)
    {
        return await _postService.GetTopSeriesAsync(_type, request);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetRelationAsync(StoryRelationPostSeriesR request)
    {
        return await _postService.GetRelationSeriesAsync(_type, request);
    }

    public async Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, StoryChapterListR request)
    {
        return await _postService.GetChapters(hashId, request);
    }

    public async Task<List<ChapterList>> GetAllChapters(string hashId)
    {
        return await _postService.GetAllChapters(hashId);
    }

    public async Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(StoryHashIdR hashId)
    {
        return await _postService.GetChaptersListSimple(hashId);
    }

    public async Task<PagedResponse<PostBoxResposne>> GetByUserProfileName(StoryPostByProFileNameR request)
    {
        return await _postService.GetPostByUserProfileName(_type, request);
    }

    public async Task<PagedResponse<PostBoxResposne>> GetByTagName(StoryPostByTagNameR request)
    {
        return await _postService.GetPostByTagName(_type, request);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(PaginatedR input)
    {
        return await _postService.GetFollowedPost(input);
    }

    public async Task<FavoritePostResponse> FollowPost(IdBaseR request)
    {
        return await _postService.FollowPost(request);
    }

    public async Task<float> GetLatestOrderChapter(string hashPostId)
    {
        var postId = await _context.Available<StoryPost>()
            .Where(p => p.HashId == hashPostId)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();

        if (postId == Guid.Empty)
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }

        var latestOrder = await _context.Available<StorySubPost>()
            .Where(p => p.PostId == postId)
            .OrderByDescending(p => p.Order)
            .Select(p => p.Order)
            .FirstOrDefaultAsync();

        if (latestOrder >= ChapterRange.Max)
        {
            throw new BadRequestException(nameof(E602), E602);
        }

        return (int)latestOrder + 1;
    }

    public async Task MoveChapterOrder(string hashId, StoryChapterOrderSwapR orders)
    {
        await _postService.MoveChapterOrder(hashId, orders);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    private readonly PostType _type;
    private readonly IPostService _postService;

    #endregion
}
