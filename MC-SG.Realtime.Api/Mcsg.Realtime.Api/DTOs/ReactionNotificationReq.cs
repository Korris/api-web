using Mcsg.Lib.Data.Enums;

namespace Mcsg.Realtime.Api.DTOs
{
    public class ReactionNotificationReq
    {
        public Guid Id { get; set; }
        public Guid TargetId { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public NotificationEntityType EntityType { get; set; } = NotificationEntityType.PostReaction;
        public string UserAvatar { get; set; }
    }
}
