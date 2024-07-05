namespace Mcsg.Realtime.Api.Requests
{
    using Lib.Data.Enums;

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
