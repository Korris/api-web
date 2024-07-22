namespace Mcsg.Realtime.Api.Requests
{
    using Common.Core.Enums;

    public class MentionNotificationReq
    {
        public Guid LocationId { get; set; }
        public MentionLocationType LocationType { get; set; }
        public Guid EntityId { get; set; }
        public MentionEntityType EntityType { get; set; }
        public string PostHashId { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string UserAvatar { get; set; }
    }
}
