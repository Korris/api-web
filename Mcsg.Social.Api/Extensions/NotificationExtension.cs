namespace Mcsg.Social.Api.Extensions;

using Common.Constants;
using Common.Core.Enums;
using Models;
using Message = Common.Core.Constants.Message;

public static class NotificationExtension
{
    public static string ToMessage(this NotificationQueryResult noti)
    {
        List<NotificationEntityType> commentEntities = new List<NotificationEntityType>()
        {
            NotificationEntityType.SocialPostComment,
            NotificationEntityType.ComicPostComment,
            NotificationEntityType.StoryPostComment,
            NotificationEntityType.SocialSubPostComment,
            NotificationEntityType.ComicSubPostComment,
            NotificationEntityType.StorySubPostComment,
            NotificationEntityType.SocialPostCommentReply,
            NotificationEntityType.ComicPostCommentReply,
            NotificationEntityType.StoryPostCommentReply,
            NotificationEntityType.SocialSubPostCommentReply,
            NotificationEntityType.ComicSubPostCommentReply,
            NotificationEntityType.StorySubPostCommentReply
        };

        List<NotificationEntityType> rejectReportEntities = new List<NotificationEntityType>()
        {
            NotificationEntityType.RejectPostReport,
            NotificationEntityType.RejectCommentReport
        };

        List<NotificationEntityType> reactionEntities = new List<NotificationEntityType>()
        {
            NotificationEntityType.SocialPostReaction,
            NotificationEntityType.StoryPostReaction,
            NotificationEntityType.ComicPostReaction,
            NotificationEntityType.SocialSubPostReaction,
            NotificationEntityType.SocialPostCommentReaction,
            NotificationEntityType.SocialSubPostCommentReaction,
            NotificationEntityType.ComicPostCommentReaction,
            NotificationEntityType.StoryPostCommentReaction,
            NotificationEntityType.ComicSubPostCommentReaction,
            NotificationEntityType.StorySubPostCommentReaction,
            NotificationEntityType.SocialPostCommentReplyReaction,
            NotificationEntityType.SocialSubPostCommentReplyReaction,
            NotificationEntityType.ComicPostCommentReplyReaction,
            NotificationEntityType.ComicSubPostCommentReplyReaction,
            NotificationEntityType.StoryPostCommentReplyReaction,
            NotificationEntityType.StorySubPostCommentReplyReaction,
        };

        List<NotificationEntityType> followEntities = new List<NotificationEntityType>()
        {
            NotificationEntityType.FollowUser,
            NotificationEntityType.FollowComicPost,
            NotificationEntityType.FollowStoryPost,
        };

        List<NotificationEntityType> deleteEntities = new List<NotificationEntityType>()
        {
            NotificationEntityType.DeleteSocial,
            NotificationEntityType.DeleteComicPost,
            NotificationEntityType.DeleteStoryPost,
            NotificationEntityType.DeleteComicSubPost,
            NotificationEntityType.DeleteStorySubPost
        };

        List<NotificationEntityType> lockEntities = new List<NotificationEntityType>()
        {
            NotificationEntityType.LockSocial,
            NotificationEntityType.LockComicPost,
            NotificationEntityType.LockStoryPost,
            NotificationEntityType.LockComicSubPost,
            NotificationEntityType.LockStorySubPost
        };

        if (noti == null)
        {
            return string.Empty;
        }

        var notiString = noti.EntityType.ToString();

        if (notiString.Contains("Mention"))
        {
            if (notiString.Contains("Comment"))
            {
                return noti.ActorName + NotificationContent.MentionOnComment;
            }
            return noti.ActorName + NotificationContent.MentionOnPost;
        }

        #region Reaction
        if (reactionEntities.Contains(noti.EntityType))
        {
            if (noti.EntityType.ToString().Contains("Comment"))
            {
                return noti.ActorName + NotificationContent.ReactOnComment;
            }
            else
            {
                var message = noti.EntityType == NotificationEntityType.SocialPostReaction ? NotificationContent.ReactOnFeed : NotificationContent.ReactOnComic;
                return noti.ActorName + message;
            }
        }
        #endregion

        #region Report
        if (rejectReportEntities.Contains(noti.EntityType))
        {
            return noti.EntityType == NotificationEntityType.RejectCommentReport ? nameof(Message.S303) : nameof(Message.S304);
        }
        #endregion

        #region Follow 

        if (followEntities.Contains(noti.EntityType))
        {
            switch (noti.EntityType)
            {
                case NotificationEntityType.FollowUser:
                    return noti.ActorName + NotificationContent.FollowUser;
                case NotificationEntityType.FollowComicPost:
                case NotificationEntityType.FollowStoryPost:
                    return NotificationContent.FollowPost;
            }
        }

        #endregion

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
            switch (noti.EntityType)
            {
                case NotificationEntityType.ComicPostComment:
                case NotificationEntityType.ComicSubPostComment:
                    return noti.ActorName + NotificationContent.CommentOnComic;
                case NotificationEntityType.StoryPostComment:
                case NotificationEntityType.StorySubPostComment:
                    return noti.ActorName + NotificationContent.CommentOnStory;
                case NotificationEntityType.SocialSubPostComment:
                    return noti.ActorName + NotificationContent.CommentOnFeed;
                default:
                    return noti.ActorName + NotificationContent.CommentOnFeed;
            }
        }

        #endregion

        #region -- Delete --
        if (deleteEntities.Contains(noti.EntityType)
                && noti.Action == NotificationAction.DeletePost)
        {
            return noti.EntityType == NotificationEntityType.DeleteSocial ? nameof(Message.S302) : nameof(Message.S300);
        }

        if (deleteEntities.Contains(noti.EntityType)
                && noti.Action == NotificationAction.DeleteSubPost)
        {
            return nameof(Message.S301);
        }
        #endregion

        #region -- Lock --
        if (lockEntities.Contains(noti.EntityType)
                && noti.Action == NotificationAction.LockPost)
        {
            return noti.EntityType == NotificationEntityType.LockSocial ? nameof(Message.S305) : nameof(Message.S303);
        }

        if (lockEntities.Contains(noti.EntityType)
                && noti.Action == NotificationAction.LockSubPost)
        {
            return nameof(Message.S304);
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
            NotificationEntityType.DeleteSocial => Common.Core.Constants.Setting.NotificationTargetType.Feed,
            NotificationEntityType.LockSocial => Common.Core.Constants.Setting.NotificationTargetType.Feed,
            NotificationEntityType.SocialPostComment => Common.Core.Constants.Setting.NotificationTargetType.Feed,
            NotificationEntityType.SocialPostMention => Common.Core.Constants.Setting.NotificationTargetType.Feed,
            NotificationEntityType.SocialSubPostComment => Common.Core.Constants.Setting.NotificationTargetType.SubFeed,
            NotificationEntityType.SocialPostCommentReply => Common.Core.Constants.Setting.NotificationTargetType.Feed,
            NotificationEntityType.SocialSubPostCommentReply => Common.Core.Constants.Setting.NotificationTargetType.SubFeed,
            NotificationEntityType.SocialPostReaction => Common.Core.Constants.Setting.NotificationTargetType.Feed,
            NotificationEntityType.SocialSubPostReaction => Common.Core.Constants.Setting.NotificationTargetType.SubFeed,
            NotificationEntityType.SocialPostCommentMention => Common.Core.Constants.Setting.NotificationTargetType.Feed,
            NotificationEntityType.SocialSubPostCommentMention => Common.Core.Constants.Setting.NotificationTargetType.SubFeed,
            NotificationEntityType.SocialPostCommentReaction => Common.Core.Constants.Setting.NotificationTargetType.Feed,
            NotificationEntityType.SocialSubPostCommentReaction => Common.Core.Constants.Setting.NotificationTargetType.SubFeed,
            NotificationEntityType.SocialPostCommentReplyReaction => Common.Core.Constants.Setting.NotificationTargetType.Feed,
            NotificationEntityType.SocialSubPostCommentReplyReaction => Common.Core.Constants.Setting.NotificationTargetType.SubFeed,

            NotificationEntityType.DeleteComicPost => Common.Core.Constants.Setting.NotificationTargetType.Comic,
            NotificationEntityType.DeleteComicSubPost => Common.Core.Constants.Setting.NotificationTargetType.SubComic,
            NotificationEntityType.LockComicPost => Common.Core.Constants.Setting.NotificationTargetType.Comic,
            NotificationEntityType.LockComicSubPost => Common.Core.Constants.Setting.NotificationTargetType.SubComic,
            NotificationEntityType.ComicPostComment => Common.Core.Constants.Setting.NotificationTargetType.Comic,
            NotificationEntityType.ComicPostReaction => Common.Core.Constants.Setting.NotificationTargetType.Comic,
            NotificationEntityType.FollowComicPost => Common.Core.Constants.Setting.NotificationTargetType.Comic,
            NotificationEntityType.ComicSubPostComment => Common.Core.Constants.Setting.NotificationTargetType.SubComic,
            NotificationEntityType.ComicPostCommentReply => Common.Core.Constants.Setting.NotificationTargetType.Comic,
            NotificationEntityType.ComicSubPostCommentReply => Common.Core.Constants.Setting.NotificationTargetType.SubComic,
            NotificationEntityType.ComicPostCommentReaction => Common.Core.Constants.Setting.NotificationTargetType.Comic,
            NotificationEntityType.ComicSubPostCommentReaction => Common.Core.Constants.Setting.NotificationTargetType.SubComic,
            NotificationEntityType.ComicPostCommentMention => Common.Core.Constants.Setting.NotificationTargetType.Comic,
            NotificationEntityType.ComicSubPostCommentMention => Common.Core.Constants.Setting.NotificationTargetType.SubComic,
            NotificationEntityType.ComicPostCommentReplyReaction => Common.Core.Constants.Setting.NotificationTargetType.Comic,
            NotificationEntityType.ComicSubPostCommentReplyReaction => Common.Core.Constants.Setting.NotificationTargetType.SubComic,

            NotificationEntityType.DeleteStoryPost => Common.Core.Constants.Setting.NotificationTargetType.Story,
            NotificationEntityType.DeleteStorySubPost => Common.Core.Constants.Setting.NotificationTargetType.SubStory,
            NotificationEntityType.LockStoryPost => Common.Core.Constants.Setting.NotificationTargetType.Story,
            NotificationEntityType.LockStorySubPost => Common.Core.Constants.Setting.NotificationTargetType.SubStory,
            NotificationEntityType.StoryPostComment => Common.Core.Constants.Setting.NotificationTargetType.Story,
            NotificationEntityType.StoryPostReaction => Common.Core.Constants.Setting.NotificationTargetType.Story,
            NotificationEntityType.FollowStoryPost => Common.Core.Constants.Setting.NotificationTargetType.Story,
            NotificationEntityType.StoryPostCommentReaction => Common.Core.Constants.Setting.NotificationTargetType.Story,
            NotificationEntityType.StorySubPostCommentReaction => Common.Core.Constants.Setting.NotificationTargetType.SubStory,
            NotificationEntityType.StorySubPostComment => Common.Core.Constants.Setting.NotificationTargetType.SubStory,
            NotificationEntityType.StoryPostCommentReply => Common.Core.Constants.Setting.NotificationTargetType.Story,
            NotificationEntityType.StorySubPostCommentReply => Common.Core.Constants.Setting.NotificationTargetType.SubStory,
            NotificationEntityType.StoryPostCommentMention => Common.Core.Constants.Setting.NotificationTargetType.Story,
            NotificationEntityType.StorySubPostCommentMention => Common.Core.Constants.Setting.NotificationTargetType.SubStory,
            NotificationEntityType.StoryPostCommentReplyReaction => Common.Core.Constants.Setting.NotificationTargetType.Story,
            NotificationEntityType.StorySubPostCommentReplyReaction => Common.Core.Constants.Setting.NotificationTargetType.SubStory,

            NotificationEntityType.FollowUser => Common.Core.Constants.Setting.NotificationTargetType.FollowUser,

            NotificationEntityType.TransferTransaction => Common.Core.Constants.Setting.NotificationTargetType.Transaction,
            NotificationEntityType.DonateTransaction => Common.Core.Constants.Setting.NotificationTargetType.Transaction,

            NotificationEntityType.RejectPostReport => Common.Core.Constants.Setting.NotificationTargetType.RejectPostReport,
            NotificationEntityType.RejectCommentReport => Common.Core.Constants.Setting.NotificationTargetType.RejectCommentReport,

            _ => throw new NotSupportedException($"Unsupported entity type: {noti.EntityType}"),
        };
    }
    public static string ToNotiType(this NotificationQueryResult noti)
    {
        return noti.EntityType switch
        {
            NotificationEntityType.Video => Common.Core.Constants.Setting.NotificationType.Video + noti.Action.ToString(),
            NotificationEntityType.SocialPostComment => Common.Core.Constants.Setting.NotificationType.Comment,
            NotificationEntityType.ComicPostComment => Common.Core.Constants.Setting.NotificationType.Comment,
            NotificationEntityType.StoryPostComment => Common.Core.Constants.Setting.NotificationType.Comment,
            NotificationEntityType.SocialSubPostComment => Common.Core.Constants.Setting.NotificationType.Comment,
            NotificationEntityType.ComicSubPostComment => Common.Core.Constants.Setting.NotificationType.Comment,
            NotificationEntityType.StorySubPostComment => Common.Core.Constants.Setting.NotificationType.Comment,
            NotificationEntityType.SocialPostCommentReply => Common.Core.Constants.Setting.NotificationType.Reply,
            NotificationEntityType.ComicPostCommentReply => Common.Core.Constants.Setting.NotificationType.Reply,
            NotificationEntityType.StoryPostCommentReply => Common.Core.Constants.Setting.NotificationType.Reply,
            NotificationEntityType.SocialSubPostCommentReply => Common.Core.Constants.Setting.NotificationType.Reply,
            NotificationEntityType.ComicSubPostCommentReply => Common.Core.Constants.Setting.NotificationType.Reply,
            NotificationEntityType.StorySubPostCommentReply => Common.Core.Constants.Setting.NotificationType.Reply,
            NotificationEntityType.SocialPostReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.SocialSubPostReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.SocialPostCommentReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.SocialSubPostCommentReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.StoryPostReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.ComicPostReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.ComicPostCommentReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.ComicSubPostCommentReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.StoryPostCommentReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.StorySubPostCommentReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.SocialPostCommentReplyReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.SocialSubPostCommentReplyReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.ComicPostCommentReplyReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.ComicSubPostCommentReplyReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.StoryPostCommentReplyReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.StorySubPostCommentReplyReaction => Common.Core.Constants.Setting.NotificationType.Reaction,
            NotificationEntityType.SocialPostCommentMention => Common.Core.Constants.Setting.NotificationType.Mention,
            NotificationEntityType.SocialSubPostCommentMention => Common.Core.Constants.Setting.NotificationType.Mention,
            NotificationEntityType.SocialPostMention => Common.Core.Constants.Setting.NotificationType.Mention,
            NotificationEntityType.ComicPostCommentMention => Common.Core.Constants.Setting.NotificationType.Mention,
            NotificationEntityType.ComicSubPostCommentMention => Common.Core.Constants.Setting.NotificationType.Mention,
            NotificationEntityType.StoryPostCommentMention => Common.Core.Constants.Setting.NotificationType.Mention,
            NotificationEntityType.StorySubPostCommentMention => Common.Core.Constants.Setting.NotificationType.Mention,
            NotificationEntityType.FollowUser => Common.Core.Constants.Setting.NotificationType.FollowUser,
            NotificationEntityType.FollowComicPost => Common.Core.Constants.Setting.NotificationType.FollowPost,
            NotificationEntityType.FollowStoryPost => Common.Core.Constants.Setting.NotificationType.FollowPost,
            NotificationEntityType.TransferTransaction => Common.Core.Constants.Setting.NotificationType.TransferTransaction,
            NotificationEntityType.DonateTransaction => Common.Core.Constants.Setting.NotificationType.DonateTransaction,
            NotificationEntityType.DeleteSocial => Common.Core.Constants.Setting.NotificationType.DeleteSocial,
            NotificationEntityType.DeleteComicPost => Common.Core.Constants.Setting.NotificationType.DeletePost,
            NotificationEntityType.DeleteStoryPost => Common.Core.Constants.Setting.NotificationType.DeletePost,
            NotificationEntityType.DeleteComicSubPost => Common.Core.Constants.Setting.NotificationType.DeleteSubPost,
            NotificationEntityType.DeleteStorySubPost => Common.Core.Constants.Setting.NotificationType.DeleteSubPost,
            NotificationEntityType.LockSocial => Common.Core.Constants.Setting.NotificationType.LockSocial,
            NotificationEntityType.LockComicPost => Common.Core.Constants.Setting.NotificationType.LockPost,
            NotificationEntityType.LockStoryPost => Common.Core.Constants.Setting.NotificationType.LockPost,
            NotificationEntityType.LockComicSubPost => Common.Core.Constants.Setting.NotificationType.LockSubPost,
            NotificationEntityType.LockStorySubPost => Common.Core.Constants.Setting.NotificationType.LockSubPost,
            NotificationEntityType.RejectCommentReport => Common.Core.Constants.Setting.NotificationType.RejectReport,
            NotificationEntityType.RejectPostReport => Common.Core.Constants.Setting.NotificationType.RejectReport,

            _ => throw new NotSupportedException($"Unsupported entity type: {noti.EntityType}"),
        };
    }

    public static string ToDisplay(this NotificationStatus value)
    {
        var enumDisplayStatus = (NotificationStatus)value;
        return enumDisplayStatus.ToString();
    }
}
