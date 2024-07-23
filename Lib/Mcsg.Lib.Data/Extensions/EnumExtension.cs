namespace Mcsg.Lib.Data.Extensions;

using Common.Core.Enums;
using Mcsg.Common.Domain.Entities;

public static class EnumExtension
{
    public static string ToTable(this NotificationEntityType notiEntityType)
    {
        switch (notiEntityType)
        {
            case NotificationEntityType.User: return $"{nameof(User)}s";
            case NotificationEntityType.Feed: return $"{nameof(SocialPost)}s";
            case NotificationEntityType.Story: return $"{nameof(SocialPost)}s";
            case NotificationEntityType.Comic: return $"{nameof(SocialPost)}s";
            case NotificationEntityType.PostComment: return $"{nameof(SocialPostComment)}s";
            case NotificationEntityType.PostCommentReply: return $"{nameof(SocialPostComment)}s";
            case NotificationEntityType.SubPostComment: return $"{nameof(SocialSubPostComment)}s";
            case NotificationEntityType.SubPostCommentReply: return $"{nameof(SocialSubPostComment)}s";
            case NotificationEntityType.PostReaction: return $"{nameof(SocialPostReaction)}s";
            case NotificationEntityType.SubPostReaction: return $"{nameof(SocialSubPostReaction)}s";
            case NotificationEntityType.PostCommentReaction: return $"{nameof(SocialPostCommentReaction)}s";
            case NotificationEntityType.SubPostCommentReaction: return $"{nameof(SocialSubPostCommentReaction)}s";
            case NotificationEntityType.Video: return $"{nameof(SocialResource)}s";

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
