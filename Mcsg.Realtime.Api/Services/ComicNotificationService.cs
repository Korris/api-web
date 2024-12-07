using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Mcsg.Realtime.Api.Services;

using Common.Constants;
using Common.Core.Enums;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.Models.RealTime;
using Constants;
using Dtos;
using Hubs;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Requests;

public class ComicNotificationService : BaseS, IComicNotificationService
{
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="hubcontext"></param>
    public ComicNotificationService(IMcsgContext context, IHubContext<NotificationHub> hubcontext) : base(context)
    {
        _hubcontext = hubcontext;
    }

    public async Task<NotificationResponse> AddCommentNotification(CommentNotificationReq comment)
    {
        var response = new NotificationResponse();

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
                response.Message = comment.AuthorName + NotificationContent.CommentOnFeed;
                response.TargetType = comment.Type == PostTypes.Post ? Common.Core.Constants.Setting.NotificationTargetType.CommentOnFeed : Common.Core.Constants.Setting.NotificationTargetType.CommentOnSubFeed;
                response.ActorId = comment.AuthorId;
                response.ActorName = comment.AuthorName;
                response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
                response.NotificationType = Common.Core.Constants.Setting.NotificationType.Comment;
                response.UserAvatar = comment.UserAvatar;

                // Then notification the comment to post owner
                await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
            }
        }

        return response;
    }

    public async Task<NotificationResponse> AddReplyNotification(CommentNotificationReq comment)
    {
        var response = new NotificationResponse();
        var receiverId = Guid.Empty;
        var id = comment.QuoteId ?? comment.ReplyToCommentId;

        if (comment.Type == PostTypes.Post)
        {
            var parentComment = await _context.Available<ComicPostComment>().FirstOrDefaultAsync(p => p.Id == id);
            receiverId = parentComment.AuthorId;
        }
        else
        {
            var parentComment = await _context.Available<ComicSubPostComment>().FirstOrDefaultAsync(p => p.Id == id);
            receiverId = parentComment.AuthorId;
        }

        // Dont notify when comment on their feed
        if (receiverId != Guid.Empty && receiverId != comment.AuthorId)
        {
            IQueryable<string?> qHashId;
            if (comment.Type == PostTypes.Post)
            {
                qHashId = _context.Available<ComicPost>().Where(p => p.Id == comment.PostId).Select(p => p.HashId);
            }
            else
            {
                qHashId = _context.Available<ComicSubPost>().Where(p => p.Id == comment.PostId).Select(p => p.HashId);
            }
            var hashId = await qHashId.FirstOrDefaultAsync();

            var noti = await AddNotificationAsync(
                                        actorId: comment.AuthorId
                                        , receiverId: receiverId
                                        , action: comment.IsReply ? NotificationAction.Reply : NotificationAction.Comment
                                        , entityType: comment.EntityType
                                        , entityId: comment.Id
                                        , locationId: comment.PostId
                                        , locationHashId: hashId);

            response.Id = noti.Id;
            response.Status = noti.Status;
            response.LocationId = comment.PostId;
            response.LocationHashId = comment.PostHashId ?? hashId;
            response.EntityId = comment.Id;
            response.Message = comment.AuthorName + NotificationContent.ReplyOnComment;
            response.TargetType = comment.Type == PostTypes.Post ? Common.Core.Constants.Setting.NotificationTargetType.Comic : Common.Core.Constants.Setting.NotificationTargetType.SubComic;
            response.ActorId = comment.AuthorId;
            response.ActorName = comment.AuthorName;
            response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
            response.NotificationType = Common.Core.Constants.Setting.NotificationType.Reply;
            response.UserAvatar = comment.UserAvatar;
            response.Order = comment?.Order ?? 0;
            /// this Id is ReplyCommentId
            response.ReplyCommentId = comment.Id;
            response.CommentId = comment.CommentId;
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
        if (reaction.EntityType == NotificationEntityType.SocialPostReaction)
        {
            var post = await _context.Available<ComicPost>().FirstOrDefaultAsync(p => p.Id == reaction.TargetId);
            if (post != null)
            {
                receiverId = post.CreatedBy != null ? post.CreatedBy.Value : Guid.Empty;
                postId = post.Id;
                postHashId = post.HashId;

                response.Message = reaction.AuthorName + NotificationContent.ReactOnFeed;
                response.TargetType = Common.Core.Constants.Setting.NotificationTargetType.Social;
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
            var entityType = NotificationEntityType.SocialPostCommentMention;
            var targetType = Common.Core.Constants.Setting.NotificationTargetType.CommentOnFeed;
            if (mention.LocationType == MentionLocationType.SubPostComment || mention.LocationType == MentionLocationType.SubPostCommentReply)
            {
                entityType = NotificationEntityType.SocialSubPostCommentMention;
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

    /// <summary>
    /// AddNotificationAsync
    /// </summary>
    /// <param name="actorId"></param>
    /// <param name="receiverId"></param>
    /// <param name="action"></param>
    /// <param name="entityType"></param>
    /// <param name="status"></param>
    /// <param name="entityId"></param>
    /// <param name="locationId"></param>
    /// <param name="locationHashId"></param>
    /// <param name="entityhashId"></param>
    /// <returns></returns>
    public async Task<NotificationDto> AddNotificationAsync(Guid actorId, Guid receiverId, NotificationAction action, NotificationEntityType entityType, NotificationStatus status = NotificationStatus.UnRead, Guid? entityId = null, Guid? locationId = null, string locationHashId = "", string entityhashId = "")
    {
        var notiObj = new NotificationObject
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
        await _context.NotificationObjects.AddAsync(notiObj);

        var noti = new Notification
        {
            NotificationObjectId = notiObj.Id,
            ReceiverId = receiverId,
            Status = status
        };
        await _context.Notifications.AddAsync(noti);
        await _context.SaveChangesAsync(default);

        var res = new NotificationDto
        {
            Id = noti.Id,
            Status = noti.Status,
            NotificationObjectId = notiObj.Id,
            ReceiverId = receiverId,
            ActorId = actorId,
            CreatedOn = noti.CreatedOn
        };

        return res;
    }

    /// <summary>
    /// AddNotificationsAsync
    /// </summary>
    /// <param name="actorId"></param>
    /// <param name="receiverIds"></param>
    /// <param name="action"></param>
    /// <param name="entityType"></param>
    /// <param name="status"></param>
    /// <param name="entityId"></param>
    /// <param name="locationId"></param>
    /// <param name="locationHashId"></param>
    /// <param name="entityhashId"></param>
    /// <returns></returns>
    public async Task<List<NotificationDto>> AddNotificationsAsync(Guid actorId, List<Guid> receiverIds, NotificationAction action, NotificationEntityType entityType, NotificationStatus status = NotificationStatus.UnRead, Guid? entityId = null, Guid? locationId = null, string locationHashId = "", string entityhashId = "")
    {
        var res = new List<NotificationDto>();

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
        await _context.NotificationObjects.AddAsync(notiObj);

        foreach (var receiverId in receiverIds)
        {
            var noti = new Notification
            {
                NotificationObjectId = notiObj.Id,
                ReceiverId = receiverId,
                Status = status
            };
            await _context.Notifications.AddAsync(noti);

            var dto = new NotificationDto
            {
                Id = noti.Id,
                Status = noti.Status,
                NotificationObjectId = notiObj.Id,
                ReceiverId = receiverId,
                ActorId = actorId,
                CreatedOn = noti.CreatedOn
            };
            res.Add(dto);
        }

        await _context.SaveChangesAsync(default);

        return res;
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

    #region -- Fields --

    private readonly IHubContext<NotificationHub> _hubcontext;

    #endregion
}
