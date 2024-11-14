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
    ComicPostComment,

    /// <summary>
    /// StoryPostComment
    /// </summary>
    StoryPostComment,
    #endregion

    #region -- SubPostComment --
    /// <summary>
    /// SocialSubPostComment
    /// </summary>
    SocialSubPostComment = 7,

    /// <summary>
    /// ComicSubPostComment
    /// </summary>
    ComicSubPostComment,

    /// <summary>
    /// StorySubPostComment
    /// </summary>
    StorySubPostComment,
    #endregion

    #region -- PostCommentReply --
    /// <summary>
    /// SocialPostCommentReply
    /// </summary>
    SocialPostCommentReply = 10,

    /// <summary>
    /// ComicPostCommentReply
    /// </summary>
    ComicPostCommentReply,

    /// <summary>
    /// StoryPostCommentReply
    /// </summary>
    StoryPostCommentReply,
    #endregion

    #region -- SubPostCommentReply --
    /// <summary>
    /// SocialSubPostCommentReply
    /// </summary>
    SocialSubPostCommentReply = 13,

    /// <summary>
    /// ComicSubPostCommentReply
    /// </summary>
    ComicSubPostCommentReply,

    /// <summary>
    /// StorySubPostCommentReply
    /// </summary>
    StorySubPostCommentReply,
    #endregion

    #region -- Social * Reaction --
    /// <summary>
    /// SocialPostCommentReaction
    /// </summary>
    SocialPostCommentReaction = 16,

    /// <summary>
    /// SocialSubPostCommentReaction
    /// </summary>
    SocialSubPostCommentReaction,

    /// <summary>
    /// SocialPostReaction
    /// </summary>
    SocialPostReaction,

    /// <summary>
    /// SocialSubPostReaction
    /// </summary>
    SocialSubPostReaction,
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
    SocialSubPostCommentMention,
    #endregion

    #region -- Follow --
    /// <summary>
    /// FollowUser
    /// </summary>
    FollowUser = 23,

    /// <summary>
    /// FollowComicPost
    /// </summary>
    FollowComicPost,

    /// <summary>
    /// FollowStoryPost
    /// </summary>
    FollowStoryPost,
    #endregion

    #region -- Comic * Reaction --
    /// <summary>
    /// ComicPostReaction
    /// </summary>
    ComicPostReaction = 26,

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
    #endregion

    #region -- Story * Reaction --
    /// <summary>
    /// StoryPostReaction
    /// </summary>
    StoryPostReaction = 30,

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
    #endregion

    #region -- Social * Mention --
    /// <summary>
    /// SocialPostMention
    /// </summary>
    SocialPostMention = 34,

    /// <summary>
    /// SocialSubPostMention
    /// </summary>
    SocialSubPostMention,
    #endregion

    #region -- Comic Story Mention --
    /// <summary>
    /// ComicCommentPostMention
    /// </summary>
    ComicPostCommentMention = 36,

    /// <summary>
    /// StoryCommentPostMention
    /// </summary>
    StoryPostCommentMention,

    /// <summary>
    /// ComicSubPostCommentPostMention
    /// </summary>
    ComicSubPostCommentMention,

    /// <summary>
    /// StorySubPostCommentPostMention
    /// </summary>
    StorySubPostCommentMention,
    #endregion

    #region -- Reaction --
    /// <summary>
    /// SocialPostCommentReplyReaction
    /// </summary>
    SocialPostCommentReplyReaction,

    /// <summary>
    /// SocialSubPostCommentReplyReaction
    /// </summary>
    SocialSubPostCommentReplyReaction,

    /// <summary>
    /// ComicPostCommentReplyReaction
    /// </summary>
    ComicPostCommentReplyReaction,

    /// <summary>
    /// ComicSubPostCommentReplyReaction
    /// </summary>
    ComicSubPostCommentReplyReaction,

    /// <summary>
    /// StoryPostCommentReplyReaction
    /// </summary>
    StoryPostCommentReplyReaction,

    /// <summary>
    /// StorySubPostCommentReplyReaction
    /// </summary>
    StorySubPostCommentReplyReaction,
    #endregion

    /// <summary>
    /// TransferTransaction
    /// </summary>
    TransferTransaction,

    /// <summary>
    /// DonateTransaction
    /// </summary>
    DonateTransaction,

    #region -- Document --
    /// <summary>
    /// Document
    /// </summary>
    Document,

    /// <summary>
    /// DocumentPostReaction
    /// </summary>
    DocumentPostReaction,

    /// <summary>
    /// DocumentPostCommentReplyReaction
    /// </summary>
    DocumentPostCommentReplyReaction,

    /// <summary>
    /// DocumentPostCommentReaction
    /// </summary>
    DocumentPostCommentReaction,

    /// <summary>
    /// DocumentSubPostCommentReplyReaction
    /// </summary>
    DocumentSubPostCommentReplyReaction,

    /// <summary>
    /// DocumentSubPostCommentReaction
    /// </summary>
    DocumentSubPostCommentReaction,
    #endregion

    #region -- Delete --
    /// <summary>
    /// DeleteComicPost
    /// </summary>
    DeleteComicPost,

    /// <summary>
    /// DeleteComicSubPost
    /// </summary>
    DeleteComicSubPost,

    /// <summary>
    /// DeleteSocial
    /// </summary>
    DeleteSocial,

    /// <summary>
    /// DeleteStoryPost
    /// </summary>
    DeleteStoryPost,

    /// <summary>
    /// DeleteStorySubPost
    /// </summary>
    DeleteStorySubPost,
    #endregion

    #region -- Lock --
    /// <summary>
    /// LockComicPost
    /// </summary>
    LockComicPost,

    /// <summary>
    /// LockComicSubPost
    /// </summary>
    LockComicSubPost,

    /// <summary>
    /// LockSocial
    /// </summary>
    LockSocial,

    /// <summary>
    /// LockStoryPost
    /// </summary>
    LockStoryPost,

    /// <summary>
    /// LockStorySubPost
    /// </summary>
    LockStorySubPost,
    #endregion

    #region -- Reject --
    /// <summary>
    /// RejectCommentReport
    /// </summary>
    RejectCommentReport,

    /// <summary>
    /// RejectPostReport
    /// </summary>
    RejectPostReport
    #endregion
}
