using Dapper;
using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Models;
using Mcsg.Social.Api.Services.Interfaces;
using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Exceptions;
using Mcsg.Lib.Common.Interfaces;
using Mcsg.Lib.Common.Web.Security;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Entities.Common;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Mcsg.Lib.Model.Enums;
using MetaData = Mcsg.Lib.Data.Domain.Entities.MetaData;

namespace Mcsg.Social.Api.Services
{
    public partial class FavoriteService : IFavoriteService
    {
        private readonly IRepository<TagFavorite> _tagFavoriteRepository;
        private readonly IRepository<PostFavorite> _postFavoriteRepository;
        private readonly IValidator<TagFavorite> _tagFavoriteValidator;
        private readonly IValidator<PostFavorite> _postFavoriteValidator;

        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<SubPost> _subPostRepository;
        private readonly IRepository<Resource> _resourceRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<MetaData> _metaDataRepository;
        private readonly IRepository<PostLink> _postLinkRepository;
        private readonly IRepository<TagPost> _tagPostRepository;

        private readonly IFeedService _feedService;

        public FavoriteService(
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            IValidator<TagFavorite> tagFavoriteValidator,
            IValidator<PostFavorite> postFavoriteValidator,
            IRepository<TagFavorite> tagFavoriteRepository,
            IRepository<PostFavorite> postFavoriteRepository,
            IRepository<Tag> tagRepository,
            IRepository<Post> postRepository,
            IRepository<SubPost> subPostRepository,
            IRepository<Resource> resourceRepository,
            IRepository<User> userRepository,
            IRepository<MetaData> metaDataRepository,
            IRepository<PostLink> postLinkRepository,
            IRepository<TagPost> tagPostRepository,
            IFeedService feedService)
        {
            _currentUserService = currentUserService;
            _tagFavoriteRepository = tagFavoriteRepository;
            _tagFavoriteValidator = tagFavoriteValidator;
            _postFavoriteRepository = postFavoriteRepository;
            _postFavoriteValidator = postFavoriteValidator;
            _tagRepository = tagRepository;
            _postRepository = postRepository;
            _subPostRepository = subPostRepository;
            _userRepository = userRepository;
            _metaDataRepository = metaDataRepository;
            _postLinkRepository = postLinkRepository;
            _subPostRepository = subPostRepository;
            _resourceRepository = resourceRepository;
            _tagPostRepository = tagPostRepository;
            _feedService = feedService;
        }

        #region Tags
        public async Task<bool> AddTagToFavoriteAsync(Guid tagId)
        {
            var tagFavorite = new TagFavorite
            {
                TagId = tagId,
                UserId = _currentUserService.Session.UserId,
            };
            await _tagFavoriteValidator.OnValidate(tagFavorite);

            var hasExisted = (await _tagFavoriteRepository.GetByCustomQuery(GetTagFavoriteByTagIdAndUserId, new { tagId = tagFavorite.TagId, userId = tagFavorite.UserId })).Any();

            if (hasExisted)
                return true;

            var iResult = await _tagFavoriteRepository.InsertAsync(tagFavorite);
            return iResult > 0;
        }

        public async Task<PagedResults<FavoriteTagResponse>> GetTagFavoriteAsync(FavoriteTagReq req)
        {
            var offset = req.PageSize * (req.PageNumber - 1);
            var multipleQuery = await _tagFavoriteRepository.Connection.
                    QueryMultipleAsync(GetFavoriteTagQuery, new
                    {
                        PageSize = req.PageSize,
                        Offet = offset
                    });

            var items = await multipleQuery.ReadAsync<FavoriteTagResponse>().ConfigureAwait(false);
            var totalItems = await multipleQuery.ReadFirstAsync<int>().ConfigureAwait(false);
            var response = new PagedResults<FavoriteTagResponse>(totalItems, req.PageNumber, req.PageSize);
            response.Items = items;

            return response;
        }

        public async Task<bool> RemoveTagToFavoriteAsync(Guid tagId)
        {
            await _tagFavoriteRepository.Connection.ExecuteAsync(DeleteTagFavoriteByUserIdAndTagIdQuery, new { tagId, userId = _currentUserService.Session.UserId });
            return true;
        }
        #endregion

        #region Post
        public async Task<bool> AddPostToFavoriteAsync(Guid postId)
        {
            var postFavorite = new PostFavorite
            {
                PostId = postId,
                UserId = _currentUserService.Session.UserId,
            };

            await _postFavoriteValidator.OnValidate(postFavorite);

            var hasExisted = (await _postFavoriteRepository.GetByCustomQuery(GetPostFavoriteByPostIdAndUserId, new { postId = postFavorite.PostId, userId = postFavorite.UserId })).Any();

            if (hasExisted)
                return true;

            var iResult = await _postFavoriteRepository.InsertAsync(postFavorite);
            return iResult > 0;
        }

        public async Task<bool> RemovePostToFavoriteAsync(Guid postId)
        {
            await _tagFavoriteRepository.Connection.ExecuteAsync(DeletePostFavoriteByUserIdAndPostIdQuery, new { postId, userId = _currentUserService.Session.UserId });
            return true;
        }

        public async Task<PagedResults<FavoritePostResponse>> GetPostFavoriteAsync(FavoritePostReq req)
        {
            var offset = req.PageSize * (req.PageNumber - 1);
            var multipleQuery = await _postFavoriteRepository.Connection.
                    QueryMultipleAsync(GetFavoritePostQuery, new
                    {
                        PageSize = req.PageSize,
                        Offet = offset
                    });

            var items = await multipleQuery.ReadAsync<FavoritePostResponse>().ConfigureAwait(false);
            var totalItems = await multipleQuery.ReadFirstAsync<int>().ConfigureAwait(false);
            var response = new PagedResults<FavoritePostResponse>(totalItems, req.PageNumber, req.PageSize);
            response.Items = items;

            return response;
        }
        public async Task<PagedResults<FeedResponse>> GetPostFavoriteByUserAsync(FavoritePostReq req)
        {
            try
            {
                PagedResults<FeedResponse> results;
                var userId = _currentUserService.Session.UserId;

                var offset = req.PageSize * (req.PageNumber - 1);
                var multi = await _postRepository
                            .Connection.QueryMultipleAsync(GetFavoritePostByUserQuery, new
                            {
                                @UserId = userId,
                                Type = (int)PostType.FEED,
                                PageSize = req.PageSize,
                                Offet = offset,
                            });
                var items = await multi.ReadAsync<FeedsListQueryDbResponse>().ConfigureAwait(false);
                var listItemResponse = new List<FeedResponse>();
                foreach (var item in items)
                {
                    listItemResponse.Add(_feedService.MappingFeedInListRespone(item));
                }
                var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

                if (items != null && items.Count() > 0)
                {
                    results = new PagedResults<FeedResponse>(totalItems, req.PageNumber, req.PageSize);
                    results.Items = listItemResponse;
                }
                else
                {
                    results = new PagedResults<FeedResponse>(0);
                }

                return results;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
            }
        }
        #endregion
    }
}
