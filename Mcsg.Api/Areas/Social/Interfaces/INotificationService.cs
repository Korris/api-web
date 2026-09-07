namespace Mcsg.Api.Areas.Social.Interfaces;

using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Requests;

public interface INotificationService
{
    Task<bool> ReadNotificationAsync(NotificationUpdateR request);
    Task<bool> ReadAllNotificationAsync(Guid? userId);
    Task<PagedResponse<Notification.SearchDto>> GetNotificationByReceiverAsync(NotificationR request);
    Task<PagedResponse<Notification.SearchDto>> GetUnReadNotificationByReceiverAsync(NotificationR request);
    Task<bool> AddReactionNotificationAsync(ReactionNotificationReq req);
    Task<bool> AddMentionNotificationAsync(MentionPostNotificationReq req);
}
