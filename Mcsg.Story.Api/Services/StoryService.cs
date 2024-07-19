namespace Mcsg.Story.Api.Services;

using Common.Core.Enums;
using Common.SeedWork.Responses;
using Enums;
using Interfaces;
using Lib.Common.Web.Security;
using Lib.Data.Domain.Entities;
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
    public StoryService(
        IUnitOfWork unitOfWork,
        IPostService postService,
        ICurrentUserService currentUserService)
    {
        _subPostRepository = unitOfWork.GetRepository<StorySubPost>();
        _postService = postService;
        _type = PostType.Story;
        _currentUserService = currentUserService;
    }
    public async Task<PostSeriesResponse> PostStory(ComicPostSeriesR comicPostReq)
    {
        return await _postService.PostSeries(_type, comicPostReq);
    }

    public async Task<PostSeriesResponse> GetStory(string hashId, bool isLoadChapters)
    {
        return await _postService.GetSeries(hashId, isLoadChapters);
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
    public async Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, ComicChapterOrderSwapR orders)
    {
        return await _postService.SwapChapterOrder(comicHashId, orders);
    }
    public async Task<bool> DeleteChapter(string comicHashId, int order)
    {
        return await _postService.DeleteChapter(comicHashId, order);
    }
    public async Task<PostSeriesResponse> UpdateStory(string hashId, ComicPostUpdateSeriesR comicPostReq)
    {
        return await _postService.UpdateSeries(hashId, comicPostReq);
    }
    public async Task<bool> Delete(Guid postId)
    {
        return await _postService.Delete(postId);
    }

    public async Task<ChapterResponse> GetChapter(string hashId, int order)
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
    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopStoryAsync(ComicPostListSeriesR request)
    {
        return await _postService.GetTopSeriesAsync(_type, request);
    }
    public async Task<PagedResponse<PostSeriesTopResponse>> GetRelationStoriesAsync(ComicRelationPostSeriesR request)
    {
        return await _postService.GetRelationSeriesAsync(_type, request);
    }
    public async Task<PagedResponse<PostSeriesTopResponse>> GetMyStories(ComicPostListSeriesR loadReq)
    {
        return await _postService.GetMySeries(_type, loadReq);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopHitListStory(ComicTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.HIT, req);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopLatestListStory(ComicTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.LATEST, req);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopCompletedListStory(ComicTopPostR req)
    {
        return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.COMPLETED, req);
    }

    public async Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, ComicChapterListR request)
    {
        return await _postService.GetChapters(hashId, request);
    }

    public async Task<PagedResponse<PostBoxResposne>> GetStoryByUserProfileName(ComicPostByProFileNameR request)
    {
        return await _postService.GetPostByUserProfileName(_type, request);
    }

    public async Task<PagedResponse<PostBoxResposne>> GetStoryByTagName(ComicPostByTagNameR request)
    {
        return await _postService.GetPostByTagName(_type, request);
    }
}
