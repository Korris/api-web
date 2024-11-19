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
    ComicPostComment = 5,

    /// <summary>
    /// ComicSubPostComment
    /// </summary>
    ComicSubPostComment = 8,

    /// <summary>
    /// ComicPostCommentReply
    /// </summary>
    ComicPostCommentReply = 11,

    /// <summary>
    /// ComicSubPostCommentReply
    /// </summary>
    ComicSubPostCommentReply = 14,

    /// <summary>
    /// ComicPostFollow
    /// </summary>
    ComicPostFollow = 24,

    /// <summary>
    /// ComicPostReaction
    /// </summary>
    ComicPostReaction = 26,

    /// <summary>
    /// ComicSubPostReaction
    /// </summary>
    ComicSubPostReaction = 27,

    /// <summary>
    /// ComicPostCommentReaction
    /// </summary>
    ComicPostCommentReaction = 28,

    /// <summary>
    /// ComicSubPostCommentReaction
    /// </summary>
    ComicSubPostCommentReaction = 29,

    /// <summary>
    /// ComicPostCommentMention
    /// </summary>
    ComicPostCommentMention = 36,

    /// <summary>
    /// ComicSubPostCommentMention
    /// </summary>
    ComicSubPostCommentMention = 38,

    /// <summary>
    /// ComicPostCommentReplyReaction
    /// </summary>
    ComicPostCommentReplyReaction = 42,

    /// <summary>
    /// ComicSubPostCommentReplyReaction
    /// </summary>
    ComicSubPostCommentReplyReaction = 43,

    /// <summary>
    /// ComicPostDelete
    /// </summary>
    ComicPostDelete = 54,

    /// <summary>
    /// ComicSubPostDelete
    /// </summary>
    ComicSubPostDelete = 55,

    /// <summary>
    /// ComicPostCommentDelete
    /// </summary>
    ComicPostCommentDelete = 116,

    /// <summary>
    /// ComicSubPostCommentDelete
    /// </summary>
    ComicSubPostCommentDelete = 117,

    /// <summary>
    /// ComicPostLock
    /// </summary>
    ComicPostLock = 59,

    /// <summary>
    /// ComicSubPostLock
    /// </summary>
    ComicSubPostLock = 60,
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
    DocumentPostCommentDelete = 216,

    /// <summary>
    /// DocumentSubPostCommentDelete
    /// </summary>
    DocumentSubPostCommentDelete = 217,

    /// <summary>
    /// DocumentPostLock
    /// </summary>
    DocumentPostLock,

    /// <summary>
    /// DocumentSubPostLock
    /// </summary>
    DocumentSubPostLock,
    #endregion

    #region -- Social --
    /// <summary>
    /// SocialPostComment
    /// </summary>
    SocialPostComment = 4,

    /// <summary>
    /// SocialSubPostComment
    /// </summary>
    SocialSubPostComment = 7,

    /// <summary>
    /// SocialPostCommentReply
    /// </summary>
    SocialPostCommentReply = 10,

    /// <summary>
    /// SocialSubPostCommentReply
    /// </summary>
    SocialSubPostCommentReply = 13,

    /// <summary>
    /// SocialPostReaction
    /// </summary>
    SocialPostReaction = 18,

    /// <summary>
    /// SocialSubPostReaction
    /// </summary>
    SocialSubPostReaction = 19,

    /// <summary>
    /// SocialPostCommentReaction
    /// </summary>
    SocialPostCommentReaction = 16,

    /// <summary>
    /// SocialSubPostCommentReaction
    /// </summary>
    SocialSubPostCommentReaction = 17,

    /// <summary>
    /// SocialPostMention
    /// </summary>
    SocialPostMention = 34,

    /// <summary>
    /// SocialSubPostMention
    /// </summary>
    SocialSubPostMention = 35,

    /// <summary>
    /// SocialPostCommentMention
    /// </summary>
    SocialPostCommentMention = 21,

    /// <summary>
    /// SocialSubPostCommentMention
    /// </summary>
    SocialSubPostCommentMention = 22,

    /// <summary>
    /// SocialPostCommentReplyReaction
    /// </summary>
    SocialPostCommentReplyReaction = 40,

    /// <summary>
    /// SocialSubPostCommentReplyReaction
    /// </summary>
    SocialSubPostCommentReplyReaction = 41,

    /// <summary>
    /// SocialPostDelete
    /// </summary>
    SocialPostDelete = 56,

    /// <summary>
    /// SocialPostCommentDelete
    /// </summary>
    SocialPostCommentDelete = 316,

    /// <summary>
    /// SocialSubPostCommentDelete
    /// </summary>
    SocialSubPostCommentDelete = 317,

    /// <summary>
    /// SocialPostLock
    /// </summary>
    SocialPostLock = 61,
    #endregion

    #region -- Story --
    /// <summary>
    /// StoryPostComment
    /// </summary>
    StoryPostComment = 6,

    /// <summary>
    /// StorySubPostComment
    /// </summary>
    StorySubPostComment = 9,

    /// <summary>
    /// StoryPostCommentReply
    /// </summary>
    StoryPostCommentReply = 12,

    /// <summary>
    /// StorySubPostCommentReply
    /// </summary>
    StorySubPostCommentReply = 15,

    /// <summary>
    /// StoryPostFollow
    /// </summary>
    StoryPostFollow = 25,

    /// <summary>
    /// StoryPostReaction
    /// </summary>
    StoryPostReaction = 30,

    /// <summary>
    /// StorySubPostReaction
    /// </summary>
    StorySubPostReaction = 31,

    /// <summary>
    /// StoryPostCommentReaction
    /// </summary>
    StoryPostCommentReaction = 32,

    /// <summary>
    /// StorySubPostCommentReaction
    /// </summary>
    StorySubPostCommentReaction = 33,

    /// <summary>
    /// StoryPostCommentMention
    /// </summary>
    StoryPostCommentMention = 37,

    /// <summary>
    /// StorySubPostCommentMention
    /// </summary>
    StorySubPostCommentMention = 39,

    /// <summary>
    /// StoryPostCommentReplyReaction
    /// </summary>
    StoryPostCommentReplyReaction = 44,

    /// <summary>
    /// StorySubPostCommentReplyReaction
    /// </summary>
    StorySubPostCommentReplyReaction = 45,

    /// <summary>
    /// StoryPostDelete
    /// </summary>
    StoryPostDelete = 57,

    /// <summary>
    /// StorySubPostDelete
    /// </summary>
    StorySubPostDelete = 58,

    /// <summary>
    /// StoryPostCommentDelete
    /// </summary>
    StoryPostCommentDelete = 416,

    /// <summary>
    /// StorySubPostCommentDelete
    /// </summary>
    StorySubPostCommentDelete = 417,

    /// <summary>
    /// StoryPostLock
    /// </summary>
    StoryPostLock = 62,

    /// <summary>
    /// StorySubPostLock
    /// </summary>
    StorySubPostLock = 63,
    #endregion

    /// <summary>
    /// Video
    /// </summary>
    Video = 20,

    /// <summary>
    /// FollowUser
    /// </summary>
    FollowUser = 23,

    /// <summary>
    /// TransferTransaction
    /// </summary>
    TransferTransaction = 46,

    /// <summary>
    /// DonateTransaction
    /// </summary>
    DonateTransaction = 47,

    /// <summary>
    /// RejectCommentReport
    /// </summary>
    RejectCommentReport = 64,

    /// <summary>
    /// RejectPostReport
    /// </summary>
    RejectPostReport = 65
}
