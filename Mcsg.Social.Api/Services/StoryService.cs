using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Enums;
using Mcsg.Social.Api.Models;
using Mcsg.Social.Api.Services.Interfaces;
using Mcsg.Lib.Common.Web.Security;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Entities.Common;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Mcsg.Lib.Model.Enums;

namespace Mcsg.Social.Api.Services
{
    public partial class StoryService : IStoryService
    {
        private readonly IPostService _postService;
        private readonly PostType _type;
        private readonly IRepository<SubPost> _subPostRepository;
        private readonly ICurrentUserService _currentUserService;
        public StoryService(
            IUnitOfWork unitOfWork,
            IPostService postService,
            ICurrentUserService currentUserService)
        {
            _subPostRepository = unitOfWork.GetRepository<SubPost>();
            _postService = postService;
            _type = PostType.STORY;
            _currentUserService = currentUserService;
        }
        public async Task<PostSeriesResponse> PostStory(PostSeriesReq comicPostReq)
        {
            return await _postService.PostSeries(_type, comicPostReq);
        }

        public async Task<PostSeriesResponse> GetStory(string hashId, bool isLoadChapters)
        {
            return await _postService.GetSeries(hashId, isLoadChapters);
        }

        public async Task<ChapterResponse> PostChapterToStory(string comicHashId, ChapterStoryReq chapterPostReq)
        {
            _postService.VerifyBasicInfo(chapterPostReq.Title);

            var subPost = await _postService.SubPostChapterToSeries(comicHashId, chapterPostReq);

            subPost.Body = System.Web.HttpUtility.HtmlEncode(chapterPostReq.Body);
            await _subPostRepository.InsertAsync(subPost);

            var result = _postService.MappingChapterResponse(subPost);
            result.Body = chapterPostReq.Body;

            return result;

        }
        public async Task<ChapterResponse> UpdateChapterToStory(string comicHashId, int order, ChapterStoryReq chapterPostReq)
        {
            _postService.VerifyBasicInfo(chapterPostReq.Title);

            var subPost = await _postService.SubPostUpdateChapterToSeries(comicHashId, order, chapterPostReq);
            subPost.Body = System.Web.HttpUtility.HtmlEncode(chapterPostReq.Body);

            await _subPostRepository.UpdateAsync(subPost);

            var result = _postService.MappingChapterResponse(subPost);
            result.Body = chapterPostReq.Body;

            return result;

        }
        public async Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, ChapterOrderSwapReq orders)
        {
            return await _postService.SwapChapterOrder(comicHashId, orders);
        }
        public async Task<bool> DeleteChapter(string comicHashId, int order)
        {
            return await _postService.DeleteChapter(comicHashId, order);
        }
        public async Task<PostSeriesResponse> UpdateStory(string hashId, PostUpdateSeriesReq comicPostReq)
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
        public async Task<PagedResults<ChapterTOCResponse>> GetChaptersListSimple(string hashId)
        {
            return await _postService.GetChaptersListSimple(hashId);
        }
        public async Task<PostSeriesAllTopResponse> GetTopStory()
        {
            return await _postService.GetTopSeries(_type);
        }
        public async Task<PagedResults<PostSeriesTopResponse>> GetTopStoryAsync(PostListSeriesReq request)
        {
            return await _postService.GetTopSeriesAsync(_type, request);
        }
        public async Task<PagedResults<PostSeriesTopResponse>> GetRelationStoriesAsync(RelationPostSeriesReq request)
        {
            return await _postService.GetRelationSeriesAsync(_type, request);
        }
        public async Task<PagedResults<PostSeriesTopResponse>> GetMyStories(PostListSeriesReq loadReq)
        {
            return await _postService.GetMySeries(_type, loadReq);
        }

        public async Task<PagedResults<PostSeriesTopResponse>> GetTopHitListStory(TopPostReq req)
        {
            return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.HIT, req);
        }

        public async Task<PagedResults<PostSeriesTopResponse>> GetTopLatestListStory(TopPostReq req)
        {
            return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.LATEST, req);
        }

        public async Task<PagedResults<PostSeriesTopResponse>> GetTopCompletedListStory(TopPostReq req)
        {
            return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.COMPLETED, req);
        }

        public async Task<PagedResults<ChapterResponse>> GetChapters(string hashId, ChapterListReq request)
        {
            return await _postService.GetChapters(hashId, request);
        }
    }
}
