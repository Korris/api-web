namespace Mcsg.Social.Api.Extensions;

using Constants;
using Lib.Common.Constants;
using Lib.Data.Enums;
using Models;

public static class NotificationExtension
{
    public static string ToMessage(this NotificationQueryResult noti)
    {
        List<NotificationEntityType> commentEntities = new List<NotificationEntityType>()
        {
            NotificationEntityType.PostComment,
            NotificationEntityType.SubPostComment,
            NotificationEntityType.PostCommentReply,
            NotificationEntityType.SubPostCommentReply,
            NotificationEntityType.PostCommentReaction,
            NotificationEntityType.SubPostCommentReaction,
        };

        List<NotificationEntityType> postEntities = new List<NotificationEntityType>()
        {
            NotificationEntityType.Feed,
            NotificationEntityType.Comic,
            NotificationEntityType.Story,
            NotificationEntityType.PostReaction,
            NotificationEntityType.SubPostReaction
        };

        if (noti == null)
        {
            return string.Empty;
        }

        #region Video 

        if (noti.EntityType == NotificationEntityType.Video
                && noti.Action == NotificationAction.Processing)
        {
            return NotificationContent.VideoUploadProcessing;
        }

        if (noti.EntityType == NotificationEntityType.Video
                && noti.Action == NotificationAction.Completed)
        {
            return NotificationContent.VideoUploadCompleted;
        }

        if (noti.EntityType == NotificationEntityType.Video
                && noti.Action == NotificationAction.Failed)
        {
            return NotificationContent.VideoUploadFailed;
        }

        #endregion

        #region Comment 

        if (commentEntities.Contains(noti.EntityType)
                && noti.Action == NotificationAction.Mention)
        {
            return noti.ActorName + NotificationContent.MentionOnComment;
        }

        if (commentEntities.Contains(noti.EntityType)
                && noti.Action == NotificationAction.Reaction)
        {
            return noti.ActorName + NotificationContent.ReactOnComment;
        }

        if (commentEntities.Contains(noti.EntityType)
                && noti.Action == NotificationAction.Reply)
        {
            return noti.ActorName + NotificationContent.ReplyOnComment;
        }

        if (commentEntities.Contains(noti.EntityType)
                && noti.Action == NotificationAction.Comment)
        {
            return noti.ActorName + NotificationContent.CommentOnFeed;
        }
        #endregion

        #region Post 

        if (postEntities.Contains(noti.EntityType)
                && noti.Action == NotificationAction.Comment)
        {
            return noti.ActorName + NotificationContent.CommentOnFeed;
        }

        if (postEntities.Contains(noti.EntityType)
                && noti.Action == NotificationAction.Reaction)
        {
            return noti.ActorName + NotificationContent.ReactOnFeed;
        }

        #endregion

        #region Mention
        if (noti.EntityType == NotificationEntityType.PostCommentMention
                && noti.Action == NotificationAction.Mention)
        {
            return noti.ActorName + NotificationContent.MentionOnComment;
        }
        if (noti.EntityType == NotificationEntityType.SubPostCommentMention
                && noti.Action == NotificationAction.Mention)
        {
            return noti.ActorName + NotificationContent.MentionOnReply;
        }
        #endregion

        return "";
    }

    public static string ToTargetType(this NotificationQueryResult noti)
    {
        return noti.EntityType switch
        {
            NotificationEntityType.Video => NotificationTargetType.Feed,
            NotificationEntityType.PostComment => NotificationTargetType.CommentOnFeed,
            NotificationEntityType.SubPostComment => NotificationTargetType.CommentOnSubFeed,
            NotificationEntityType.PostCommentReply => NotificationTargetType.ReplyOnFeed,
            NotificationEntityType.SubPostCommentReply => NotificationTargetType.ReplyOnSubFeed,
            NotificationEntityType.PostReaction => NotificationTargetType.Feed,
            NotificationEntityType.PostCommentMention => NotificationTargetType.CommentOnFeed,
            NotificationEntityType.SubPostCommentMention => NotificationTargetType.CommentOnSubFeed,
            _ => throw new NotSupportedException($"Unsupported entity type: {noti.EntityType}"),
        };
    }
    public static string ToNotiType(this NotificationQueryResult noti)
    {
        return noti.EntityType switch
        {
            NotificationEntityType.Video => NotificationType.Video + noti.Action.ToString(),
            NotificationEntityType.PostComment => NotificationType.Comment,
            NotificationEntityType.SubPostComment => NotificationType.Comment,
            NotificationEntityType.PostCommentReply => NotificationType.Reply,
            NotificationEntityType.SubPostCommentReply => NotificationType.Reply,
            NotificationEntityType.PostReaction => NotificationType.Reaction,
            NotificationEntityType.SubPostReaction => NotificationType.Reaction,
            NotificationEntityType.PostCommentReaction => NotificationType.Reaction,
            NotificationEntityType.SubPostCommentReaction => NotificationType.Reaction,
            NotificationEntityType.Feed => NotificationType.Feed,
            NotificationEntityType.PostCommentMention => NotificationType.Mention,
            NotificationEntityType.SubPostCommentMention => NotificationType.Mention,
            _ => throw new NotSupportedException($"Unsupported entity type: {noti.EntityType}"),
        };
    }

    public static string ToDisplay(this NotificationStatus value)
    {
        var enumDisplayStatus = (NotificationStatus)value;
        return enumDisplayStatus.ToString();
    }
}
