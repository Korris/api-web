namespace Mcsg.Common.Core.Enums;

/// <summary>
/// NotificationEntity type
/// </summary>
public enum NotificationEntityType
{
    #region -- Comic --
    /// <summary>
    /// ComicPostComment
    /// </summary>
    ComicPostComment = 101,

    /// <summary>
    /// ComicSubPostComment
    /// </summary>
    ComicSubPostComment,

    /// <summary>
    /// ComicPostCommentReply
    /// </summary>
    ComicPostCommentReply,

    /// <summary>
    /// ComicSubPostCommentReply
    /// </summary>
    ComicSubPostCommentReply,

    /// <summary>
    /// ComicPostFollow
    /// </summary>
    ComicPostFollow,

    /// <summary>
    /// ComicPostReaction
    /// </summary>
    ComicPostReaction,

    /// <summary>
    /// ComicSubPostReaction
    /// </summary>
    ComicSubPostReaction,

    /// <summary>
    /// ComicPostCommentReaction
    /// </summary>
    ComicPostCommentReaction,

    /// <summary>
    /// ComicSubPostCommentReaction
    /// </summary>
    ComicSubPostCommentReaction,

    /// <summary>
    /// ComicPostCommentMention
    /// </summary>
    ComicPostCommentMention,

    /// <summary>
    /// ComicSubPostCommentMention
    /// </summary>
    ComicSubPostCommentMention,

    /// <summary>
    /// ComicPostCommentReplyReaction
    /// </summary>
    ComicPostCommentReplyReaction,

    /// <summary>
    /// ComicSubPostCommentReplyReaction
    /// </summary>
    ComicSubPostCommentReplyReaction,

    /// <summary>
    /// ComicPostDelete
    /// </summary>
    ComicPostDelete,

    /// <summary>
    /// ComicSubPostDelete
    /// </summary>
    ComicSubPostDelete,

    /// <summary>
    /// ComicPostCommentDelete
    /// </summary>
    ComicPostCommentDelete,

    /// <summary>
    /// ComicSubPostCommentDelete
    /// </summary>
    ComicSubPostCommentDelete,

    /// <summary>
    /// ComicPostLock
    /// </summary>
    ComicPostLock,

    /// <summary>
    /// ComicSubPostLock
    /// </summary>
    ComicSubPostLock,

    /// <summary>
    /// ComicSubPostAdd
    /// </summary>
    ComicSubPostAdd,
    #endregion

    #region -- Document --
    /// <summary>
    /// DocumentPostComment
    /// </summary>
    DocumentPostComment = 201,

    /// <summary>
    /// DocumentSubPostComment
    /// </summary>
    DocumentSubPostComment,

    /// <summary>
    /// DocumentPostCommentReply
    /// </summary>
    DocumentPostCommentReply,

    /// <summary>
    /// DocumentSubPostCommentReply
    /// </summary>
    DocumentSubPostCommentReply,

    /// <summary>
    /// DocumentPostFollow
    /// </summary>
    DocumentPostFollow,

    /// <summary>
    /// DocumentPostReaction
    /// </summary>
    DocumentPostReaction,

    /// <summary>
    /// DocumentSubPostReaction
    /// </summary>
    DocumentSubPostReaction,

    /// <summary>
    /// DocumentPostCommentReaction
    /// </summary>
    DocumentPostCommentReaction,

    /// <summary>
    /// DocumentSubPostCommentReaction
    /// </summary>
    DocumentSubPostCommentReaction,

    /// <summary>
    /// DocumentPostCommentMention
    /// </summary>
    DocumentPostCommentMention,

    /// <summary>
    /// DocumentSubPostCommentMention
    /// </summary>
    DocumentSubPostCommentMention,

    /// <summary>
    /// DocumentPostCommentReplyReaction
    /// </summary>
    DocumentPostCommentReplyReaction,

    /// <summary>
    /// DocumentSubPostCommentReplyReaction
    /// </summary>
    DocumentSubPostCommentReplyReaction,

    /// <summary>
    /// DocumentPostDelete
    /// </summary>
    DocumentPostDelete,

    /// <summary>
    /// DocumentSubPostDelete
    /// </summary>
    DocumentSubPostDelete,

    /// <summary>
    /// DocumentPostCommentDelete
    /// </summary>
    DocumentPostCommentDelete,

    /// <summary>
    /// DocumentSubPostCommentDelete
    /// </summary>
    DocumentSubPostCommentDelete,

    /// <summary>
    /// DocumentPostLock
    /// </summary>
    DocumentPostLock,

    /// <summary>
    /// DocumentSubPostLock
    /// </summary>
    DocumentSubPostLock,

    /// <summary>
    /// DocumentSubPostAdd
    /// </summary>
    DocumentSubPostAdd,
    #endregion

    #region -- Social --
    /// <summary>
    /// SocialPostComment
    /// </summary>
    SocialPostComment = 301,

    /// <summary>
    /// SocialSubPostComment
    /// </summary>
    SocialSubPostComment,

    /// <summary>
    /// SocialPostCommentReply
    /// </summary>
    SocialPostCommentReply,

    /// <summary>
    /// SocialSubPostCommentReply
    /// </summary>
    SocialSubPostCommentReply,

    /// <summary>
    /// SocialPostReaction
    /// </summary>
    SocialPostReaction,

    /// <summary>
    /// SocialSubPostReaction
    /// </summary>
    SocialSubPostReaction,

    /// <summary>
    /// SocialPostCommentReaction
    /// </summary>
    SocialPostCommentReaction,

    /// <summary>
    /// SocialSubPostCommentReaction
    /// </summary>
    SocialSubPostCommentReaction,

    /// <summary>
    /// SocialPostMention
    /// </summary>
    SocialPostMention,

    /// <summary>
    /// SocialSubPostMention
    /// </summary>
    SocialSubPostMention,

    /// <summary>
    /// SocialPostCommentMention
    /// </summary>
    SocialPostCommentMention,

    /// <summary>
    /// SocialSubPostCommentMention
    /// </summary>
    SocialSubPostCommentMention,

    /// <summary>
    /// SocialPostCommentReplyReaction
    /// </summary>
    SocialPostCommentReplyReaction,

    /// <summary>
    /// SocialSubPostCommentReplyReaction
    /// </summary>
    SocialSubPostCommentReplyReaction,

    /// <summary>
    /// SocialPostDelete
    /// </summary>
    SocialPostDelete,

    /// <summary>
    /// SocialPostCommentDelete
    /// </summary>
    SocialPostCommentDelete,

    /// <summary>
    /// SocialSubPostCommentDelete
    /// </summary>
    SocialSubPostCommentDelete,

    /// <summary>
    /// SocialPostLock
    /// </summary>
    SocialPostLock,
    #endregion

    #region -- Story --
    /// <summary>
    /// StoryPostComment
    /// </summary>
    StoryPostComment = 401,

    /// <summary>
    /// StorySubPostComment
    /// </summary>
    StorySubPostComment,

    /// <summary>
    /// StoryPostCommentReply
    /// </summary>
    StoryPostCommentReply,

    /// <summary>
    /// StorySubPostCommentReply
    /// </summary>
    StorySubPostCommentReply,

    /// <summary>
    /// StoryPostFollow
    /// </summary>
    StoryPostFollow,

    /// <summary>
    /// StoryPostReaction
    /// </summary>
    StoryPostReaction,

    /// <summary>
    /// StorySubPostReaction
    /// </summary>
    StorySubPostReaction,

    /// <summary>
    /// StoryPostCommentReaction
    /// </summary>
    StoryPostCommentReaction,

    /// <summary>
    /// StorySubPostCommentReaction
    /// </summary>
    StorySubPostCommentReaction,

    /// <summary>
    /// StoryPostCommentMention
    /// </summary>
    StoryPostCommentMention,

    /// <summary>
    /// StorySubPostCommentMention
    /// </summary>
    StorySubPostCommentMention,

    /// <summary>
    /// StoryPostCommentReplyReaction
    /// </summary>
    StoryPostCommentReplyReaction,

    /// <summary>
    /// StorySubPostCommentReplyReaction
    /// </summary>
    StorySubPostCommentReplyReaction,

    /// <summary>
    /// StoryPostDelete
    /// </summary>
    StoryPostDelete,

    /// <summary>
    /// StorySubPostDelete
    /// </summary>
    StorySubPostDelete,

    /// <summary>
    /// StoryPostCommentDelete
    /// </summary>
    StoryPostCommentDelete,

    /// <summary>
    /// StorySubPostCommentDelete
    /// </summary>
    StorySubPostCommentDelete,

    /// <summary>
    /// StoryPostLock
    /// </summary>
    StoryPostLock,

    /// <summary>
    /// StorySubPostLock
    /// </summary>
    StorySubPostLock,

    /// <summary>
    /// StorySubPostAdd
    /// </summary>
    StorySubPostAdd,
    #endregion

    #region -- Other --
    /// <summary>
    /// Video
    /// </summary>
    Video = 501,

    /// <summary>
    /// FollowUser
    /// </summary>
    FollowUser = 502,
    #endregion

    #region -- Transaction --
    /// <summary>
    /// TransferTransaction
    /// </summary>
    TransferTransaction = 511,

    /// <summary>
    /// DonateTransaction
    /// </summary>
    DonateTransaction = 512,

    /// <summary>
    /// DepositTransaction
    /// </summary>
    DepositTransaction = 513,

    /// <summary>
    /// BuyPremiumTransaction
    /// </summary>
    BuyPremiumTransaction = 514,

    /// <summary>
    /// BuyUpgradePremiumTransaction
    /// </summary>
    BuyUpgradePremiumTransaction = 515,
    #endregion

    #region -- Report --
    /// <summary>
    /// RejectCommentReport
    /// </summary>
    RejectCommentReport = 521,

    /// <summary>
    /// RejectPostReport
    /// </summary>
    RejectPostReport = 522,
    #endregion

    #region -- Subscription --
    /// <summary>
    /// RemindExpiredSubscription
    /// </summary>
    RemindExpiredSubscription = 531,

    /// <summary>
    /// ExpiredSubscription
    /// </summary>
    ExpiredSubscription = 532,
    #endregion
}
