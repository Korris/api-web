namespace Mcsg.Comic.Api.Extensions;

using Common.Constants;
using Common.Core.Enums;
using Models;

public static class NotificationExtension
{
    public static string ToMessage(this NotificationQueryResult noti)
    {
        List<NotificationEntityType> commentEntities =
        [
            NotificationEntityType.SocialPostComment,
            NotificationEntityType.SocialSubPostComment,
            NotificationEntityType.SocialPostCommentReply,
            NotificationEntityType.SocialSubPostCommentReply,
            NotificationEntityType.SocialPostCommentReaction,
            NotificationEntityType.SocialSubPostCommentReaction,
        ];

        List<NotificationEntityType> postEntities =
        [
            NotificationEntityType.SocialPostReaction,
            NotificationEntityType.SocialSubPostReaction
        ];

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
        if (noti.EntityType == NotificationEntityType.SocialPostCommentMention
                && noti.Action == NotificationAction.Mention)
        {
            return noti.ActorName + NotificationContent.MentionOnComment;
        }
        if (noti.EntityType == NotificationEntityType.SocialSubPostCommentMention
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
            NotificationEntityType.SocialPostComment => Common.Core.Constants.Setting.NotificationTargetType.CommentOnFeed,
            NotificationEntityType.SocialSubPostComment => Common.Core.Constants.Setting.NotificationTargetType.CommentOnSubFeed,
            NotificationEntityType.SocialPostCommentReply => Common.Core.Constants.Setting.NotificationTargetType.ReplyOnFeed,
            NotificationEntityType.SocialSubPostCommentReply => Common.Core.Constants.Setting.NotificationTargetType.ReplyOnSubFeed,
            NotificationEntityType.SocialPostReaction => Common.Core.Constants.Setting.NotificationTargetType.Feed,
            NotificationEntityType.SocialPostCommentMention => Common.Core.Constants.Setting.NotificationTargetType.CommentOnFeed,
            NotificationEntityType.SocialSubPostCommentMention => Common.Core.Constants.Setting.NotificationTargetType.CommentOnSubFeed,
            _ => throw new NotSupportedException($"Unsupported entity type: {noti.EntityType}"),
        };
    }

    public static string ToNotiType(this NotificationQueryResult noti)
    {
        return noti.EntityType switch
        {
            NotificationEntityType.Video => Common.Core.Constants.Setting.NotificationType.Video + noti.Action.ToString(),
            NotificationEntityType.SocialPostComment => Common.Core.Constants.Setting.NotificationType.Comment,
            NotificationEntityType.SocialSubPostComment => Common.Core.Constants.Setting.NotificationType.Comment,
            NotificationEntityType.SocialPostCommentReply => Common.Core.Constants.Setting.NotificationType.Reply,
            NotificationEntityType.SocialSubPostCommentReply => Common.Core.Constants.Setting.NotificationType.Reply,
            NotificationEntityType.SocialPostReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.SocialSubPostReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.SocialPostCommentReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.SocialSubPostCommentReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.SocialPostCommentMention => Common.Core.Constants.Setting.NotificationType.Mention,
            NotificationEntityType.SocialSubPostCommentMention => Common.Core.Constants.Setting.NotificationType.Mention,
            _ => throw new NotSupportedException($"Unsupported entity type: {noti.EntityType}"),
        };
    }
}
