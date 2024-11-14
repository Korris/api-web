namespace Mcsg.Common.Core.Enums;

/// <summary>
/// NotificationEntity type
/// </summary>
public enum NotificationEntityType
{
    /// <summary>
    /// PostComment
    /// </summary>
    PostComment = 4,

    /// <summary>
    /// ComicPostComment
    /// </summary>
    ComicPostComment,

    /// <summary>
    /// StoryPostComment
    /// </summary>
    StoryPostComment,

    /// <summary>
    /// SubPostComment
    /// </summary>
    SubPostComment,

    /// <summary>
    /// ComicSubPostComment
    /// </summary>
    ComicSubPostComment,

    /// <summary>
    /// StorySubPostComment
    /// </summary>
    StorySubPostComment,

    /// <summary>
    /// PostCommentReply
    /// </summary>
    PostCommentReply,

    /// <summary>
    /// ComicPostCommentReply
    /// </summary>
    ComicPostCommentReply,

    /// <summary>
    /// StoryPostCommentReply
    /// </summary>
    StoryPostCommentReply,

    /// <summary>
    /// SubPostCommentReply
    /// </summary>
    SubPostCommentReply,

    /// <summary>
    /// ComicSubPostCommentReply
    /// </summary>
    ComicSubPostCommentReply,

    /// <summary>
    /// StorySubPostCommentReply
    /// </summary>
    StorySubPostCommentReply,

    /// <summary>
    /// PostCommentReaction
    /// </summary>
    PostCommentReaction,

    /// <summary>
    /// SubPostCommentReaction
    /// </summary>
    SubPostCommentReaction,

    /// <summary>
    /// PostReaction
    /// </summary>
    PostReaction,

    /// <summary>
    /// SubPostReaction
    /// </summary>
    SubPostReaction,

    /// <summary>
    /// Video
    /// </summary>
    Video,

    /// <summary>
    /// PostCommentMention
    /// </summary>
    PostCommentMention,

    /// <summary>
    /// SubPostCommentMention
    /// </summary>
    SubPostCommentMention,

    /// <summary>
    /// FollowUser
    /// </summary>
    FollowUser,

    /// <summary>
    /// FollowComicPost
    /// </summary>
    FollowComicPost,

    /// <summary>
    /// FollowStoryPost
    /// </summary>
    FollowStoryPost,

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
    /// PostMention
    /// </summary>
    PostMention,

    /// <summary>
    /// SubPostMention
    /// </summary>
    SubPostMention,

    /// <summary>
    /// ComicCommentPostMention
    /// </summary>
    ComicPostCommentMention,

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

    /// <summary>
    /// PostCommentReplyReaction
    /// </summary>
    PostCommentReplyReaction,

    /// <summary>
    /// SubPostCommentReplyReaction
    /// </summary>
    SubPostCommentReplyReaction,

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
