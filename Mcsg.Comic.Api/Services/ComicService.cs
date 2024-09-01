using Microsoft.EntityFrameworkCore;

namespace Mcsg.Comic.Api.Services;

using Api.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Dtos;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Enums;
using Interfaces;
using Models;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Message;

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

    public async Task<ChapterResponse> SubPostCreate(string hashId, ComicSubPostCreateR request)
    {
        var vr = new ComicSubPostCreateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(M109);
        }

        var userId = request.UserId.Value;
        var userFolder = request.UserFolder;
        var userAvatar = request.UserAvatar;
        var userName = request.UserName;

        var subPost = await _postService.SubPostCreate(hashId, request);
        await _context.ComicSubPosts.AddAsync(subPost);
        await _context.SaveChangesAsync(default);

        var result = _postService.MappingChapterResponse(subPost);
        if (request?.Files.Count > 0)
        {
            var urDto = new UploadResourceDto(request.Files, userId, userFolder, userAvatar, userName, subPost.PostId, subPost.PostHashId)
            {
                SubPostId = subPost.Id,
                Order = subPost.Order
            };
            result.Files = await _fileService.ProcessComicFilesAsync(urDto);
        }

        return result;
    }

    public async Task<ChapterResponse> SubPostUpdate(string hashId, float order, ComicSubPostUpdateR request)
    {
        var vr = new ComicSubPostUpdateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(M109);
        }

        var userId = request.UserId.Value;
        var userFolder = request.UserFolder;
        var userAvatar = request.UserAvatar;
        var userName = request.UserName;

        var subPost = await _postService.SubPostUpdate(hashId, order, request);
        await _context.SaveChangesAsync(default);

        var result = _postService.MappingChapterResponse(subPost);
        if (request?.Files.Count > 0)
        {
            var urDto = new UploadResourceDto(request.Files, userId, userFolder, userAvatar, userName, subPost.PostId, subPost.PostHashId)
            {
                SubPostId = subPost.Id,
                Order = subPost.Order
            };
            result.Files = await _fileService.ProcessComicFilesAsync(urDto);
        }

        return result;
    }

    public async Task<PostSeriesResponse> Get(ComicHashIdR req)
    {
        return await _postService.GetSeries(req);
    }

    public async Task<List<ChapterResponse>> SwapChapterOrder(string hashId, ComicChapterOrderSwapR orders)
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

    public async Task<bool> FollowPost(Guid postId)
    {
        return await _postService.FollowPost(postId);
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
