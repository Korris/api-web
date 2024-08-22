using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;

namespace Mcsg.Realtime.Api.Services;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Constants;
using Dtos;
using Hubs;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Models.RealTime;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Requests;

public class NotificationService : INotificationService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<SocialPost> _postRepository;
    private readonly IRepository<SocialPostComment> _postCommentRepository;
    private readonly IRepository<SocialSubPostComment> _subPostCommentRepository;
    private readonly IRepository<Notification> _notiRepository;
    private readonly IRepository<NotificationObject> _notiObjectRepository;
    private readonly IHubContext<NotificationHub> _hubcontext;
    private IUnitOfWork _unitOfWork;
    private readonly IMcsgContext _context;

    public NotificationService(ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IHubContext<NotificationHub> hubcontext, IMcsgContext context)
    {
        _currentUserService = currentUserService;
        _postRepository = unitOfWork.GetRepository<SocialPost>();
        _notiRepository = unitOfWork.GetRepository<Notification>();
        _notiObjectRepository = unitOfWork.GetRepository<NotificationObject>();
        _postCommentRepository = unitOfWork.GetRepository<SocialPostComment>();
        _subPostCommentRepository = unitOfWork.GetRepository<SocialSubPostComment>();
        _unitOfWork = unitOfWork;
        _hubcontext = hubcontext;
        _context = context;
    }

    public async Task<NotificationResponse> AddCommentNotification(CommentNotificationReq comment)
    {
        var response = new NotificationResponse();

        //var post = await _postRepository.GetByIdAsync(comment.PostId);
        if (!string.IsNullOrWhiteSpace(comment.PostHashId))
        {
            var receiverId = comment.PostCreatedBy;

            // Dont notify when comment on their feed
            if (receiverId != comment.AuthorId)
            {
                var noti = await AddNotificationAsync(
                                        actorId: comment.AuthorId
                                        , receiverId: receiverId
                                        , action: comment.IsReply ? NotificationAction.Reply : NotificationAction.Comment
                                        , entityType: comment.EntityType
                                        , entityId: comment.Id
                                        , locationId: comment.PostId
                                        , locationHashId: comment.PostHashId);

                response.Id = noti.Id;
                response.Status = noti.Status;
                response.LocationId = comment.PostId;
                response.LocationHashId = comment.PostHashId;
                response.EntityId = comment.Id;
                response.Message = GetMessage(comment);
                response.TargetType = GetTargetType(comment);
                response.ActorId = comment.AuthorId;
                response.ActorName = comment.AuthorName;
                response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
                response.NotificationType = Common.Core.Constants.Setting.NotificationType.Comment;
                response.UserAvatar = comment.UserAvatar;
                response.Order = comment.Order ?? 0;
                // Then notification the comment to post owner
                await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
            }
        }

        return response;
    }

    private string GetTargetType(CommentNotificationReq comment)
    {
        switch (comment.PostType)
        {
            case PostType.Comic:
                return comment.Type == PostTypes.Post ? Common.Core.Constants.Setting.NotificationTargetType.CommentOnComic : Common.Core.Constants.Setting.NotificationTargetType.CommentOnSubComic;
            case PostType.Story:
                return comment.Type == PostTypes.Post ? Common.Core.Constants.Setting.NotificationTargetType.CommentOnStory : Common.Core.Constants.Setting.NotificationTargetType.CommentOnSubStory;
            default:
                return comment.Type == PostTypes.Post ? Common.Core.Constants.Setting.NotificationTargetType.CommentOnFeed : Common.Core.Constants.Setting.NotificationTargetType.CommentOnSubFeed;
        }
    }

    private string GetMessage(CommentNotificationReq comment)
    {
        switch (comment.PostType)
        {
            case PostType.Comic:
                return comment.AuthorName + NotificationContent.CommentOnComic;

            case PostType.Story:
                return comment.AuthorName + NotificationContent.CommentOnStory;
            default:
                return comment.AuthorName + NotificationContent.CommentOnFeed;
                ;

        }
    }

    public async Task<NotificationResponse> AddReplyNotification(CommentNotificationReq comment)
    {
        var response = new NotificationResponse();
        var receiverId = Guid.Empty;

        if (comment.Type == PostTypes.Post)
        {
            var parentComment = await _postCommentRepository.GetByIdAsync(comment.ReplyToCommentId.Value);
            receiverId = parentComment.AuthorId;
        }
        else
        {
            var parentComment = await _subPostCommentRepository.GetByIdAsync(comment.ReplyToCommentId.Value);
            receiverId = parentComment.AuthorId;
        }

        // Dont notify when comment on their feed
        if (receiverId != Guid.Empty && receiverId != comment.AuthorId)
        {
            var post = await _postRepository.GetByIdAsync(comment.PostId);

            var noti = await AddNotificationAsync(
                                        actorId: comment.AuthorId
                                        , receiverId: receiverId
                                        , action: comment.IsReply ? NotificationAction.Reply : NotificationAction.Comment
                                        , entityType: comment.EntityType
                                        , entityId: comment.Id
                                        , locationId: comment.PostId
                                        , locationHashId: post.HashId);

            response.Id = noti.Id;
            response.Status = noti.Status;
            response.LocationId = comment.PostId;
            response.LocationHashId = post.HashId;
            response.EntityId = comment.Id;
            response.Message = comment.AuthorName + NotificationContent.ReplyOnComment;
            response.TargetType = comment.Type == PostTypes.Post ? Common.Core.Constants.Setting.NotificationTargetType.ReplyOnFeed : Common.Core.Constants.Setting.NotificationTargetType.ReplyOnSubFeed;
            response.ActorId = comment.AuthorId;
            response.ActorName = comment.AuthorName;
            response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
            response.NotificationType = Common.Core.Constants.Setting.NotificationType.Reply;
            response.UserAvatar = comment.UserAvatar;

            // Then notification the comment to post owner
            await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
        }

        return response;
    }

    public async Task<NotificationResponse> AddVideoNotification(VideoNotificationR video)
    {
        var response = new NotificationResponse();

        var receiverId = video.AuthorId;
        var noti = await AddNotificationAsync(
                                    actorId: video.AuthorId
                                    , receiverId: receiverId
                                    , action: video.Action
                                    , entityType: NotificationEntityType.Video
                                    , entityId: video.Id
                                    , locationId: video.PostId
                                    , locationHashId: video.PostHashId);

        response.Id = noti.Id;
        response.Status = noti.Status;
        response.LocationId = video.PostId;
        response.LocationHashId = video.PostHashId;
        response.EntityHashId = video.HashId;
        response.Message = GetVideoMessage(video.Action);
        response.TargetType = !string.IsNullOrWhiteSpace(video.TargetType) ? video.TargetType : Common.Core.Constants.Setting.NotificationTargetType.None;
        response.ActorId = video.AuthorId;
        response.ActorName = video.AuthorName;
        response.UserAvatar = video.UserAvatar;
        response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
        response.NotificationType = Common.Core.Constants.Setting.NotificationType.Video + video.Action.ToString();

        // Then notification the comment to post owner
        await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));

        return response;
    }

    public async Task<NotificationResponse> AddReactionNotification(ReactionNotificationReq reaction)
    {
        var response = new NotificationResponse();
        var receiverId = Guid.Empty;
        var postId = Guid.Empty;
        string postHashId = "";

        // React to post
        if (reaction.EntityType == NotificationEntityType.PostReaction)
        {
            var post = await _postRepository.GetByIdAsync(reaction.TargetId);
            if (post != null)
            {
                receiverId = post.CreatedBy != null ? post.CreatedBy.Value : Guid.Empty;
                postId = post.Id;
                postHashId = post.HashId;

                response.Message = reaction.AuthorName + NotificationContent.ReactOnFeed;
                response.TargetType = Common.Core.Constants.Setting.NotificationTargetType.Feed;
            }
        }

        // React to subpost : TODO

        // React to comment : TODO

        // React to reply : TODO

        // Dont notify when comment on their feed
        if (receiverId != Guid.Empty && receiverId != reaction.AuthorId)
        {
            // Move code below to common when have another reaction type
            var noti = await AddNotificationAsync(
                                actorId: reaction.AuthorId
                                , receiverId: receiverId
                                , action: NotificationAction.Reaction
                                , entityType: reaction.EntityType
                                , entityId: reaction.Id
                                , locationId: postId
                                , locationHashId: postHashId);

            response.Id = noti.Id;
            response.Status = noti.Status;
            response.LocationId = postId;
            response.LocationHashId = postHashId;
            response.EntityId = reaction.Id;
            response.ActorId = reaction.AuthorId;
            response.ActorName = reaction.AuthorName;
            response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
            response.NotificationType = Common.Core.Constants.Setting.NotificationType.Reaction;
            response.UserAvatar = reaction.UserAvatar;
            // Then notification the comment to post owner
            await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
        }

        return response;
    }

    public async Task<NotificationResponse> AddMentionNotification(MentionNotificationReq mention)
    {
        // TODO
        var response = new NotificationResponse();

        var receiverId = mention.EntityId;

        // Dont notify when comment on their feed
        if (receiverId != mention.AuthorId)
        {
            var entityType = NotificationEntityType.PostCommentMention;
            var targetType = Common.Core.Constants.Setting.NotificationTargetType.CommentOnFeed;
            if (mention.LocationType == MentionLocationType.SubPostComment || mention.LocationType == MentionLocationType.SubPostCommentReply)
            {
                entityType = NotificationEntityType.SubPostCommentMention;
                targetType = Common.Core.Constants.Setting.NotificationTargetType.CommentOnSubFeed;
            }

            var noti = await AddNotificationAsync(
                                    actorId: mention.AuthorId
                                    , receiverId: receiverId
                                    , action: NotificationAction.Mention
                                    , entityType: entityType
                                    , entityId: mention.EntityId
                                    , locationId: mention.LocationId
                                    , locationHashId: mention.PostHashId);

            response.Id = noti.Id;
            response.Status = noti.Status;
            response.LocationId = mention.LocationId;
            response.LocationHashId = mention.PostHashId;
            response.EntityId = mention.EntityId;
            response.Message = mention.AuthorName + NotificationContent.MentionOnComment;
            response.TargetType = targetType;
            response.ActorId = mention.AuthorId;
            response.ActorName = mention.AuthorName;
            response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
            response.NotificationType = Common.Core.Constants.Setting.NotificationType.Mention;
            response.UserAvatar = mention.UserAvatar;

            // Then notification the comment to post owner
            await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
        }

        return response;
    }

    public async Task<NotificationDto> AddNotificationAsync(Guid actorId, Guid receiverId, NotificationAction action
        , NotificationEntityType entityType, NotificationStatus status = NotificationStatus.UnRead
        , Guid? entityId = null, Guid? locationId = null, string locationHashId = "", string entityhashId = "")
    {
        try
        {
            var notiObj = new NotificationObject()
            {
                EntityType = entityType,
                Action = action,
                EntityId = entityId,
                EntityHashId = entityhashId,
                ActorId = actorId,
                LocationId = locationId,
                LocationHashId = locationHashId,
                CreatedBy = actorId,
            };

            await _notiObjectRepository.InsertAsync(notiObj);

            var dto = new NotificationDto();

            var noti = new Notification()
            {
                NotificationObjectId = notiObj.Id,
                ReceiverId = receiverId,
                Status = status
            };
            await _notiRepository.InsertAsync(noti);

            dto.Id = noti.Id;
            dto.Status = ((NotificationStatus)noti.Status).ToString();
            dto.NotificationObjectId = notiObj.Id;
            dto.ReceiverId = receiverId;
            dto.ActorId = actorId;
            dto.CreatedOn = noti.CreatedOn;

            return dto;

        }
        catch (PostgresException ex)
        {
            _unitOfWork.RollbackTransaction();
            throw ex;
        }
    }
    public async Task<List<NotificationDto>> AddNotificationsAsync(Guid actorId, List<Guid> receiverIds, NotificationAction action
        , NotificationEntityType entityType, NotificationStatus status = NotificationStatus.UnRead
        , Guid? entityId = null, Guid? locationId = null, string locationHashId = "", string entityhashId = "")
    {
        try
        {
            var response = new List<NotificationDto>();

            var notiObj = new NotificationObject()
            {
                EntityType = entityType,
                Action = action,
                EntityId = entityId,
                EntityHashId = entityhashId,
                ActorId = actorId,
                LocationId = locationId,
                LocationHashId = locationHashId,
                CreatedBy = actorId,
            };

            await _notiObjectRepository.InsertAsync(notiObj);

            foreach (var receiverId in receiverIds)
            {
                var dto = new NotificationDto();

                var noti = new Notification()
                {
                    NotificationObjectId = notiObj.Id,
                    ReceiverId = receiverId,
                    Status = status
                };
                await _notiRepository.InsertAsync(noti);

                dto.Id = noti.Id;
                dto.Status = ((NotificationStatus)noti.Status).ToString();
                dto.NotificationObjectId = notiObj.Id;
                dto.ReceiverId = receiverId;
                dto.ActorId = actorId;
                dto.CreatedOn = noti.CreatedOn;

                response.Add(dto);
            }

            return response;

        }
        catch (PostgresException ex)
        {
            _unitOfWork.RollbackTransaction();
            throw ex;
        }
    }

    public async Task AddTransactionUpdate(RealTimeTransactionUpdateReq req)
    {
        if (req != null && req.UserId != Guid.Empty && !string.IsNullOrEmpty(req.TransactionId))
        {
            await _hubcontext.Clients.Group(req.UserId.ToString()).SendAsync(RealTimeTopic.ReceiveTransactionUpdate, JsonConvert.SerializeObject(req));
        }
    }
    public async Task AddCommonNotification(CommonNotificationReq req)
    {
        if (req != null && !string.IsNullOrEmpty(req.TopicName) && !string.IsNullOrEmpty(req.Message))
        {
            if (!string.IsNullOrEmpty(req.UserId))
            {
                await _hubcontext.Clients.Group(req.UserId).SendAsync(req.TopicName, req.Message);
            }
            else
            {
                await _hubcontext.Clients.All.SendAsync(req.TopicName, req.Message);
            }

        }
    }

    private string GetVideoMessage(NotificationAction action)
    {
        return action switch
        {
            NotificationAction.Processing => NotificationContent.VideoUploadProcessing,
            NotificationAction.Completed => NotificationContent.VideoUploadCompleted,
            NotificationAction.Failed => NotificationContent.VideoUploadFailed,
            _ => throw new NotSupportedException($"Unsupported video action: {action}"),
        };
    }

    public async Task<NotificationResponse> FollowNotification(UserFollowResp followResp)
    {
        var response = new NotificationResponse();

        if (followResp.CreatedByUserId != Guid.Empty)
        {
            var receiverId = followResp.FollowedId;

            if (receiverId != followResp.CreatedByUserId)
            {
                var notiObj = await (from a in _context.NotificationObjectAvailable
                                     join b in _context.Notifications on a.Id equals b.NotificationObjectId
                                     where a.ActorId == followResp.CreatedByUserId
                                           && b.ReceiverId == followResp.FollowedId
                                           && a.Action == NotificationAction.FollowUser
                                     select new
                                     {
                                         a.Id,
                                         a.CreatedOn
                                     }).FirstOrDefaultAsync();

                if (notiObj != null)
                {
                    await _context.NotificationAvailable
                        .Where(p => p.NotificationObjectId == notiObj.Id)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(p => p.Status, p => NotificationStatus.UnRead)
                            .SetProperty(p => p.CreatedOn, p => DateTime.UtcNow)
                            .SetProperty(p => p.ModifiedOn, p => DateTime.UtcNow));
                    await _context.NotificationObjectAvailable
                        .Where(p => p.Id == notiObj.Id)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(p => p.CreatedOn, p => DateTime.UtcNow)
                            .SetProperty(p => p.ModifiedOn, p => DateTime.UtcNow));
                }
                else
                {
                    var noti = await AddNotificationAsync(
                                        actorId: followResp.CreatedByUserId
                                        , receiverId: receiverId
                                        , action: NotificationAction.FollowUser
                                        , entityType: NotificationEntityType.FollowUser
                                        , locationHashId: followResp.CreatedByUserName);

                    response.Id = noti.Id;
                    response.Status = noti.Status;
                    response.LocationId = null;
                    response.LocationHashId = "";
                    response.EntityId = null;
                    response.Message = followResp.CreatedByUserName + NotificationContent.FollowUser;
                    response.TargetType = Common.Core.Constants.Setting.NotificationTargetType.FollowUser;
                    response.ActorId = followResp.CreatedByUserId;
                    response.ActorName = followResp.CreatedByUserName;
                    response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
                    response.NotificationType = Common.Core.Constants.Setting.NotificationType.Comment;
                    response.UserAvatar = followResp.CreatedByUserAvata;

                    await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
                    return response;
                }
            }
        }

        return response;
    }
}
