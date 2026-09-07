namespace Mcsg.Api.Areas.Comic.Requests;

using Common.Core.Enums;

public class ReactionNotificationReq
{
    public Guid Id { get; set; }
    public Guid TargetId { get; set; }
    public Guid AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public string? UserAvatar { get; set; }
    public NotificationEntityType EntityType { get; set; } = NotificationEntityType.ComicPostReaction;
    public ReactionType ReactionType { get; set; }
    public bool IsReplyReaction { get; set; }
}
