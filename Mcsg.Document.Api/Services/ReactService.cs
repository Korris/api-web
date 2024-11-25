using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Mcsg.Document.Api.Services;

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
using Dtos;
using Interfaces;
using Models;
using Requests;
using static Common.SeedWork.Constants.Error;

public partial class ReactService<T> : BaseSettingS, IReactService<T> where T : BaseReaction, new()
{
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="notificationService"></param>
    /// <param name="smartCountService"></param>
    public ReactService(IMcsgContext context, ISetting setting, IUnitOfWork unitOfWork, ISmartCountService smartCountService) : base(context, setting)
    {
        _reactRepository = unitOfWork.GetRepository<T>();
        _smartCountService = smartCountService;
    }

    public async Task<ReactionUpdateResponse> AddReaction(ReactionReactR request)
    {
        var targetId = request.TargetId;
        var userId = request.UserId ?? Guid.Empty;
        var type = request.Type;
        var isReply = request.IsReply ?? false;

        var postId = await _context.DocumentSubPostAvailable
                                   .Where(p => p.Id == targetId)
                                   .Select(p => p.PostId)
                                   .FirstOrDefaultAsync();

        var response = new ReactionUpdateResponse
        {
            MicroService = MicroService.Document.ToString(),
            TargetId = postId != Guid.Empty ? postId : targetId,
            SubPostId = postId != Guid.Empty ? targetId : null
        };

        var ett = await GetReactionByUser(request);
        if (ett != null)
        {
            response.ReactionId = ett.Id;
            bool isChange = false;
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

                // Send Notification
                await SendReactNotificationAsync(ett.Id, request, targetId, type, isReply);

                response.ReactionId = ett.Id;
                response.IsDeleted = ett.IsDelete;
            }
        }
        else
        {
            ett = await AddNewReaction(targetId, type, userId);
            if (ett != null)
            {
                await SendReactNotificationAsync(ett.Id, request, targetId, type, isReply);
                await AddCountQueue(targetId);

                response.ReactionId = ett.Id;
                response.IsDeleted = ett.IsDelete;
            }
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

    public async Task<ReactionUpdateResponse> RemoveReaction(ReactionReactR request)
    {
        var targetId = request.TargetId;

        var postId = await _context.DocumentSubPostAvailable
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

        return new ReactionUpdateResponse
        {
            ReactionId = ett.Id,
            MicroService = MicroService.Document.ToString(),
            TargetId = postId != Guid.Empty ? postId : targetId,
            SubPostId = postId != Guid.Empty ? targetId : null,
            IsDeleted = ett.IsDelete
        };
    }

    public async Task<T?> GetReactionByUser(ReactionReactR request)
    {
        var set = _context.Set<T>();
        return await set.FirstOrDefaultAsync(p => !p.IsDelete && p.TargetId == request.TargetId && p.AuthorId == request.UserId);
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
            nameof(DocumentPostReaction) => NotificationEntityType.DocumentPostReaction,
            nameof(DocumentPostCommentReaction) => isReply ? NotificationEntityType.DocumentPostCommentReplyReaction : NotificationEntityType.DocumentPostCommentReaction,
            nameof(DocumentSubPostCommentReaction) => isReply ? NotificationEntityType.DocumentSubPostCommentReplyReaction : NotificationEntityType.DocumentSubPostCommentReaction,
            nameof(DocumentSubPostReaction) => NotificationEntityType.DocumentSubPostReaction,
            _ => NotificationEntityType.DocumentPostReaction
        };

        await AddReactionNotificationAsync(notiReq);
    }

    public async Task<bool> AddReactionNotificationAsync(ReactionNotificationReq req)
    {
        var baseUrl = _setting.Api.Web.Realtime;
        var urlBuilder = new System.Text.StringBuilder();
        urlBuilder.Append(baseUrl != null ? baseUrl.TrimEnd('/') : "").Append("/notification/reaction");

        var url = urlBuilder.ToString();

        var response = await url.MakePostRequest(req);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<ApiNotificationDto>(responseContent);

            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task AddCountQueue(Guid targetId)
    {
        switch (typeof(T))
        {
            case
           var cls when cls == typeof(DocumentPostReaction):
                {
                    await _smartCountService.QueueAddReactionCount(targetId, EntityType.Post);
                    break;
                }
            case
            var cls when cls == typeof(DocumentSubPostReaction):
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
            case var cls when cls == typeof(DocumentPostReaction):
                {
                    await _smartCountService.QueueRemoveReactionCount(targetId, EntityType.Post);
                    break;
                }

            case var cls when cls == typeof(DocumentSubPostReaction):
                {
                    await _smartCountService.QueueRemoveReactionCount(targetId, EntityType.SubPost);
                    break;
                }
        }
    }

    #region -- Fields --

    private readonly IRepository<T> _reactRepository;
    private readonly ISmartCountService _smartCountService;

    #endregion
}
