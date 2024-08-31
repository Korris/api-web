using Microsoft.EntityFrameworkCore;

namespace Mcsg.Story.Api.Services;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Constants;
using Enums;
using Interfaces;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;
using Requests;

public partial class StoryService : IStoryService
{
    private readonly IPostService _postService;
    private readonly PostType _type;
    private readonly IRepository<StorySubPost> _subPostRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMcsgContext _context;
    public StoryService(
        IUnitOfWork unitOfWork,
        IPostService postService,
        ICurrentUserService currentUserService,
        IMcsgContext context)
    {
        _subPostRepository = unitOfWork.GetRepository<StorySubPost>();
        _postService = postService;
        _type = PostType.Story;
        _currentUserService = currentUserService;
        _context = context;
    }
    public async Task<PostSeriesResponse> PostStory(StoryPostSeriesR request)
    {
        return await _postService.PostSeries(_type, request);
    }

    public async Task<PostSeriesResponse> GetStory(StoryHashIdR req)
    {
        return await _postService.GetSeries(req);
    }

    public async Task<ChapterResponse> PostChapterToStory(string comicHashId, StoryChapterR chapterPostReq)
    {
        _postService.VerifyBasicInfo(chapterPostReq.Title);

        var subPost = await _postService.SubPostChapterToSeries(comicHashId, chapterPostReq);

        subPost.Body = System.Web.HttpUtility.HtmlEncode(chapterPostReq.Body);
        await _subPostRepository.InsertAsync(subPost);

        var result = _postService.MappingChapterResponse(subPost);
        result.Body = chapterPostReq.Body;

        return result;

    }
    public async Task<ChapterResponse> UpdateChapterToStory(string comicHashId, int order, StoryChapterR chapterPostReq)
    {
        _postService.VerifyBasicInfo(chapterPostReq.Title);

        var subPost = await _postService.SubPostUpdateChapterToSeries(comicHashId, order, chapterPostReq);
        subPost.Body = System.Web.HttpUtility.HtmlEncode(chapterPostReq.Body);

        await _subPostRepository.UpdateAsync(subPost);

        var result = _postService.MappingChapterResponse(subPost);
        result.Body = chapterPostReq.Body;

        return result;

    }
    public async Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, StoryChapterOrderSwapR orders)
    {
        return await _postService.SwapChapterOrder(comicHashId, orders);
    }
    public async Task<bool> DeleteChapter(string comicHashId, int order)
    {
        return await _postService.DeleteChapter(comicHashId, order);
    }
    public async Task<PostSeriesResponse> UpdateStory(string hashId, StoryPostUpdateSeriesR comicPostReq)
    {
        return await _postService.UpdateSeries(hashId, comicPostReq);
    }
    public async Task<bool> Delete(Guid postId)
    {
        return await _postService.Delete(postId);
    }

    public async Task<ChapterResponse> GetChapter(string hashId, float order)
    {
        return await _postService.GetSeriesChapter(hashId, order);
    }
    public async Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId)
    {
        return await _postService.GetChaptersListSimple(hashId);
    }
    public async Task<PostSeriesAllTopResponse> GetTopStory()
    {
        return await _postService.GetTopSeries(_type);
    }
    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopStoryAsync(StoryPostListSeriesR request)
    {
        return await _postService.GetTopSeriesAsync(_type, request);
    }
    public async Task<PagedResponse<PostSeriesTopResponse>> GetRelationStoriesAsync(StoryRelationPostSeriesR request)
    {
        return await _postService.GetRelationSeriesAsync(_type, request);
    }
    public async Task<PagedResponse<PostSeriesTopResponse>> GetMyStories(StoryPostListSeriesR loadReq)
    {
        return await _postService.GetMySeries(_type, loadReq);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopHitListStory(StoryTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.HIT, req);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopLatestListStory(StoryTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.LATEST, req);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopCompletedListStory(StoryTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.COMPLETED, req);
    }

    public async Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, StoryChapterListR request)
    {
        return await _postService.GetChapters(hashId, request);
    }

    public async Task<PagedResponse<PostBoxResposne>> GetStoryByUserProfileName(StoryPostByProFileNameR request)
    {
        return await _postService.GetPostByUserProfileName(_type, request);
    }

    public async Task<PagedResponse<PostBoxResposne>> GetStoryByTagName(StoryPostByTagNameR request)
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
}
