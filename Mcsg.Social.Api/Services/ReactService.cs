using Dapper;

namespace Mcsg.Social.Api.Services;

using Common.Core.Enums;
using Extensions;
using Interfaces;
using Lib.Common.Extensions;
using Lib.Common.Helpers;
using Lib.Common.Web.Security;
using Lib.Data.Domain.Entities;
using Lib.Data.Domain.Entities.Common;
using Lib.Data.Entities.Common;
using Lib.Data.Enums;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;
using Requests;

public partial class ReactService<T> : IReactService<T> where T : ReactionBase, new()
{
    private readonly IRepository<T> _reactRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ISmartCountService _smartCountService;
    private IConfiguration _configuration;
    public ReactService(IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        IConfiguration configuration,
        ISetting setting,
        ISmartCountService smartCountService)
    {
        _reactRepository = unitOfWork.GetRepository<T>();
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _smartCountService = smartCountService;
        _setting = setting;
        _configuration = configuration;
    }
    public async Task<bool> AddReaction(Guid targetId, ReactionType type)
    {
        var currentUserId = _currentUserService.Session.UserId;

        var reactionDb = await GetReactionByUser(targetId, currentUserId);

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
                await SendReactNotificationAsync(reactionDb.Id, targetId);

                return updateResult;
            }
            else
            {
                //DO NOTHING
                return false;
            }

        }
        else
        {
            var insertResult = await AddNewReaction(targetId, type, currentUserId);

            // Send Notification
            if (insertResult != null)
            {
                await SendReactNotificationAsync(insertResult.Id, targetId);
                await AddCountQueue(targetId);
                return true;
            }

            return false;
        }

    }

    public async Task<T> GetReaction(Guid targetId, ReactionType type)
    {
        var currentUserId = _currentUserService.Session.UserId;
        var query = string.Format(GetReactTypeAndUsersQuery, _reactRepository.TableName);

        var checkDb = await _reactRepository
                .Connection.QueryFirstOrDefaultAsync<T>(query, new
                {
                    TargetId = targetId,
                    AuthorId = currentUserId,
                    Type = type
                });
        return checkDb;
    }
    public async Task<ReactionsResponse> GetReactions(Guid targetId)
    {
        var currentUserId = _currentUserService.Session?.UserId;
        var query = string.Format(GetReactByTargetQuery, _reactRepository.TableName);

        var reactionsDb = await _reactRepository
                .Connection.QueryAsync<ReactionResponseQuery>(query, new
                {
                    TargetId = targetId,
                    UserId = currentUserId
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
    public async Task<PagedResults<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request)
    {
        int? reactType = !request.Type.IsNumeric() ? null : request.Type.ToInt();
        var query = string.Format(GetReactionByTargetQuery, _reactRepository.TableName);

        var offset = request.PageSize * (request.PageNumber - 1);

        var multi = await _reactRepository.Connection.QueryMultipleAsync(query,
                                            new
                                            {
                                                TargetId = targetId,
                                                Type = reactType,
                                                PageSize = request.PageSize,
                                                Offet = offset
                                            });

        var items = await multi.ReadAsync<ReactionsUserModel>().ConfigureAwait(false);
        if (items != null)
        {
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);
            foreach (var item in items)
            {
                item.AuthorAvatar = UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, item.AuthorAvatar);
            }
            var response = new PagedResults<ReactionsUserModel>(totalItems, request.PageNumber, request.PageSize);
            response.Items = items;

            return response;
        }
        else
        {
            return new PagedResults<ReactionsUserModel>(0);
        }
    }

    public async Task<bool> RemoveReaction(Guid targetId)
    {
        var currentUserId = _currentUserService.Session.UserId;

        var checkDb = await GetReactionByUser(targetId, currentUserId);
        if (checkDb == null)
        {
            //DO NOTHING
            return false;
        }
        checkDb.IsDelete = true;
        await RemoveCountQueue(targetId);
        return await _reactRepository.UpdateAsync(checkDb);
    }
    public async Task<T> GetReactionByUser(Guid targetId, Guid userId)
    {
        var query = string.Format(GetReactByUsersQuery, _reactRepository.TableName);

        var checkDb = await _reactRepository
                .Connection.QueryFirstOrDefaultAsync<T>(query, new
                {
                    TargetId = targetId,
                    AuthorId = userId
                });
        return checkDb;
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
    private async Task SendReactNotificationAsync(Guid reactionId, Guid targetId)
    {
        Type entityType = typeof(T);
        if (entityType.Name == nameof(PostReaction))
        {
            //Send notification when video processing
            var authorName = !string.IsNullOrWhiteSpace(_currentUserService.Session.ProfileName)
                                            ? _currentUserService.Session.ProfileName
                                            : _currentUserService.Session.UserName;

            var notiReq = new ReactionNotificationReq()
            {
                Id = reactionId,
                TargetId = targetId,
                AuthorId = _currentUserService.Session.UserId,
                AuthorName = authorName,
                EntityType = NotificationEntityType.PostReaction
            };

            await _notificationService.AddReactionNotificationAsync(notiReq);
        }
    }

    private async Task AddCountQueue(Guid targetId)
    {
        switch (typeof(T))
        {
            case
           var cls when cls == typeof(PostReaction):
                {
                    await _smartCountService.QueueAddReactionCount(targetId, EntityType.Post);
                    break;
                }
            case
            var cls when cls == typeof(SubPostReaction):
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
            case
           var cls when cls == typeof(PostReaction):
                {
                    await _smartCountService.QueueRemoveReactionCount(targetId, EntityType.Post);
                    break;
                }
            case
            var cls when cls == typeof(SubPostReaction):
                {
                    await _smartCountService.QueueRemoveReactionCount(targetId, EntityType.SubPost);
                    break;
                }

        }
    }

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
