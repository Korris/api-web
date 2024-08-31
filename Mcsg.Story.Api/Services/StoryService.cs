using Microsoft.EntityFrameworkCore;
using System.Web;

namespace Mcsg.Story.Api.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Requests;
using Common.Domain;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Constants;
using Enums;
using Interfaces;
using Models;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Message;

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

    public async Task<PostSeriesResponse> PostCreate(StoryPostCreateR request)
    {
        return await _postService.PostCreate(_type, request);
    }

    public async Task<PostSeriesResponse> PostUpdate(string hashId, StoryPostUpdateR request)
    {
        return await _postService.PostUpdate(hashId, request);
    }

    public async Task<ChapterResponse> SubPostCreate(string hashId, StorySubPostCreateR request)
    {
        var vr = new StorySubPostCreateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(M109);
        }

        var subPost = await _postService.SubPostChapterToSeries(hashId, request);

        subPost.Body = HttpUtility.HtmlEncode(request.Body);
        await _context.StorySubPosts.AddAsync(subPost);
        await _context.SaveChangesAsync(default);

        var result = _postService.MappingChapterResponse(subPost);
        result.Body = request.Body;

        return result;
    }

    public async Task<ChapterResponse> SubPostUpdate(string hashId, float order, StorySubPostUpdateR request)
    {
        var vr = new StorySubPostUpdateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(M109);
        }

        var subPost = await _postService.SubPostUpdateChapterToSeries(hashId, order, request);
        subPost.Body = HttpUtility.HtmlEncode(request.Body);
        await _context.SaveChangesAsync(default);

        var result = _postService.MappingChapterResponse(subPost);
        result.Body = request.Body;

        return result;
    }

    public async Task<PostSeriesResponse> Get(StoryHashIdR req)
    {
        return await _postService.GetSeries(req);
    }

    public async Task<List<ChapterResponse>> SwapChapterOrder(string hashId, StoryChapterOrderSwapR orders)
    {
        return await _postService.SwapChapterOrder(hashId, orders);
    }

    public async Task<bool> DeleteChapter(string hashId, float order)
    {
        return await _postService.DeleteChapter(hashId, order);
    }

    public async Task<bool> Delete(Guid postId)
    {
        return await _postService.Delete(postId);
    }

    public async Task<ChapterResponse> GetChapter(string hashId, float order)
    {
        return await _postService.GetSeriesChapter(hashId, order);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopHitList(StoryTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.HIT, req);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopLatestList(StoryTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.LATEST, req);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopCompletedList(StoryTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.COMPLETED, req);
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

    public async Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId)
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

    public async Task<bool> FollowPost(Guid postId)
    {
        return await _postService.FollowPost(postId);
    }

    public async Task<float> GetLatestOrderChapter(string hashPostId)
    {
        var postId = await _context.StoryPostAvailable.AsNoTracking()
            .Where(p => p.HashId == hashPostId)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();

        if (postId == Guid.Empty)
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }

        var latestOrder = await _context.StorySubPostAvailable.AsNoTracking()
            .Where(p => p.PostId == postId)
            .OrderByDescending(p => p.Order)
            .Select(p => p.Order)
            .FirstOrDefaultAsync();

        return (int)latestOrder + 1;
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
