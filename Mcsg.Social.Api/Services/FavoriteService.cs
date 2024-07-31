using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Services;

using Common.Core.Enums;
using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Dtos;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Interfaces;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;
using Requests;

public partial class FavoriteService : IFavoriteService
{
    private readonly IRepository<TagFavorite> _tagFavoriteRepository;
    private readonly IRepository<SocialPostFavorite> _postFavoriteRepository;
    private readonly IValidator<TagFavorite> _tagFavoriteValidator;
    private readonly IValidator<SocialPostFavorite> _postFavoriteValidator;

    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<Tag> _tagRepository;
    private readonly IRepository<SocialPost> _postRepository;
    private readonly IRepository<SocialSubPost> _subPostRepository;
    private readonly IRepository<SocialResource> _resourceRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<SocialMetaData> _metaDataRepository;
    private readonly IRepository<SocialPostLink> _postLinkRepository;
    private readonly IRepository<SocialTagPost> _tagPostRepository;

    private readonly IFeedService _feedService;

    public FavoriteService(
        IMcsgContext context,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IValidator<TagFavorite> tagFavoriteValidator,
        IValidator<SocialPostFavorite> postFavoriteValidator,
        IRepository<TagFavorite> tagFavoriteRepository,
        IRepository<SocialPostFavorite> postFavoriteRepository,
        IRepository<Tag> tagRepository,
        IRepository<SocialPost> postRepository,
        IRepository<SocialSubPost> subPostRepository,
        IRepository<SocialResource> resourceRepository,
        IRepository<User> userRepository,
        IRepository<SocialMetaData> metaDataRepository,
        IRepository<SocialPostLink> postLinkRepository,
        IRepository<SocialTagPost> tagPostRepository,
        IFeedService feedService)
    {
        _context = context;
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

    public async Task<PagedResponse<FavoriteTagResponse>> GetTagFavoriteAsync(FavoriteTagR req)
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
        var response = new PagedResponse<FavoriteTagResponse>(totalItems, req.PageNumber, req.PageSize);
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
    public async Task<bool> RemovePostToFavoriteAsync(Guid postId)
    {
        await _tagFavoriteRepository.Connection.ExecuteAsync(DeletePostFavoriteByUserIdAndPostIdQuery, new { postId, userId = _currentUserService.Session.UserId });
        return true;
    }

    public async Task<PagedResponse<FavoritePostResponse>> GetPostFavoriteAsync(FavoritePostR req)
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
        var response = new PagedResponse<FavoritePostResponse>(totalItems, req.PageNumber, req.PageSize);
        response.Items = items;

        return response;
    }
    public async Task<PagedResponse<FeedDto>> GetPostFavoriteByUserAsync(FavoritePostR req)
    {
        try
        {
            PagedResponse<FeedDto> results;
            var userId = _currentUserService.Session.UserId;

            var offset = req.PageSize * (req.PageNumber - 1);
            var multi = await _postRepository
                        .Connection.QueryMultipleAsync(GetFavoritePostByUserQuery, new
                        {
                            @UserId = userId,
                            Type = (int)PostType.Feed,
                            PageSize = req.PageSize,
                            Offet = offset,
                        });
            var items = await multi.ReadAsync<FeedsListQueryDbDto>().ConfigureAwait(false);
            var listItemResponse = new List<FeedDto>();

            var postIds = await _context.SocialPostFavoriteAvailable.Where(p => p.UserId == userId).Select(p => p.PostId).ToListAsync();

            foreach (var item in items)
            {
                listItemResponse.Add(_feedService.MappingFeedInListRespone(item, postIds));
            }
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            if (items != null && items.Count() > 0)
            {
                results = new PagedResponse<FeedDto>(totalItems, req.PageNumber, req.PageSize);
                results.Items = listItemResponse;
            }
            else
            {
                results = new PagedResponse<FeedDto>(0);
            }

            return results;
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }
    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    #endregion
}
