namespace Mcsg.Comic.Api.Extensions;

using Common.Core.Enums;
using Lib.Common.Constants;
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
            NotificationEntityType.Video => Common.Core.Constants.Setting.NotificationTargetType.Feed,
            NotificationEntityType.PostComment => Common.Core.Constants.Setting.NotificationTargetType.CommentOnFeed,
            NotificationEntityType.SubPostComment => Common.Core.Constants.Setting.NotificationTargetType.CommentOnSubFeed,
            NotificationEntityType.PostCommentReply => Common.Core.Constants.Setting.NotificationTargetType.ReplyOnFeed,
            NotificationEntityType.SubPostCommentReply => Common.Core.Constants.Setting.NotificationTargetType.ReplyOnSubFeed,
            NotificationEntityType.PostReaction => Common.Core.Constants.Setting.NotificationTargetType.Feed,
            NotificationEntityType.PostCommentMention => Common.Core.Constants.Setting.NotificationTargetType.CommentOnFeed,
            NotificationEntityType.SubPostCommentMention => Common.Core.Constants.Setting.NotificationTargetType.CommentOnSubFeed,
            _ => throw new NotSupportedException($"Unsupported entity type: {noti.EntityType}"),
        };
    }
    public static string ToNotiType(this NotificationQueryResult noti)
    {
        return noti.EntityType switch
        {
            NotificationEntityType.Video => Common.Core.Constants.Setting.NotificationType.Video + noti.Action.ToString(),
            NotificationEntityType.PostComment => Common.Core.Constants.Setting.NotificationType.Comment,
            NotificationEntityType.SubPostComment => Common.Core.Constants.Setting.NotificationType.Comment,
            NotificationEntityType.PostCommentReply => Common.Core.Constants.Setting.NotificationType.Reply,
            NotificationEntityType.SubPostCommentReply => Common.Core.Constants.Setting.NotificationType.Reply,
            NotificationEntityType.PostReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.SubPostReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.PostCommentReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.SubPostCommentReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.Feed => Common.Core.Constants.Setting.NotificationType.Feed,
            NotificationEntityType.PostCommentMention => Common.Core.Constants.Setting.NotificationType.Mention,
            NotificationEntityType.SubPostCommentMention => Common.Core.Constants.Setting.NotificationType.Mention,
            _ => throw new NotSupportedException($"Unsupported entity type: {noti.EntityType}"),
        };
    }

    public static string ToDisplay(this NotificationStatus value)
    {
        var enumDisplayStatus = (NotificationStatus)value;
        return enumDisplayStatus.ToString();
    }
}
