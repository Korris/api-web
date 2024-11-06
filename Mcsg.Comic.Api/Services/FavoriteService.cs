using Dapper;

namespace Mcsg.Comic.Api.Services;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Requests;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Dtos;
using Interfaces;
using Lib.Common.Interfaces;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;
using Requests;

public partial class FavoriteService : IFavoriteService
{
    private readonly IRepository<TagFavorite> _tagFavoriteRepository;
    private readonly IRepository<ComicPostFavorite> _postFavoriteRepository;
    private readonly IValidator<TagFavorite> _tagFavoriteValidator;
    private readonly IValidator<ComicPostFavorite> _postFavoriteValidator;

    private readonly IRepository<Tag> _tagRepository;
    private readonly IRepository<ComicPost> _postRepository;
    private readonly IRepository<ComicSubPost> _subPostRepository;
    private readonly IRepository<ComicResource> _resourceRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<ComicMetaData> _metaDataRepository;
    private readonly IRepository<ComicPostLink> _postLinkRepository;
    private readonly IRepository<ComicTagPost> _tagPostRepository;

    private readonly IFeedService _feedService;

    public FavoriteService(
        IUnitOfWork unitOfWork,
        IValidator<TagFavorite> tagFavoriteValidator,
        IValidator<ComicPostFavorite> postFavoriteValidator,
        IRepository<TagFavorite> tagFavoriteRepository,
        IRepository<ComicPostFavorite> postFavoriteRepository,
        IRepository<Tag> tagRepository,
        IRepository<ComicPost> postRepository,
        IRepository<ComicSubPost> subPostRepository,
        IRepository<ComicResource> resourceRepository,
        IRepository<User> userRepository,
        IRepository<ComicMetaData> metaDataRepository,
        IRepository<ComicPostLink> postLinkRepository,
        IRepository<ComicTagPost> tagPostRepository,
        IFeedService feedService)
    {
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
    public async Task<bool> AddTagToFavoriteAsync(IdBaseR request)
    {
        var tagFavorite = new TagFavorite
        {
            TagId = request.Id,
            UserId = request.UserId ?? Guid.Empty
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
                    req.PageSize,
                    Offet = offset,
                    UserId = req.UserId ?? Guid.Empty
                });

        var items = await multipleQuery.ReadAsync<FavoriteTagResponse>().ConfigureAwait(false);
        var totalItems = await multipleQuery.ReadFirstAsync<int>().ConfigureAwait(false);
        var response = new PagedResponse<FavoriteTagResponse>(totalItems, req.PageNumber, req.PageSize);
        response.Items = items;

        return response;
    }

    public async Task<bool> RemoveTagToFavoriteAsync(IdBaseR request)
    {
        await _tagFavoriteRepository.Connection.ExecuteAsync(DeleteTagFavoriteByUserIdAndTagIdQuery, new { tagId = request.Id, userId = request.UserId });
        return true;
    }
    #endregion

    #region Post
    public async Task<bool> AddPostToFavoriteAsync(IdBaseR request)
    {
        var postFavorite = new ComicPostFavorite
        {
            PostId = request.Id,
            UserId = request.UserId ?? Guid.Empty
        };

        await _postFavoriteValidator.OnValidate(postFavorite);

        var hasExisted = (await _postFavoriteRepository.GetByCustomQuery(GetPostFavoriteByPostIdAndUserId, new { postId = postFavorite.PostId, userId = postFavorite.UserId })).Any();

        if (hasExisted)
            return true;

        var iResult = await _postFavoriteRepository.InsertAsync(postFavorite);
        return iResult > 0;
    }

    public async Task<bool> RemovePostToFavoriteAsync(IdBaseR request)
    {
        await _tagFavoriteRepository.Connection.ExecuteAsync(DeletePostFavoriteByUserIdAndPostIdQuery, new { postId = request.Id, userId = request.UserId });
        return true;
    }

    public async Task<PagedResponse<FeedDto>> GetPostFavoriteByUserAsync(FavoritePostR req)
    {
        try
        {
            PagedResponse<FeedDto> results;

            var offset = req.PageSize * (req.PageNumber - 1);
            var multi = await _postRepository
                        .Connection.QueryMultipleAsync(GetFavoritePostByUserQuery, new
                        {
                            Type = (int)PostType.Feed,
                            req.PageSize,
                            Offet = offset,
                            UserId = req.UserId ?? Guid.Empty
                        });
            var items = await multi.ReadAsync<FeedsListQueryDbDto>().ConfigureAwait(false);
            var listItemResponse = new List<FeedDto>();
            foreach (var item in items)
            {
                listItemResponse.Add(_feedService.MappingFeedInListRespone(item));
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
}
