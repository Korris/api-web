using System.Linq.Expressions;
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
using Common.SeedWork;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Interfaces;
using Models;
using Requests;
using static Common.SeedWork.Constants.Error;

public partial class ReactService<T> : BaseS, IReactService<T> where T : BaseReaction, new()
{
    #region -- Methods --

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

        var reactionInfor = await GetReactionInfor(targetId);

        var response = new ReactionUpdateResponse
        {
            MicroService = MicroService.Social.ToString(),
            TargetId = reactionInfor.PostId,
            SubPostId = reactionInfor.SubPostId
        };

        var ett = await GetData<T>(p => p.TargetId == request.TargetId && p.AuthorId == request.UserId);

        bool isReactNotification = reactionInfor.AuthorId != userId;
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
        var reactionInfor = await GetReactionInfor(request.TargetId);

        var ett = await GetData<T>(p => p.TargetId == request.TargetId && p.AuthorId == request.UserId);
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
            TargetId = reactionInfor.PostId,
            SubPostId = reactionInfor.SubPostId,
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

    private async Task<P?> GetData<P>(Expression<Func<P, bool>> predicate, Func<IQueryable<P>, IQueryable<P>>? includes = null) where P : AuditableEntity
    {
        var query = _context.Available<P>().AsQueryable();

        if (includes != null)
        {
            query = includes(query);
        }

        return await query.FirstOrDefaultAsync(predicate);
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

    /// <summary>
    /// GetReactionInfor
    /// </summary>
    /// <param name="targetId">Can be PostId, SubPostId, PostCommentId, or SubPostCommentId, depending on what is being commented on</param>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    private async Task<ReactionInfor> GetReactionInfor(Guid targetId)
    {
        switch (typeof(T).Name)
        {
            case nameof(SocialPostReaction):
                var post = await GetData<SocialPost>(p => p.Id == targetId);
                if (post == null)
                {
                    throw new NotFoundException(nameof(E204), E204);
                }

                return new ReactionInfor(Guid.Empty, post.Id, post.UserId);

            case nameof(SocialSubPostReaction):
                var subPost = await GetData<SocialSubPost>(p => p.Id == targetId);
                if (subPost == null)
                {
                    throw new NotFoundException(nameof(E208), E208);
                }

                return new ReactionInfor(subPost.PostId, subPost.Id, subPost.UserId);

            case nameof(SocialPostCommentReaction):
                var commentPost = await GetData<SocialPostComment>(p => p.Id == targetId, q => q.Include(p => p.Post));
                if (commentPost == null || commentPost.Post.IsDelete)
                {
                    throw new NotFoundException(nameof(E204), E204);
                }

                return new ReactionInfor(commentPost.PostId, Guid.Empty, commentPost.AuthorId);

            case nameof(SocialSubPostCommentReaction):
                var commentSubPost = await GetData<SocialSubPostComment>(p => p.Id == targetId, q => q.Include(p => p.Post));
                if (commentSubPost == null || commentSubPost.Post.IsDelete)
                {
                    throw new NotFoundException(nameof(E208), E208);
                }

                return new ReactionInfor(commentSubPost.Post.PostId, commentSubPost.PostId, commentSubPost.AuthorId);

            default:
                return new ReactionInfor();
        }
    }

    #endregion

    #region -- Fields --

    private readonly IRepository<T> _reactRepository;
    private readonly INotificationService _notificationService;
    private readonly ISmartCountService _smartCountService;

    #endregion

    #region -- Classes --

    /// <summary>
    /// ReactionInfo
    /// </summary>
    private class ReactionInfor
    {
        #region -- Method --

        /// <summary>
        /// Initialize
        /// </summary>
        public ReactionInfor() { }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="subPostId"></param>
        /// <param name="authorId"></param>
        public ReactionInfor(Guid postId, Guid subPostId, Guid authorId)
        {
            PostId = postId;
            SubPostId = subPostId;
            AuthorId = authorId;
        }

        #endregion

        #region -- Properties --

        /// <summary>
        /// PostId
        /// </summary>
        public Guid PostId { get; init; } = default!;

        /// <summary>
        /// SubPostId
        /// </summary>
        public Guid SubPostId { get; init; } = default!;

        /// <summary>
        /// AuthorId
        /// </summary>
        public Guid AuthorId { get; init; } = default!;

        #endregion
    };

    #endregion
}
