namespace Mcsg.Document.Api.Interfaces;

using Common.SeedWork.Responses;
using Models;
using Requests;

public interface INotificationService
{
    Task<bool> ReadNotificationAsync(Guid id);
    Task<bool> ReadAllNotificationAsync();
    Task<PagedResponse<NotificationModel>> GetNotificationByReceiverAsync(NotificationR request);
    Task<PagedResponse<NotificationModel>> GetUnReadNotificationByReceiverAsync(NotificationR request);
    Task<bool> AddReactionNotificationAsync(ReactionNotificationReq req);
    Task AddTrackingFollowAsync(TrackingFollowReq req);
}
