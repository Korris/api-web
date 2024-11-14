namespace Mcsg.Common.Core.Enums;

/// <summary>
/// NotificationEntity type
/// </summary>
public enum NotificationEntityType
{
    #region -- PostComment --
    /// <summary>
    /// SocialPostComment
    /// </summary>
    SocialPostComment = 4,

    /// <summary>
    /// ComicPostComment
    /// </summary>
    ComicPostComment = 5,

    /// <summary>
    /// StoryPostComment
    /// </summary>
    StoryPostComment = 6,
    #endregion

    #region -- SubPostComment --
    /// <summary>
    /// SocialSubPostComment
    /// </summary>
    SocialSubPostComment = 7,

    /// <summary>
    /// ComicSubPostComment
    /// </summary>
    ComicSubPostComment = 8,

    /// <summary>
    /// StorySubPostComment
    /// </summary>
    StorySubPostComment = 9,
    #endregion

    #region -- PostCommentReply --
    /// <summary>
    /// SocialPostCommentReply
    /// </summary>
    SocialPostCommentReply = 10,

    /// <summary>
    /// ComicPostCommentReply
    /// </summary>
    ComicPostCommentReply = 11,

    /// <summary>
    /// StoryPostCommentReply
    /// </summary>
    StoryPostCommentReply = 12,
    #endregion

    #region -- SubPostCommentReply --
    /// <summary>
    /// SocialSubPostCommentReply
    /// </summary>
    SocialSubPostCommentReply = 13,

    /// <summary>
    /// ComicSubPostCommentReply
    /// </summary>
    ComicSubPostCommentReply = 14,

    /// <summary>
    /// StorySubPostCommentReply
    /// </summary>
    StorySubPostCommentReply = 15,
    #endregion

    #region -- Social * Reaction --
    /// <summary>
    /// SocialPostCommentReaction
    /// </summary>
    SocialPostCommentReaction = 16,

    /// <summary>
    /// SocialSubPostCommentReaction
    /// </summary>
    SocialSubPostCommentReaction = 17,

    /// <summary>
    /// SocialPostReaction
    /// </summary>
    SocialPostReaction = 18,

    /// <summary>
    /// SocialSubPostReaction
    /// </summary>
    SocialSubPostReaction = 19,
    #endregion

    /// <summary>
    /// Video
    /// </summary>
    Video = 20,

    #region -- Social * Mention --
    /// <summary>
    /// SocialPostCommentMention
    /// </summary>
    SocialPostCommentMention = 21,

    /// <summary>
    /// SocialSubPostCommentMention
    /// </summary>
    SocialSubPostCommentMention = 22,
    #endregion

    #region -- Follow --
    /// <summary>
    /// FollowUser
    /// </summary>
    FollowUser = 23,

    /// <summary>
    /// FollowComicPost
    /// </summary>
    FollowComicPost = 24,

    /// <summary>
    /// FollowStoryPost
    /// </summary>
    FollowStoryPost = 25,
    #endregion

    #region -- Comic * Reaction --
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
    #endregion

    #region -- Story * Reaction --
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
    #endregion

    #region -- Social * Mention --
    /// <summary>
    /// SocialPostMention
    /// </summary>
    SocialPostMention = 34,

    /// <summary>
    /// SocialSubPostMention
    /// </summary>
    SocialSubPostMention = 35,
    #endregion

    #region -- Comic Story Mention --
    /// <summary>
    /// ComicCommentPostMention
    /// </summary>
    ComicPostCommentMention = 36,

    /// <summary>
    /// StoryCommentPostMention
    /// </summary>
    StoryPostCommentMention = 37,

    /// <summary>
    /// ComicSubPostCommentPostMention
    /// </summary>
    ComicSubPostCommentMention = 38,

    /// <summary>
    /// StorySubPostCommentPostMention
    /// </summary>
    StorySubPostCommentMention = 39,
    #endregion

    #region -- Reaction --
    /// <summary>
    /// SocialPostCommentReplyReaction
    /// </summary>
    SocialPostCommentReplyReaction = 40,

    /// <summary>
    /// SocialSubPostCommentReplyReaction
    /// </summary>
    SocialSubPostCommentReplyReaction = 41,

    /// <summary>
    /// ComicPostCommentReplyReaction
    /// </summary>
    ComicPostCommentReplyReaction = 42,

    /// <summary>
    /// ComicSubPostCommentReplyReaction
    /// </summary>
    ComicSubPostCommentReplyReaction = 43,

    /// <summary>
    /// StoryPostCommentReplyReaction
    /// </summary>
    StoryPostCommentReplyReaction = 44,

    /// <summary>
    /// StorySubPostCommentReplyReaction
    /// </summary>
    StorySubPostCommentReplyReaction = 45,
    #endregion

    /// <summary>
    /// TransferTransaction
    /// </summary>
    TransferTransaction = 46,

    /// <summary>
    /// DonateTransaction
    /// </summary>
    DonateTransaction = 47,

    #region -- Document --
    /// <summary>
    /// Document
    /// </summary>
    Document = 48,

    /// <summary>
    /// DocumentPostReaction
    /// </summary>
    DocumentPostReaction = 49,

    /// <summary>
    /// DocumentPostCommentReplyReaction
    /// </summary>
    DocumentPostCommentReplyReaction = 50,

    /// <summary>
    /// DocumentPostCommentReaction
    /// </summary>
    DocumentPostCommentReaction = 51,

    /// <summary>
    /// DocumentSubPostCommentReplyReaction
    /// </summary>
    DocumentSubPostCommentReplyReaction = 52,

    /// <summary>
    /// DocumentSubPostCommentReaction
    /// </summary>
    DocumentSubPostCommentReaction = 53,
    #endregion

    #region -- Delete --
    /// <summary>
    /// DeleteComicPost
    /// </summary>
    DeleteComicPost = 54,

    /// <summary>
    /// DeleteComicSubPost
    /// </summary>
    DeleteComicSubPost = 55,

    /// <summary>
    /// DeleteSocial
    /// </summary>
    DeleteSocial = 56,

    /// <summary>
    /// DeleteStoryPost
    /// </summary>
    DeleteStoryPost = 57,

    /// <summary>
    /// DeleteStorySubPost
    /// </summary>
    DeleteStorySubPost = 58,
    #endregion

    #region -- Lock --
    /// <summary>
    /// LockComicPost
    /// </summary>
    LockComicPost = 59,

    /// <summary>
    /// LockComicSubPost
    /// </summary>
    LockComicSubPost = 60,

    /// <summary>
    /// LockSocial
    /// </summary>
    LockSocial = 61,

    /// <summary>
    /// LockStoryPost
    /// </summary>
    LockStoryPost = 62,

    /// <summary>
    /// LockStorySubPost
    /// </summary>
    LockStorySubPost = 63,
    #endregion

    #region -- Reject --
    /// <summary>
    /// RejectCommentReport
    /// </summary>
    RejectCommentReport = 64,

    /// <summary>
    /// RejectPostReport
    /// </summary>
    RejectPostReport = 65
    #endregion
}
