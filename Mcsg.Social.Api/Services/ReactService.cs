using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.Extensions;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Interfaces;
using Models;
using Requests;
using static Common.SeedWork.Constants.Error;

public partial class ReactService<T> : BaseS, IReactService<T> where T : BaseReaction, new()
{
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="notificationService"></param>
    /// <param name="smartCountService"></param>
    public ReactService(IMcsgContext context, IUnitOfWork unitOfWork, INotificationService notificationService, ISmartCountService smartCountService) : base(context)
    {
        _reactRepository = unitOfWork.GetRepository<T>();
        _notificationService = notificationService;
        _smartCountService = smartCountService;
    }

    public async Task<ReactionUpdateResponse> AddReaction(ReactionReactR request)
    {
        var targetId = request.TargetId;
        var userId = request.UserId ?? Guid.Empty;
        var type = request.Type;
        var isReply = request.IsReply ?? false;

        var postId = Guid.Empty;
        var subPostId = Guid.Empty;
        var authorId = Guid.Empty;
        Type entityType = typeof(T);
        switch (entityType.Name)
        {
            case nameof(SocialPostReaction):
                postId = targetId;
                authorId = await _context.Available<SocialPost>(false)
                    .Where(p => p.Id == targetId)
                    .Select(p => p.UserId)
                    .FirstOrDefaultAsync();
                break;

            case nameof(SocialSubPostReaction):
                var subPost = await _context.Available<SocialSubPost>(false)
                    .Where(p => p.Id == targetId)
                    .FirstOrDefaultAsync();

                postId = subPost!.PostId;
                subPostId = targetId;
                authorId = subPost.UserId;
                break;

            case nameof(SocialPostCommentReaction):
                var commentPost = await _context.Available<SocialPostComment>(false)
                    .Where(p => p.Id == targetId)
                    .FirstOrDefaultAsync();

                postId = commentPost!.PostId;
                authorId = commentPost.AuthorId;
                break;

            case nameof(SocialSubPostCommentReaction):
                var commentSubPost = await _context.Available<SocialSubPostComment>(false)
                    .Include(p => p.Post)
                    .Where(p => p.Id == targetId)
                    .FirstOrDefaultAsync();

                postId = commentSubPost!.Post.PostId;
                subPostId = commentSubPost.PostId;
                authorId = commentSubPost.AuthorId;
                break;

            default:
                break;
        }

        var response = new ReactionUpdateResponse
        {
            MicroService = MicroService.Social.ToString(),
            TargetId = postId,
            SubPostId = subPostId
        };

        var ett = await GetReactionByUser(request);

        bool isReactNotification = authorId != userId;
        bool isChange = false;
        bool newReaction = false;

        if (ett != null)
        {
            response.ReactionId = ett.Id;
            if (ett.IsDelete)
            {
                ett.IsDelete = false;
                await AddCountQueue(targetId);
                isChange = true;
            }

            if (ett.Type != type)
            {
                ett.Type = type;
                isChange = true;
            }

            if (isChange)
            {
                // Update when revert delete or update new type
                await _context.SaveChangesAsync(default);

                response.ReactionId = ett.Id;
                response.IsDeleted = ett.IsDelete;
            }
        }
        else
        {
            ett = await AddNewReaction(targetId, type, userId);
            if (ett != null)
            {
                await AddCountQueue(targetId);

                newReaction = true;
                response.ReactionId = ett.Id;
                response.IsDeleted = ett.IsDelete;
            }
        }
        if ((isChange || newReaction) && isReactNotification)
        {
            await SendReactNotificationAsync(ett.Id, request, targetId, type, isReply);
        }

        return response;
    }

    public async Task<ReactionsResponse> GetReactions(ReactionReactR request)
    {
        var targetId = request.TargetId;
        var query = string.Format(GetReactByTargetQuery, _reactRepository.TableName);

        var reactionsDb = await _reactRepository
                .Connection.QueryAsync<ReactionResponseQuery>(query, new
                {
                    TargetId = targetId,
                    request.UserId
                });
        var currentUserReact = reactionsDb.Where(x => x.ReactByCurrent > 0).FirstOrDefault();
        var result = new ReactionsResponse
        {
            TargetId = targetId,
            CurrentUserReactType = currentUserReact?.Type
        };
        result.Reactions = reactionsDb.Select(x => new ReactionResponse { Count = x.Count, Type = x.Type }).ToList();
        result.TotalReacts = reactionsDb.Select(x => x.Count).Sum();
        if (result.TotalReacts > 0)
        {
            result.MostReactionType = reactionsDb.OrderByDescending(p => p.Count).FirstOrDefault().Type;
        }
        return result;
    }

    public async Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request)
    {
        int? reactType = !request.Type.IsNumeric() ? null : request.Type.ToInt();
        var query = string.Format(GetReactionByTargetQuery, _reactRepository.TableName);

        var offset = request.PageSize * (request.PageNumber - 1);

        var multi = await _reactRepository.Connection.QueryMultipleAsync(query,
                                            new
                                            {
                                                TargetId = targetId,
                                                Type = reactType,
                                                request.PageSize,
                                                Offet = offset
                                            });

        var items = await multi.ReadAsync<ReactionsUserModel>().ConfigureAwait(false);
        var userId = request.UserId;
        var followingList = await _context.Available<UserFollow>()
                            .Where(p => userId == p.UserFollowerId)
                            .Select(p => p.UserFollowingId)
                            .ToListAsync();

        if (items != null)
        {
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            var response = new PagedResponse<ReactionsUserModel>(totalItems, request.PageNumber, request.PageSize);
            response.Items = items;

            foreach (var item in items)
            {
                item.IsFollowing = followingList.Contains(item.AuthorId);
            }

            return response;
        }
        else
        {
            return new PagedResponse<ReactionsUserModel>(0);
        }
    }

    public async Task<ReactionUpdateResponse> RemoveReaction(ReactionReactR request)
    {
        var targetId = request.TargetId;

        var postId = await _context.Available<SocialSubPost>()
                                   .Where(p => p.Id == targetId)
                                   .Select(p => p.PostId)
                                   .FirstOrDefaultAsync();

        var ett = await GetReactionByUser(request);
        if (ett == null)
        {
            throw new NotFoundException(nameof(E002), E002);
        }

        ett.IsDelete = true;
        await RemoveCountQueue(request.TargetId);
        await _context.SaveChangesAsync(default);

        await RemoveNotification(request);
        return new ReactionUpdateResponse
        {
            ReactionId = ett.Id,
            MicroService = MicroService.Social.ToString(),
            TargetId = postId != Guid.Empty ? postId : targetId,
            SubPostId = postId != Guid.Empty ? targetId : null,
            IsDeleted = ett.IsDelete
        };
    }

    private async Task RemoveNotification(ReactionReactR request)
    {
        var notificationEntitype = NotificationEntityType.ComicPostReaction;
        Type entityType = typeof(T);
        if (Enum.TryParse<NotificationEntityType>(entityType.Name, out var notificationType))
        {
            notificationEntitype = notificationType;
        }
        var notificationObject = await _context.Available<NotificationObject>().FirstOrDefaultAsync(p => p.EntityType == notificationEntitype &&
                                                                                            p.LocationId == request.TargetId &&
                                                                                            p.ActorId == request.UserId);

        await _context.Available<Notification>().Where(c => c.NotificationObjectId == notificationObject.Id)
        .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDelete, p => true));
    }

    public async Task<T?> GetReactionByUser(ReactionReactR request)
    {
        var set = _context.Set<T>();
        return await set.FirstOrDefaultAsync(p => p.TargetId == request.TargetId && p.AuthorId == request.UserId);
    }

    private async Task<T?> AddNewReaction(Guid targetId, ReactionType type, Guid userId)
    {
        var ett = new T
        {
            TargetId = targetId,
            AuthorId = userId,
            Type = type
        };

        var set = _context.Set<T>();
        await set.AddAsync(ett);

        var result = await _context.SaveChangesAsync(default) > 0;
        if (result)
        {
            return ett;
        }
        else
        {
            return null;
        }
    }

    private async Task SendReactNotificationAsync(Guid reactionId, BaseR request, Guid targetId, ReactionType reactionType, bool isReply = false)
    {
        var authorName = !string.IsNullOrWhiteSpace(request.ProfileName) ? request.ProfileName : request.UserName;

        var notiReq = new ReactionNotificationReq()
        {
            Id = reactionId,
            TargetId = targetId,
            AuthorId = request.UserId ?? Guid.Empty,
            AuthorName = authorName,
            ReactionType = reactionType,
            UserAvatar = request.UserAvatar ?? "",
            IsReplyReaction = isReply
        };

        Type entityType = typeof(T);
        notiReq.EntityType = entityType.Name switch
        {
            nameof(SocialPostReaction) => NotificationEntityType.SocialPostReaction,
            nameof(SocialPostCommentReaction) => isReply ? NotificationEntityType.SocialPostCommentReplyReaction : NotificationEntityType.SocialPostCommentReaction,
            nameof(SocialSubPostCommentReaction) => isReply ? NotificationEntityType.SocialSubPostCommentReplyReaction : NotificationEntityType.SocialSubPostCommentReaction,
            nameof(SocialSubPostReaction) => NotificationEntityType.SocialSubPostReaction,
            _ => NotificationEntityType.SocialPostReaction
        };

        await _notificationService.AddReactionNotificationAsync(notiReq);
    }

    private async Task AddCountQueue(Guid targetId)
    {
        switch (typeof(T))
        {
            case
           var cls when cls == typeof(SocialPostReaction):
                {
                    await _smartCountService.QueueAddReactionCount(targetId, EntityType.Post);
                    break;
                }
            case
            var cls when cls == typeof(SocialSubPostReaction):
                {
                    await _smartCountService.QueueAddReactionCount(targetId, EntityType.SubPost);
                    break;
                }

        }
    }

    private async Task RemoveCountQueue(Guid targetId)
    {
        switch (typeof(T))
        {
            case var cls when cls == typeof(SocialPostReaction):
                {
                    await _smartCountService.QueueRemoveReactionCount(targetId, EntityType.Post);
                    break;
                }

            case var cls when cls == typeof(SocialSubPostReaction):
                {
                    await _smartCountService.QueueRemoveReactionCount(targetId, EntityType.SubPost);
                    break;
                }
        }
    }

    private async Task<Guid> GetUserIdByPostId(Guid postId)
    {
        return await _context.Available<SocialPost>(false)
            .Where(p => p.Id == postId)
            .Select(p => p.UserId)
            .FirstOrDefaultAsync();
    }

    #region -- Fields --

    private readonly IRepository<T> _reactRepository;
    private readonly INotificationService _notificationService;
    private readonly ISmartCountService _smartCountService;

    #endregion
}
