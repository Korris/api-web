namespace Mcsg.Social.Api.Services
{
    using Api.Interfaces;
    using DTOs;
    using Enums;
    using Interfaces;
    using Lib.Common.Helpers;
    using Lib.Common.Web.Security;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Entities.Common;
    using Lib.Data.Repositories;
    using Lib.Data.Repositories.Interface;
    using Lib.Model.Enums;
    using Models;

    public partial class ComicService : IComicService
    {

        private readonly IPostService _postService;
        private readonly PostType _type;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileService _fileService;
        private readonly IRepository<SubPost> _subPostRepository;
        private readonly IConfiguration _configuration;
        public ComicService(
            IPostService postService, IUnitOfWork unitOfWork,
            IFileService fileService,
            ICurrentUserService currentUserService,
            ISetting setting,
            IConfiguration configuration)
        {
            _postService = postService;
            _subPostRepository = unitOfWork.GetRepository<SubPost>();
            _fileService = fileService;
            _currentUserService = currentUserService;
            _type = PostType.COMIC;
            _setting = setting;
            _configuration = configuration;
        }

        #region Load data
        public async Task<PagedResults<PostSeriesTopResponse>> GetTopHitListComic(TopPostReq req)
        {
            return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.HIT, req);
        }
        public async Task<PagedResults<PostSeriesTopResponse>> GetTopLatestListComic(TopPostReq req)
        {
            return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.LATEST, req);
        }
        public async Task<PagedResults<PostSeriesTopResponse>> GetTopCompletedListComic(TopPostReq req)
        {
            return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.COMPLETED, req);
        }
        public async Task<PagedResults<PostSeriesTopResponse>> GetRecommendedComic(TopPostRecommendedReq req)
        {
            return await _postService.GetTopSeriesByPage(_type, PostSeriesSelectedType.COMPLETED, req);
        }
        public async Task<PostSeriesResponse> GetComic(string hashId, bool isLoadChapters)
        {
            return await _postService.GetSeries(hashId, isLoadChapters);
        }
        public async Task<ChapterResponse> GetChapter(string hashId, int order)
        {
            return await _postService.GetSeriesChapter(hashId, order);
        }
        public async Task<PagedResults<PostSeriesTopResponse>> GetMyComics(PostListSeriesReq loadReq)
        {
            return await _postService.GetMySeries(_type, loadReq);
        }
        public async Task<PostSeriesAllTopResponse> GetTopComic()
        {
            return await _postService.GetTopSeries(_type);
        }
        public async Task<PagedResults<PostSeriesTopResponse>> GetTopComicAsync(PostListSeriesReq request)
        {
            return await _postService.GetTopSeriesAsync(_type, request);
        }
        public async Task<PagedResults<PostSeriesTopResponse>> GetRelationComicsAsync(RelationPostSeriesReq request)
        {
            return await _postService.GetRelationSeriesAsync(_type, request);
        }
        public async Task<List<PostSeriesTopResponse>> GetRecommendedComic(int number)
        {
            return await _postService.GetTopNewSeries(_type, number);
        }
        public async Task<PagedResults<ChapterResponse>> GetChapters(string hashId, ChapterListReq request)
        {
            return await _postService.GetChapters(hashId, request);
        }
        public async Task<PagedResults<ChapterTOCResponse>> GetChaptersListSimple(string hashId)
        {
            return await _postService.GetChaptersListSimple(hashId);
        }
        public async Task<PagedResults<PostBoxResposne>> GetComicByUserProfileName(PostByProFileNameInput request)
        {
            return await _postService.GetPostByUserProfileName(_type, request);
        }

        public async Task<PagedResults<PostBoxResposne>> GetComicByTagName(PostByTagNameInput request)
        {
            return await _postService.GetPostByTagName(_type, request);
        }

        #endregion

        #region Modify data
        public async Task<PostSeriesResponse> PostComic(PostSeriesReq comicPostReq)
        {
            return await _postService.PostSeries(_type, comicPostReq);
        }
        public async Task<ChapterResponse> PostChapterToComic(string comicHashId, ChapterComicReq chapterPostReq)
        {
            var currentUserId = _currentUserService.Session.UserId;
            var currentUserName = _currentUserService.Session.UserName;
            var currentUserAvatar = _currentUserService.Session.UserAvatar;
            var currentUserAvatarUrl = string.IsNullOrEmpty(currentUserAvatar) ? string.Empty : UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, currentUserAvatar);

            _postService.VerifyBasicInfo(chapterPostReq.Title);

            var subPost = await _postService.SubPostChapterToSeries(comicHashId, chapterPostReq);
            await _subPostRepository.InsertAsync(subPost);

            var result = _postService.MappingChapterResponse(subPost);
            if (chapterPostReq.Files != null && chapterPostReq?.Files.Count > 0)
            {
                result.Files = await _fileService.ProcessComicFilesAsync(chapterPostReq.Files, currentUserId, currentUserName, currentUserAvatarUrl, subPost.Id);
            }

            return result;

        }
        public async Task<ChapterResponse> UpdateChapterToComic(string comicHashId, int order, ChapterComicReq chapterPostReq)
        {
            var currentUserId = _currentUserService.Session.UserId;
            var currentUserName = _currentUserService.Session.UserName;
            var currentUserAvatar = _currentUserService.Session.UserAvatar;
            var currentUserAvatarUrl = string.IsNullOrEmpty(currentUserAvatar) ? string.Empty : UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, currentUserAvatar);
            _postService.VerifyBasicInfo(chapterPostReq.Title);

            var subPost = await _postService.SubPostUpdateChapterToSeries(comicHashId, order, chapterPostReq);
            //subPost.CreatorNote = chapterPostReq.CreatorNote;

            await _subPostRepository.UpdateAsync(subPost);

            var result = _postService.MappingChapterResponse(subPost);
            if (chapterPostReq.Files != null && chapterPostReq?.Files.Count > 0)
            {
                result.Files = await _fileService.ProcessComicFilesAsync(chapterPostReq.Files, currentUserId, currentUserName, currentUserAvatarUrl, subPost.Id);
            }

            return result;

        }
        public async Task<bool> DeleteChapter(string comicHashId, int order)
        {
            return await _postService.DeleteChapter(comicHashId, order);
        }
        public async Task<PostSeriesResponse> UpdateComic(string hashId, PostUpdateSeriesReq comicPostReq)
        {
            return await _postService.UpdateSeries(hashId, comicPostReq);
        }
        public async Task<bool> Delete(Guid postId)
        {
            return await _postService.Delete(postId);
        }
        public async Task<List<ChapterResponse>> SwapChapterOrder(string comicHashId, ChapterOrderSwapReq orders)
        {
            return await _postService.SwapChapterOrder(comicHashId, orders);
        }
        #endregion

        #region -- Fields --

        /// <summary>
        /// Setting
        /// </summary>
        private readonly ISetting _setting;

        #endregion
    }
}
