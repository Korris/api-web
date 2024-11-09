namespace Mcsg.Realtime.Api.Interfaces;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.Models.RealTime;
using Dtos;
using Requests;

public interface IStoryNotificationService
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
}
