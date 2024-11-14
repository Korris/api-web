namespace Mcsg.Social.Api.Interfaces;

using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Requests;

public interface INotificationService
{
    Task<bool> ReadNotificationAsync(NotificationUpdateR request);
    Task<bool> ReadAllNotificationAsync(Guid? userId);
    Task<PagedResponse<Notification.SearchDto>> GetNotificationByReceiverAsync(NotificationR request);
    Task<PagedResponse<Notification.SearchDto>> GetUnReadNotificationByReceiverAsync(NotificationR request);
    Task<bool> AddReactionNotificationAsync(ReactionNotificationReq req);
    Task<bool> AddMentionNotificationAsync(MentionPostNotificationReq req);
}
