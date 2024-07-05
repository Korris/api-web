namespace Mcsg.Realtime.Api.Requests
{
    using Lib.Data.Enums;

    public class VideoNotificationReq
    {
        public Guid Id { get; set; }
        public NotificationAction Action { get; set; }
        public string? HashId { get; set; }
        public Guid? PostId { get; set; }
        public string? PostHashId { get; set; }
        public string? TargetType { get; set; }
        public Guid AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public string? UserAvatar { get; set; }
    }
}
