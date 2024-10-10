namespace Mcsg.Realtime.Api.Interfaces;

using Common.Core.Enums;
using Common.Core.Requests;
using Dtos;
using Lib.Common.Models.RealTime;
using Requests;

public interface INotificationService
{
    Task<NotificationResponse> AddCommentNotification(CommentNotificationReq comment);
    Task<NotificationResponse> AddReplyNotification(CommentNotificationReq comment);
    Task<NotificationResponse> AddVideoNotification(VideoNotificationR video);
    Task<NotificationResponse> AddReactionNotification(ReactionNotificationReq reaction);
    Task<NotificationResponse> AddMentionNotification(MentionNotificationReq mention);
    Task<NotificationDto> AddNotificationAsync(Guid actorId, Guid receiverId, NotificationAction action, NotificationEntityType entityType, NotificationStatus status = NotificationStatus.UnRead, Guid? entityId = null, Guid? locationId = null, string locationHashId = "", string entityhashId = "");
    Task<List<NotificationDto>> AddNotificationsAsync(Guid actorId, List<Guid> receiverIds, NotificationAction action, NotificationEntityType entityType, NotificationStatus status = NotificationStatus.UnRead, Guid? entityId = null, Guid? locationId = null, string locationHashId = "", string entityhashId = "");
    Task AddTransactionUpdate(RealTimeTransactionUpdateReq req);
    Task AddCommonNotification(CommonNotificationReq req);
    Task<NotificationResponse> FollowNotification(UserFollowResp followResp);
    Task<NotificationResponse> AddFollowPostNotification(FollowPostNotificationReq request);
    Task AddPostMentionNotification(MentionPostNotificationReq request);
    Task<NotificationResponse> AddTransactionNotification(TransactionNotificationReq req);
}
