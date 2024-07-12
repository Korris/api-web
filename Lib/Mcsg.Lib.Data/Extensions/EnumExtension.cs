namespace Mcsg.Lib.Data.Extensions;

using Common.Core.Enums;
using Domain.Entities;
using Enums;

public static class EnumExtension
{
    public static string ToTable(this NotificationEntityType notiEntityType)
    {
        switch (notiEntityType)
        {
            case NotificationEntityType.User: return $"{nameof(User)}s";
            case NotificationEntityType.Feed: return $"{nameof(Post)}s";
            case NotificationEntityType.Story: return $"{nameof(Post)}s";
            case NotificationEntityType.Comic: return $"{nameof(Post)}s";
            case NotificationEntityType.PostComment: return $"{nameof(PostComment)}s";
            case NotificationEntityType.PostCommentReply: return $"{nameof(PostComment)}s";
            case NotificationEntityType.SubPostComment: return $"{nameof(SubPostComment)}s";
            case NotificationEntityType.SubPostCommentReply: return $"{nameof(SubPostComment)}s";
            case NotificationEntityType.PostReaction: return $"{nameof(PostReaction)}s";
            case NotificationEntityType.SubPostReaction: return $"{nameof(SubPostReaction)}s";
            case NotificationEntityType.PostCommentReaction: return $"{nameof(PostCommentReaction)}s";
            case NotificationEntityType.SubPostCommentReaction: return $"{nameof(SubPostCommentReaction)}s";
            case NotificationEntityType.Video: return $"{nameof(Resource)}s";

            default: throw new ArgumentOutOfRangeException("NotificationEntityType");
        }
    }
    public static string ToMessage(this NotificationAction notiEntityType)
    {
        switch (notiEntityType)
        {
            case NotificationAction.Create: return $"created";
            case NotificationAction.Update: return $"updated";
            case NotificationAction.Delete: return $"deleted";
            case NotificationAction.Processing: return $"processing";
            case NotificationAction.Completed: return $"completed";
            case NotificationAction.Failed: return $"failed";
            case NotificationAction.Comment: return $"commented";
            case NotificationAction.Reply: return $"replied";
            case NotificationAction.Reaction: return $"reacted";
            case NotificationAction.Mention: return $"mentioned";
            default: throw new ArgumentOutOfRangeException("NotificationAction");
        }
    }
}
