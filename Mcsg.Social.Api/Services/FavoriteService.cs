using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Services;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Dtos;
using Extensions;
using Interfaces;
using Models;
using Requests;

public partial class FavoriteService : BaseS, IFavoriteService
{
    private readonly IRepository<TagFavorite> _tagFavoriteRepository;
    private readonly IRepository<SocialPostFavorite> _postFavoriteRepository;
    private readonly IValidator<TagFavorite> _tagFavoriteValidator;
    private readonly IValidator<SocialPostFavorite> _postFavoriteValidator;

    private readonly IRepository<Tag> _tagRepository;
    private readonly IRepository<SocialPost> _postRepository;
    private readonly IRepository<SocialSubPost> _subPostRepository;
    private readonly IRepository<SocialResource> _resourceRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<SocialMetaData> _metaDataRepository;
    private readonly IRepository<SocialPostLink> _postLinkRepository;
    private readonly IRepository<SocialTagPost> _tagPostRepository;

    private readonly IFeedService _feedService;

    public FavoriteService(IMcsgContext context,
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
    public async Task<bool> RemovePostToFavoriteAsync(IdBaseR request)
    {
        await _tagFavoriteRepository.Connection.ExecuteAsync(DeletePostFavoriteByUserIdAndPostIdQuery, new { postId = request.Id, userId = request.UserId });
        return true;
    }

    public async Task<PagedResponse<FeedDto>> GetPostFavoriteByUserAsync(FavoritePostR req)
    {
        try
        {
            var userId = req.UserId ?? Guid.Empty;
            PagedResponse<FeedDto> results;

            var offset = req.PageSize * (req.PageNumber - 1);
            var multi = await _postRepository
                        .Connection.QueryMultipleAsync(GetFavoritePostByUserQuery, new
                        {
                            Type = (int)PostType.Feed,
                            req.PageSize,
                            Offet = offset,
                            UserId = userId
                        });
            var items = await multi.ReadAsync<FeedsListQueryDbDto>().ConfigureAwait(false);
            var listItemResponse = new List<FeedDto>();

            var postIds = await _context.Available<SocialPostFavorite>().Where(p => p.UserId == userId).Select(p => p.PostId).ToListAsync();

            foreach (var item in items)
            {
                listItemResponse.Add(_feedService.MappingFeedInListRespone(item, postIds, null));
            }
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            if (items != null && items.Count() > 0)
            {
                var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
                var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"social.""SocialPostReactions"""), new
                {
                    TargetIds = items.Select(p => p.Id).ToList(),
                    UserId = userId
                });
                if (postReactionResponse.Count() > 0)
                {
                    foreach (var item in listItemResponse)
                    {
                        var postReaction = postReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                        if (postReaction.Count > 0)
                        {
                            MapReactionFeedDtoResponse(item, postReaction);
                        }
                    }
                }
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

    private void MapReactionFeedDtoResponse(FeedDto item, List<CommentReactionResponseQuery> reactions)
    {
        var currentUserReact = reactions.Where(x => x.ReactByCurrent > 0).FirstOrDefault();
        item.Reaction = new ReactionsResponse
        {
            TargetId = item.Id,
            CurrentUserReactType = currentUserReact?.Type,
            Reactions = reactions.Select(x => new ReactionResponse { Count = x.Count, Type = x.Type.Value }).ToList(),
            TotalReacts = reactions.Select(x => x.Count).Sum(),
            MostReactionType = reactions.OrderByDescending(p => p.Count).FirstOrDefault().Type
        };
    }
    #endregion
}
