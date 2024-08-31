namespace Mcsg.Comic.Api.Requests;

using Common.Core.Enums;

public class ReactionNotificationReq
{
    public Guid Id { get; set; }
    public Guid TargetId { get; set; }
    public Guid AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public string? UserAvatar { get; set; }
    public NotificationEntityType EntityType { get; set; } = NotificationEntityType.PostReaction;
    public ReactionType ReactionType { get; set; }
}
