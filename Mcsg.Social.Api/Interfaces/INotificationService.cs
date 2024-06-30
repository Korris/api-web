namespace Mcsg.Social.Api.Interfaces
{
    using DTOs;
    using Lib.Data.Entities.Common;
    using Models;

    public interface INotificationService
    {
        Task<NotificationModel> GetNotificationAsync(Guid id);
        Task<bool> ReadNotificationAsync(Guid id);
        Task<bool> ReadAllNotificationAsync();
        Task<PagedResults<NotificationModel>> GetNotificationByReceiverAsync(NotificationReq request);
        Task<PagedResults<NotificationModel>> GetUnReadNotificationByReceiverAsync(NotificationReq request);
        Task<bool> AddVideoNotificationAsync(VideoNotificationReq req);
        Task<bool> AddReactionNotificationAsync(ReactionNotificationReq req);
    }
}
