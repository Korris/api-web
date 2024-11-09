using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Story.Api.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Interfaces;
using Lib.Common.Extensions;
using Models;
using Requests;

public partial class ReactService<T> : IReactService<T> where T : BaseReaction, new()
{
    public ReactService(IUnitOfWork unitOfWork,
        INotificationService notificationService,
        ISmartCountService smartCountService,
        IMcsgContext context)
    {
        _reactRepository = unitOfWork.GetRepository<T>();
        _notificationService = notificationService;
        _smartCountService = smartCountService;
        _context = context;
    }

    public async Task<bool> AddReaction(ReactionReactR request)
    {
        var targetId = request.TargetId;
        var userId = request.UserId ?? Guid.Empty;
        var type = request.Type;
        var isReply = request.IsReply ?? false;

        var reactionDb = await GetReactionByUser(request);
        if (reactionDb != null)
        {
            bool isChange = false;
            if (reactionDb.IsDelete)
            {
                reactionDb.IsDelete = false;
                await AddCountQueue(targetId);
                isChange = true;
            }

            if (reactionDb.Type != type)
            {
                reactionDb.Type = type;
                isChange = true;
            }

            if (isChange)
            {
                //Update when revert delete or update new type
                var updateResult = await _reactRepository.UpdateAsync(reactionDb);

                // Send Notification
                await SendReactNotificationAsync(reactionDb.Id, request, targetId, type, isReply);

                return updateResult;
            }
            else
            {
                return false;
            }
        }
        else
        {
            var insertResult = await AddNewReaction(targetId, type, userId);
            if (insertResult != null)
            {
                await SendReactNotificationAsync(insertResult.Id, request, targetId, type, isReply);
                await AddCountQueue(targetId);
                return true;
            }

            return false;
        }
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
        var followingList = await _context.UserFollowAvailable
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

    public async Task<bool> RemoveReaction(ReactionReactR request)
    {
        var res = await GetReactionByUser(request);
        if (res == null)
        {
            return false;
        }

        res.IsDelete = true;
        await RemoveCountQueue(request.TargetId);

        return await _reactRepository.UpdateAsync(res);
    }
    public async Task<T?> GetReactionByUser(ReactionReactR request)
    {
        var query = string.Format(GetReactByUsersQuery, _reactRepository.TableName);

        var res = await _reactRepository
                .Connection.QueryFirstOrDefaultAsync<T>(query, new
                {
                    request.TargetId,
                    AuthorId = request.UserId
                });

        return res;
    }

    private async Task<T> AddNewReaction(Guid targetId, ReactionType type, Guid userId)
    {
        var checkDb = new T
        {
            TargetId = targetId,
            AuthorId = userId,
            Type = type
        };

        var result = await _reactRepository.InsertAsync(checkDb);
        if (result > 0)
        {
            return checkDb;
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
            nameof(StoryPostReaction) => NotificationEntityType.StoryPostReaction,
            nameof(StoryPostCommentReaction) => isReply ? NotificationEntityType.StoryPostCommentReplyReaction : NotificationEntityType.StoryPostCommentReaction,
            nameof(StorySubPostCommentReaction) => isReply ? NotificationEntityType.StorySubPostCommentReplyReaction : NotificationEntityType.StorySubPostCommentReaction,
            _ => NotificationEntityType.StoryPostReaction
        };

        await _notificationService.AddReactionNotificationAsync(notiReq);
    }

    private async Task AddCountQueue(Guid targetId)
    {
        switch (typeof(T))
        {
            case
           var cls when cls == typeof(StoryPostReaction):
                {
                    await _smartCountService.QueueAddReactionCount(targetId, EntityType.Post);
                    break;
                }
            case
            var cls when cls == typeof(StorySubPostReaction):
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
            case var cls when cls == typeof(StoryPostReaction):
                {
                    await _smartCountService.QueueRemoveReactionCount(targetId, EntityType.Post);
                    break;
                }

            case var cls when cls == typeof(StorySubPostReaction):
                {
                    await _smartCountService.QueueRemoveReactionCount(targetId, EntityType.SubPost);
                    break;
                }
        }
    }

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    private readonly IRepository<T> _reactRepository;
    private readonly INotificationService _notificationService;
    private readonly ISmartCountService _smartCountService;

    #endregion
}
