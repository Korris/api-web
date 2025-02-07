using System.Text.Json.Serialization;

namespace Mcsg.Common.Domain.Entities;

using Constants;
using Core.Enums;
using SeedWork.Converters;
using SeedWork.Dtos;
using Message = Core.Constants.Message;
using Setting = Core.Constants.Setting;

partial class Notification
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public Notification() { }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <returns>Return the DTO</returns>
    public SearchDto ToSearchDto()
    {
        return new SearchDto();
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <returns>Return the DTO</returns>
    public ViewDto ToViewDto()
    {
        return ToBaseDto<ViewDto>();
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <returns>Return the DTO</returns>
    public T ToBaseDto<T>() where T : BaseDto, new()
    {
        return new T
        {
            Id = Id,
        };
    }

    public static string ToMessage(SearchDto noti)
    {
        if (noti == null)
        {
            return string.Empty;
        }

        List<NotificationEntityType> commentEntities =
        [
            NotificationEntityType.ComicPostComment,
            NotificationEntityType.ComicSubPostComment,
            NotificationEntityType.ComicPostCommentReply,
            NotificationEntityType.ComicSubPostCommentReply,

            NotificationEntityType.DocumentPostComment,
            NotificationEntityType.DocumentSubPostComment,
            NotificationEntityType.DocumentPostCommentReply,
            NotificationEntityType.DocumentSubPostCommentReply,

            NotificationEntityType.SocialPostComment,
            NotificationEntityType.SocialSubPostComment,
            NotificationEntityType.SocialPostCommentReply,
            NotificationEntityType.SocialSubPostCommentReply,

            NotificationEntityType.StoryPostComment,
            NotificationEntityType.StorySubPostComment,
            NotificationEntityType.StoryPostCommentReply,
            NotificationEntityType.StorySubPostCommentReply
        ];

        List<NotificationEntityType> rejectReportEntities =
        [
            NotificationEntityType.RejectPostReport,
            NotificationEntityType.RejectCommentReport
        ];

        List<NotificationEntityType> reactionEntities =
        [
            NotificationEntityType.ComicPostReaction,
            NotificationEntityType.ComicSubPostReaction,
            NotificationEntityType.ComicPostCommentReaction,
            NotificationEntityType.ComicSubPostCommentReaction,
            NotificationEntityType.ComicPostCommentReplyReaction,
            NotificationEntityType.ComicSubPostCommentReplyReaction,

            NotificationEntityType.DocumentPostReaction,
            NotificationEntityType.DocumentSubPostReaction,
            NotificationEntityType.DocumentPostCommentReaction,
            NotificationEntityType.DocumentSubPostCommentReaction,
            NotificationEntityType.DocumentPostCommentReplyReaction,
            NotificationEntityType.DocumentSubPostCommentReplyReaction,

            NotificationEntityType.SocialPostReaction,
            NotificationEntityType.SocialSubPostReaction,
            NotificationEntityType.SocialPostCommentReaction,
            NotificationEntityType.SocialSubPostCommentReaction,
            NotificationEntityType.SocialPostCommentReplyReaction,
            NotificationEntityType.SocialSubPostCommentReplyReaction,

            NotificationEntityType.StoryPostReaction,
            NotificationEntityType.StorySubPostReaction,
            NotificationEntityType.StoryPostCommentReaction,
            NotificationEntityType.StorySubPostCommentReaction,
            NotificationEntityType.StoryPostCommentReplyReaction,
            NotificationEntityType.StorySubPostCommentReplyReaction
        ];

        List<NotificationEntityType> followEntities =
        [
            NotificationEntityType.FollowUser,
            NotificationEntityType.ComicPostFollow,
            NotificationEntityType.DocumentPostFollow,
            NotificationEntityType.StoryPostFollow,
        ];

        List<NotificationEntityType> deleteEntities =
        [
            NotificationEntityType.ComicPostDelete,
            NotificationEntityType.ComicSubPostDelete,
            NotificationEntityType.ComicPostCommentDelete,
            NotificationEntityType.ComicSubPostCommentDelete,

            NotificationEntityType.DocumentPostDelete,
            NotificationEntityType.DocumentSubPostDelete,
            NotificationEntityType.DocumentPostCommentDelete,
            NotificationEntityType.DocumentSubPostCommentDelete,

            NotificationEntityType.SocialPostDelete,
            NotificationEntityType.SocialPostCommentDelete,
            NotificationEntityType.SocialSubPostCommentDelete,

            NotificationEntityType.StoryPostDelete,
            NotificationEntityType.StorySubPostDelete,
            NotificationEntityType.StoryPostCommentDelete,
            NotificationEntityType.StorySubPostCommentDelete,
        ];

        List<NotificationEntityType> lockEntities =
        [
            NotificationEntityType.ComicPostLock,
            NotificationEntityType.ComicSubPostLock,

            NotificationEntityType.DocumentPostLock,
            NotificationEntityType.DocumentSubPostLock,

            NotificationEntityType.SocialPostLock,

            NotificationEntityType.StoryPostLock,
            NotificationEntityType.StorySubPostLock
        ];

        List<NotificationEntityType> subscriptionEntities =
        [
           NotificationEntityType.RemindExpiredSubscription,
           NotificationEntityType.ExpiredSubscription,
        ];

        var entityType = noti.EntityType.ToString();
        if (entityType.Contains("Mention"))
        {
            if (entityType.Contains("Comment"))
            {
                return nameof(NotificationContent.MentionOnComment);
            }

            return nameof(NotificationContent.MentionOnPost);
        }

        #region -- Reaction --
        if (reactionEntities.Contains(noti.EntityType))
        {
            if (noti.EntityType.ToString().Contains("Comment"))
            {
                return nameof(NotificationContent.ReactOnComment);
            }
            else
            {
                var notiContent = noti.EntityType switch
                {
                    NotificationEntityType.ComicPostReaction => nameof(NotificationContent.ReactOnComic),
                    NotificationEntityType.DocumentPostReaction => nameof(NotificationContent.ReactOnDocument),
                    NotificationEntityType.StoryPostReaction => nameof(NotificationContent.ReactOnStory),
                    _ => nameof(NotificationContent.ReactOnFeed),
                };

                return notiContent;
            }
        }
        #endregion

        #region -- Report --
        if (rejectReportEntities.Contains(noti.EntityType))
        {
            return noti.EntityType == NotificationEntityType.RejectCommentReport ? nameof(Message.S306) : nameof(Message.S307);
        }
        #endregion

        #region -- Follow --
        if (followEntities.Contains(noti.EntityType))
        {
            switch (noti.EntityType)
            {
                case NotificationEntityType.FollowUser:
                    return nameof(NotificationContent.FollowUser);

                case NotificationEntityType.ComicPostFollow:
                    return nameof(NotificationContent.FollowComic);

                case NotificationEntityType.DocumentPostFollow:
                    return nameof(NotificationContent.FollowDocument);

                case NotificationEntityType.StoryPostFollow:
                    return nameof(NotificationContent.FollowStory);
            }
        }
        #endregion

        #region -- Video --
        if (noti.EntityType == NotificationEntityType.Video && noti.Action == NotificationAction.Processing)
        {
            return nameof(NotificationContent.VideoUploadProcessing);
        }

        if (noti.EntityType == NotificationEntityType.Video && noti.Action == NotificationAction.Completed)
        {
            return nameof(NotificationContent.VideoUploadCompleted);
        }

        if (noti.EntityType == NotificationEntityType.Video && noti.Action == NotificationAction.Failed)
        {
            return nameof(NotificationContent.VideoUploadFailed);
        }
        #endregion

        #region -- Comment --
        if (commentEntities.Contains(noti.EntityType) && noti.Action == NotificationAction.Mention)
        {
            return nameof(NotificationContent.MentionOnComment);
        }

        if (commentEntities.Contains(noti.EntityType) && noti.Action == NotificationAction.Reaction)
        {
            return nameof(NotificationContent.ReactOnComment);
        }

        if (commentEntities.Contains(noti.EntityType) && noti.Action == NotificationAction.Reply)
        {
            return nameof(NotificationContent.ReplyOnComment);
        }

        if (commentEntities.Contains(noti.EntityType) && noti.Action == NotificationAction.Comment)
        {
            switch (noti.EntityType)
            {
                case NotificationEntityType.ComicPostComment:
                case NotificationEntityType.ComicSubPostComment:
                    return nameof(NotificationContent.CommentOnComic);

                case NotificationEntityType.DocumentPostComment:
                case NotificationEntityType.DocumentSubPostComment:
                    return nameof(NotificationContent.CommentOnDocument);

                case NotificationEntityType.StoryPostComment:
                case NotificationEntityType.StorySubPostComment:
                    return nameof(NotificationContent.CommentOnStory);

                case NotificationEntityType.SocialSubPostComment:
                    return nameof(NotificationContent.CommentOnFeed);

                default:
                    return nameof(NotificationContent.CommentOnFeed);
            }
        }
        #endregion

        #region -- Delete --
        if (deleteEntities.Contains(noti.EntityType) && noti.Action == NotificationAction.DeletePost)
        {
            return noti.EntityType == NotificationEntityType.SocialPostDelete ? nameof(Message.S302) : nameof(Message.S300);
        }

        if (deleteEntities.Contains(noti.EntityType) && noti.Action == NotificationAction.DeleteSubPost)
        {
            return nameof(Message.S301);
        }

        if (deleteEntities.Contains(noti.EntityType) && noti.Action == NotificationAction.DeleteComment)
        {
            return nameof(Message.S308);
        }
        #endregion

        #region -- Lock --
        if (lockEntities.Contains(noti.EntityType) && noti.Action == NotificationAction.LockPost)
        {
            return noti.EntityType == NotificationEntityType.SocialPostLock ? nameof(Message.S305) : nameof(Message.S303);
        }

        if (lockEntities.Contains(noti.EntityType) && noti.Action == NotificationAction.LockSubPost)
        {
            return nameof(Message.S304);
        }
        #endregion

        #region -- Mention --
        if (noti.EntityType == NotificationEntityType.SocialPostCommentMention && noti.Action == NotificationAction.Mention)
        {
            return nameof(NotificationContent.MentionOnComment);
        }

        if (noti.EntityType == NotificationEntityType.SocialSubPostCommentMention && noti.Action == NotificationAction.Mention)
        {
            return nameof(NotificationContent.MentionOnReply);
        }
        #endregion

        #region -- Transaction --
        if (noti.EntityType == NotificationEntityType.DonateTransaction)
        {
            return nameof(NotificationContent.DonateTransaction);
        }
        if (noti.EntityType == NotificationEntityType.TransferTransaction)
        {
            return nameof(NotificationContent.TransferTransaction);
        }

        if (noti.EntityType == NotificationEntityType.DepositTransaction)
        {
            return nameof(NotificationContent.DepositTransaction);
        }
        #endregion

        #region -- Subscription --
        if (noti.EntityType == NotificationEntityType.RemindExpiredSubscription)
        {
            return nameof(NotificationContent.RemindExpiredSubscription);
        }

        if (noti.EntityType == NotificationEntityType.ExpiredSubscription)
        {
            return nameof(NotificationContent.ExpiredSubscription);
        }
        #endregion

        return "";
    }

    public static string ToTargetType(SearchDto noti)
    {
        return noti.EntityType switch
        {
            NotificationEntityType.Video => Setting.NotificationTargetType.Social,

            NotificationEntityType.ComicPostComment => Setting.NotificationTargetType.Comic,
            NotificationEntityType.ComicSubPostComment => Setting.NotificationTargetType.SubComic,
            NotificationEntityType.ComicPostCommentReply => Setting.NotificationTargetType.Comic,
            NotificationEntityType.ComicSubPostCommentReply => Setting.NotificationTargetType.SubComic,
            NotificationEntityType.ComicPostFollow => Setting.NotificationTargetType.Comic,
            NotificationEntityType.ComicPostReaction => Setting.NotificationTargetType.Comic,
            NotificationEntityType.ComicSubPostReaction => Setting.NotificationTargetType.SubComic,
            NotificationEntityType.ComicPostCommentReaction => Setting.NotificationTargetType.Comic,
            NotificationEntityType.ComicSubPostCommentReaction => Setting.NotificationTargetType.SubComic,
            NotificationEntityType.ComicPostCommentMention => Setting.NotificationTargetType.Comic,
            NotificationEntityType.ComicSubPostCommentMention => Setting.NotificationTargetType.SubComic,
            NotificationEntityType.ComicPostCommentReplyReaction => Setting.NotificationTargetType.Comic,
            NotificationEntityType.ComicSubPostCommentReplyReaction => Setting.NotificationTargetType.SubComic,
            NotificationEntityType.ComicPostDelete => Setting.NotificationTargetType.Comic,
            NotificationEntityType.ComicSubPostDelete => Setting.NotificationTargetType.SubComic,
            NotificationEntityType.ComicPostLock => Setting.NotificationTargetType.Comic,
            NotificationEntityType.ComicSubPostLock => Setting.NotificationTargetType.SubComic,

            NotificationEntityType.DocumentPostComment => Setting.NotificationTargetType.Document,
            NotificationEntityType.DocumentSubPostComment => Setting.NotificationTargetType.SubDocument,
            NotificationEntityType.DocumentPostCommentReply => Setting.NotificationTargetType.Document,
            NotificationEntityType.DocumentSubPostCommentReply => Setting.NotificationTargetType.SubDocument,
            NotificationEntityType.DocumentPostFollow => Setting.NotificationTargetType.Document,
            NotificationEntityType.DocumentPostReaction => Setting.NotificationTargetType.Document,
            NotificationEntityType.DocumentSubPostReaction => Setting.NotificationTargetType.SubDocument,
            NotificationEntityType.DocumentPostCommentReaction => Setting.NotificationTargetType.Document,
            NotificationEntityType.DocumentSubPostCommentReaction => Setting.NotificationTargetType.SubDocument,
            NotificationEntityType.DocumentPostCommentMention => Setting.NotificationTargetType.Document,
            NotificationEntityType.DocumentSubPostCommentMention => Setting.NotificationTargetType.SubDocument,
            NotificationEntityType.DocumentPostCommentReplyReaction => Setting.NotificationTargetType.Document,
            NotificationEntityType.DocumentSubPostCommentReplyReaction => Setting.NotificationTargetType.SubDocument,
            NotificationEntityType.DocumentPostDelete => Setting.NotificationTargetType.Document,
            NotificationEntityType.DocumentSubPostDelete => Setting.NotificationTargetType.SubDocument,
            NotificationEntityType.DocumentPostLock => Setting.NotificationTargetType.Document,
            NotificationEntityType.DocumentSubPostLock => Setting.NotificationTargetType.SubDocument,

            NotificationEntityType.SocialPostComment => Setting.NotificationTargetType.Social,
            NotificationEntityType.SocialSubPostComment => Setting.NotificationTargetType.SubSocial,
            NotificationEntityType.SocialPostCommentReply => Setting.NotificationTargetType.Social,
            NotificationEntityType.SocialSubPostCommentReply => Setting.NotificationTargetType.SubSocial,
            NotificationEntityType.SocialPostReaction => Setting.NotificationTargetType.Social,
            NotificationEntityType.SocialSubPostReaction => Setting.NotificationTargetType.SubSocial,
            NotificationEntityType.SocialPostMention => Setting.NotificationTargetType.Social,
            NotificationEntityType.SocialPostCommentReaction => Setting.NotificationTargetType.Social,
            NotificationEntityType.SocialSubPostCommentReaction => Setting.NotificationTargetType.SubSocial,
            NotificationEntityType.SocialPostCommentMention => Setting.NotificationTargetType.Social,
            NotificationEntityType.SocialSubPostCommentMention => Setting.NotificationTargetType.SubSocial,
            NotificationEntityType.SocialPostCommentReplyReaction => Setting.NotificationTargetType.Social,
            NotificationEntityType.SocialSubPostCommentReplyReaction => Setting.NotificationTargetType.SubSocial,
            NotificationEntityType.SocialPostDelete => Setting.NotificationTargetType.Social,
            NotificationEntityType.SocialPostLock => Setting.NotificationTargetType.Social,

            NotificationEntityType.StoryPostComment => Setting.NotificationTargetType.Story,
            NotificationEntityType.StorySubPostComment => Setting.NotificationTargetType.SubStory,
            NotificationEntityType.StoryPostCommentReply => Setting.NotificationTargetType.Story,
            NotificationEntityType.StorySubPostCommentReply => Setting.NotificationTargetType.SubStory,
            NotificationEntityType.StoryPostFollow => Setting.NotificationTargetType.Story,
            NotificationEntityType.StoryPostReaction => Setting.NotificationTargetType.Story,
            NotificationEntityType.StorySubPostReaction => Setting.NotificationTargetType.SubStory,
            NotificationEntityType.StoryPostCommentReaction => Setting.NotificationTargetType.Story,
            NotificationEntityType.StorySubPostCommentReaction => Setting.NotificationTargetType.SubStory,
            NotificationEntityType.StoryPostCommentMention => Setting.NotificationTargetType.Story,
            NotificationEntityType.StorySubPostCommentMention => Setting.NotificationTargetType.SubStory,
            NotificationEntityType.StoryPostCommentReplyReaction => Setting.NotificationTargetType.Story,
            NotificationEntityType.StorySubPostCommentReplyReaction => Setting.NotificationTargetType.SubStory,
            NotificationEntityType.StoryPostDelete => Setting.NotificationTargetType.Story,
            NotificationEntityType.StorySubPostDelete => Setting.NotificationTargetType.SubStory,
            NotificationEntityType.StoryPostLock => Setting.NotificationTargetType.Story,
            NotificationEntityType.StorySubPostLock => Setting.NotificationTargetType.SubStory,

            NotificationEntityType.FollowUser => Setting.NotificationTargetType.FollowUser,

            NotificationEntityType.TransferTransaction => Setting.NotificationTargetType.Transaction,
            NotificationEntityType.DonateTransaction => Setting.NotificationTargetType.Transaction,
            NotificationEntityType.DepositTransaction => Setting.NotificationTargetType.Transaction,

            NotificationEntityType.RejectPostReport => Setting.NotificationTargetType.RejectPostReport,
            NotificationEntityType.RejectCommentReport => Setting.NotificationTargetType.RejectCommentReport,

            NotificationEntityType.RemindExpiredSubscription => Setting.NotificationTargetType.Subscription,
            NotificationEntityType.ExpiredSubscription => Setting.NotificationTargetType.Subscription,

            NotificationEntityType.ComicPostCommentDelete
         or NotificationEntityType.ComicSubPostCommentDelete
         or NotificationEntityType.StoryPostCommentDelete
         or NotificationEntityType.StorySubPostCommentDelete
         or NotificationEntityType.SocialPostCommentDelete
         or NotificationEntityType.SocialSubPostCommentDelete
         or NotificationEntityType.DocumentPostCommentDelete
         or NotificationEntityType.DocumentSubPostCommentDelete
         => Setting.NotificationType.DeleteComment,

            _ => throw new NotSupportedException($"Unsupported entity type: {noti.EntityType}")
        };
    }

    public static string ToNotiType(SearchDto noti)
    {
        return noti.EntityType switch
        {
            NotificationEntityType.Video => Setting.NotificationType.Video + noti.Action.ToString(),

            NotificationEntityType.ComicPostComment => Setting.NotificationType.Comment,
            NotificationEntityType.ComicSubPostComment => Setting.NotificationType.Comment,
            NotificationEntityType.ComicPostCommentReply => Setting.NotificationType.Reply,
            NotificationEntityType.ComicSubPostCommentReply => Setting.NotificationType.Reply,
            NotificationEntityType.ComicPostFollow => Setting.NotificationType.FollowPost,
            NotificationEntityType.ComicPostReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.ComicSubPostReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.ComicPostCommentReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.ComicSubPostCommentReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.ComicPostCommentMention => Setting.NotificationType.Mention,
            NotificationEntityType.ComicSubPostCommentMention => Setting.NotificationType.Mention,
            NotificationEntityType.ComicPostCommentReplyReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.ComicSubPostCommentReplyReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.ComicPostDelete => Setting.NotificationType.DeletePost,
            NotificationEntityType.ComicSubPostDelete => Setting.NotificationType.DeleteSubPost,
            NotificationEntityType.ComicPostLock => Setting.NotificationType.LockPost,
            NotificationEntityType.ComicSubPostLock => Setting.NotificationType.LockSubPost,

            NotificationEntityType.DocumentPostComment => Setting.NotificationType.Comment,
            NotificationEntityType.DocumentSubPostComment => Setting.NotificationType.Comment,
            NotificationEntityType.DocumentPostCommentReply => Setting.NotificationType.Reply,
            NotificationEntityType.DocumentSubPostCommentReply => Setting.NotificationType.Reply,
            NotificationEntityType.DocumentPostFollow => Setting.NotificationType.FollowPost,
            NotificationEntityType.DocumentPostReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.DocumentSubPostReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.DocumentPostCommentReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.DocumentSubPostCommentReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.DocumentPostCommentMention => Setting.NotificationType.Mention,
            NotificationEntityType.DocumentSubPostCommentMention => Setting.NotificationType.Mention,
            NotificationEntityType.DocumentPostCommentReplyReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.DocumentSubPostCommentReplyReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.DocumentPostDelete => Setting.NotificationType.DeletePost,
            NotificationEntityType.DocumentSubPostDelete => Setting.NotificationType.DeleteSubPost,
            NotificationEntityType.DocumentPostLock => Setting.NotificationType.LockPost,
            NotificationEntityType.DocumentSubPostLock => Setting.NotificationType.LockSubPost,

            NotificationEntityType.SocialPostComment => Setting.NotificationType.Comment,
            NotificationEntityType.SocialSubPostComment => Setting.NotificationType.Comment,
            NotificationEntityType.SocialPostCommentReply => Setting.NotificationType.Reply,
            NotificationEntityType.SocialSubPostCommentReply => Setting.NotificationType.Reply,
            NotificationEntityType.SocialPostReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.SocialSubPostReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.SocialPostMention => Setting.NotificationType.Mention,
            NotificationEntityType.SocialPostCommentReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.SocialSubPostCommentReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.SocialPostCommentMention => Setting.NotificationType.Mention,
            NotificationEntityType.SocialSubPostCommentMention => Setting.NotificationType.Mention,
            NotificationEntityType.SocialPostCommentReplyReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.SocialSubPostCommentReplyReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.SocialPostDelete => Setting.NotificationType.DeleteSocial,
            NotificationEntityType.SocialPostLock => Setting.NotificationType.LockSocial,

            NotificationEntityType.StoryPostComment => Setting.NotificationType.Comment,
            NotificationEntityType.StorySubPostComment => Setting.NotificationType.Comment,
            NotificationEntityType.StoryPostCommentReply => Setting.NotificationType.Reply,
            NotificationEntityType.StorySubPostCommentReply => Setting.NotificationType.Reply,
            NotificationEntityType.StoryPostFollow => Setting.NotificationType.FollowPost,
            NotificationEntityType.StoryPostReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.StorySubPostReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.StoryPostCommentReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.StorySubPostCommentReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.StoryPostCommentMention => Setting.NotificationType.Mention,
            NotificationEntityType.StorySubPostCommentMention => Setting.NotificationType.Mention,
            NotificationEntityType.StoryPostCommentReplyReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.StorySubPostCommentReplyReaction => Setting.NotificationType.Reaction,
            NotificationEntityType.StoryPostDelete => Setting.NotificationType.DeletePost,
            NotificationEntityType.StorySubPostDelete => Setting.NotificationType.DeleteSubPost,
            NotificationEntityType.StoryPostLock => Setting.NotificationType.LockPost,
            NotificationEntityType.StorySubPostLock => Setting.NotificationType.LockSubPost,

            NotificationEntityType.FollowUser => Setting.NotificationType.FollowUser,

            NotificationEntityType.TransferTransaction => Setting.NotificationType.TransferTransaction,
            NotificationEntityType.DonateTransaction => Setting.NotificationType.DonateTransaction,
            NotificationEntityType.DepositTransaction => Setting.NotificationType.DepositTransaction,

            NotificationEntityType.RejectPostReport => Setting.NotificationType.RejectReport,
            NotificationEntityType.RejectCommentReport => Setting.NotificationType.RejectReport,

            NotificationEntityType.RemindExpiredSubscription => Setting.NotificationType.RemindExpiredSubscription,
            NotificationEntityType.ExpiredSubscription => Setting.NotificationType.ExpiredSubscription,

            NotificationEntityType.ComicPostCommentDelete
         or NotificationEntityType.ComicSubPostCommentDelete
         or NotificationEntityType.StoryPostCommentDelete
         or NotificationEntityType.StorySubPostCommentDelete
         or NotificationEntityType.SocialPostCommentDelete
         or NotificationEntityType.SocialSubPostCommentDelete
         or NotificationEntityType.DocumentPostCommentDelete
         or NotificationEntityType.DocumentSubPostCommentDelete
         => Setting.NotificationType.DeleteComment,

            _ => throw new NotSupportedException($"Unsupported entity type: {noti.EntityType}")
        };
    }

    #endregion

    #region -- Classes --

    /// <summary>
    /// Base
    /// </summary>
    public class BaseDto : IdDto
    {
    }

    /// <summary>
    /// Search
    /// </summary>
    public class SearchDto : BaseDto
    {
        #region -- Properties --

        public Guid ReceiverId { get; set; }
        public NotificationStatus Status { get; set; }
        public Guid? LocationId { get; set; }
        public string LocationHashId { get; set; } = default!;
        public NotificationEntityType EntityType { get; set; }
        public string? EntityTypeName => EntityType.ToString();
        public Guid? EntityId { get; set; }
        public string EntityHashId { get; set; } = default!;
        public NotificationAction Action { get; set; }

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime CreatedOn { get; set; }

        public Guid ActorId { get; set; }
        public string ActorName { get; set; } = default!;
        public string UserAvatar { get; set; } = default!;
        public int? ReactionType { get; set; }
        public string UserName { get; set; } = default!;

        public string? ReferenceNumber { get; set; }
        public string? Amount { get; set; }
        public string FollowPostMessage { get; set; } = default!;
        public string Message => ToMessage(this);
        public float Order { get; set; }
        public Guid? ReplyCommentId { get; set; }
        public string StatusName => Status.ToString();
        public string TargetType => ToTargetType(this);
        public string NotificationType => ToNotiType(this);
        public Guid? CommentId => EntityType.ToString().Contains("Comment") ? EntityId : null;
        public string? CurrencyUnit { get; set; }
        public string? PostName { get; set; }
        public DateTime ExpiredDate { get; set; }

        #endregion
    }

    /// <summary>
    /// View
    /// </summary>
    public class ViewDto : BaseDto
    {
    }

    #endregion
}
