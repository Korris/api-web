namespace Mcsg.Api.Areas.Social.Requests;

using Common.Core.Enums;

public class MentionPostNotificationReq
{
    public Guid TargetId { get; set; }
    public NotificationEntityType EntityType { get; set; } = NotificationEntityType.SocialPostMention;
    public Guid UserId { get; set; }
    public string UserAvatar { get; set; }
    public string UserProfileName { get; set; }
    public List<Guid> ReceiversId { get; set; } = new List<Guid>();
}