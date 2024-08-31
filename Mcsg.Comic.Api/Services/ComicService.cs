using Microsoft.EntityFrameworkCore;

namespace Mcsg.Comic.Api.Services;

using Api.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Dtos;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Enums;
using Interfaces;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Message;

public partial class ComicService : IComicService
{
    private readonly IPostService _postService;
    private readonly PostType _type;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileService _fileService;
    private readonly IRepository<ComicSubPost> _subPostRepository;
    private readonly IConfiguration _configuration;
    private readonly IMcsgContext _context;
    public ComicService(
        IPostService postService, IUnitOfWork unitOfWork,
        IFileService fileService,
        ICurrentUserService currentUserService,
        ISetting setting,
        IConfiguration configuration,
        IMcsgContext context)
    {
        _postService = postService;
        _subPostRepository = unitOfWork.GetRepository<ComicSubPost>();
        _fileService = fileService;
        _currentUserService = currentUserService;
        _type = PostType.Comic;
        _setting = setting;
        _configuration = configuration;
        _context = context;
    }

    #region Load data
    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopHitListComic(ComicTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.HIT, req);
    }
    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopLatestListComic(ComicTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.LATEST, req);
    }
    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopCompletedListComic(ComicTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.COMPLETED, req);
    }
    public async Task<PagedResponse<PostSeriesTopResponse>> GetRecommendedComic(ComicTopPostRecommendedR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.COMPLETED, req);
    }
    public async Task<PostSeriesResponse> GetComic(ComicHashIdR req)
    {
        return await _postService.GetSeries(req);
    }
    public async Task<ChapterResponse> GetChapter(string hashId, float order)
    {
        return await _postService.GetSeriesChapter(hashId, order);
    }
    public async Task<PagedResponse<PostSeriesTopResponse>> GetMyComics(ComicPostListSeriesR loadReq)
    {
        return await _postService.GetMySeries(_type, loadReq);
    }
    public async Task<PostSeriesAllTopResponse> GetTopComic()
    {
        return await _postService.GetTopSeries(_type);
    }
    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopComicAsync(ComicPostListSeriesR request)
    {
        return await _postService.GetTopSeriesAsync(_type, request);
    }
    public async Task<PagedResponse<PostSeriesTopResponse>> GetRelationComicsAsync(ComicRelationPostSeriesR request)
    {
        return await _postService.GetRelationSeriesAsync(_type, request);
    }
    public async Task<List<PostSeriesTopResponse>> GetRecommendedComic(ComicRecommendedR req)
    {
        return await _postService.GetTopNewSeries(_type, req);
    }
    public async Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, ComicChapterListR request)
    {
        return await _postService.GetChapters(hashId, request);
    }
    public async Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId)
    {
        return await _postService.GetChaptersListSimple(hashId);
    }
    public async Task<PagedResponse<PostBoxResposne>> GetComicByUserProfileName(ComicPostByProFileNameR request)
    {
        return await _postService.GetPostByUserProfileName(_type, request);
    }

    public async Task<PagedResponse<PostBoxResposne>> GetComicByTagName(ComicPostByTagNameR request)
    {
        return await _postService.GetPostByTagName(_type, request);
    }

    #endregion

    #region Modify data
    public async Task<PostSeriesResponse> PostCreate(ComicPostCreateR request)
    {
        return await _postService.PostCreate(_type, request);
    }

    public async Task<ChapterResponse> SubPostCreate(string comicHashId, ComicSubPostCreateR request)
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

        var subPost = await _postService.SubPostChapterToSeries(comicHashId, request);
        await _subPostRepository.InsertAsync(subPost);

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

    public async Task<ChapterResponse> SubPostUpdate(string comicHashId, float order, ComicSubPostUpdateR request)
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

        var subPost = await _postService.SubPostUpdateChapterToSeries(comicHashId, order, request);
        //subPost.CreatorNote = chapterPostReq.CreatorNote;

        await _subPostRepository.UpdateAsync(subPost);

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
    public async Task<bool> DeleteChapter(string comicHashId, float order)
    {
        return await _postService.DeleteChapter(comicHashId, order);
    }
    public async Task<PostSeriesResponse> PostUpdate(string hashId, ComicPostUpdateR request)
    {
        return await _postService.PostUpdate(hashId, request);
    }
    public async Task<bool> Delete(Guid postId)
    {
        return await _postService.Delete(postId);
    }
    public async Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, ComicChapterOrderSwapR orders)
    {
        return await _postService.SwapChapterOrder(comicHashId, orders);
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

    #endregion

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
