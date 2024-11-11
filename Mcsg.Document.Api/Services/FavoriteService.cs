using Dapper;

namespace Mcsg.Document.Api.Services;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Dtos;
using Interfaces;
using Models;
using Requests;

public partial class FavoriteService : BaseS, IFavoriteService
{
    private readonly IRepository<TagFavorite> _tagFavoriteRepository;
    private readonly IRepository<DocumentPostFavorite> _postFavoriteRepository;
    private readonly IValidator<TagFavorite> _tagFavoriteValidator;
    private readonly IValidator<DocumentPostFavorite> _postFavoriteValidator;

    private readonly IRepository<Tag> _tagRepository;
    private readonly IRepository<DocumentPost> _postRepository;
    private readonly IRepository<DocumentSubPost> _subPostRepository;
    private readonly IRepository<DocumentResource> _resourceRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<DocumentMetaData> _metaDataRepository;
    private readonly IRepository<DocumentPostLink> _postLinkRepository;
    private readonly IRepository<DocumentTagPost> _tagPostRepository;

    private readonly IFeedService _feedService;

    public FavoriteService(IMcsgContext context,
        IUnitOfWork unitOfWork,
        IValidator<TagFavorite> tagFavoriteValidator,
        IValidator<DocumentPostFavorite> postFavoriteValidator,
        IRepository<TagFavorite> tagFavoriteRepository,
        IRepository<DocumentPostFavorite> postFavoriteRepository,
        IRepository<Tag> tagRepository,
        IRepository<DocumentPost> postRepository,
        IRepository<DocumentSubPost> subPostRepository,
        IRepository<DocumentResource> resourceRepository,
        IRepository<User> userRepository,
        IRepository<DocumentMetaData> metaDataRepository,
        IRepository<DocumentPostLink> postLinkRepository,
        IRepository<DocumentTagPost> tagPostRepository,
        IFeedService feedService) : base(context)
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
        {
            return true;
        }

        await _context.TagFavorites.AddAsync(tagFavorite);
        return await _context.SaveChangesAsync(default) > 0;
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
        var postFavorite = new DocumentPostFavorite
        {
            PostId = request.Id,
            UserId = request.UserId ?? Guid.Empty
        };

        await _postFavoriteValidator.OnValidate(postFavorite);

        var hasExisted = (await _postFavoriteRepository.GetByCustomQuery(GetPostFavoriteByPostIdAndUserId, new { postId = postFavorite.PostId, userId = postFavorite.UserId })).Any();
        if (hasExisted)
        {
            return true;
        }

        await _context.DocumentPostFavorites.AddAsync(postFavorite);
        return await _context.SaveChangesAsync(default) > 0;
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
