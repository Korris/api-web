namespace Mcsg.Api.Areas.Realtime.Interfaces;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.Models.RealTime;
using Mcsg.Api.Areas.Realtime.Dtos;
using Mcsg.Api.Areas.Realtime.Requests;

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
    Task<NotificationResponse> AddRejecton(NotificationAddRejectionR request);
    Task AddPostMentionNotification(MentionPostNotificationReq request);
    Task<NotificationResponse> AddTransactionNotification(TransactionNotificationReq req);
    Task<NotificationResponse> AddDeletion(NotificationAddDeletionR request);
    Task<NotificationResponse> AddLock(NotificationAddLockR request);
    Task RemindExpiredSubscriptionNotification(RemindExpiredSubscriptionR request);
    Task ExpiredSubscriptionNotification(ExpiredSubscriptionR request);
    Task AddSubPostNotification(NotificationAddSubPostR request);
}
