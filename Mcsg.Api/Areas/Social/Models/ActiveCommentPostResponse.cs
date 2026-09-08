namespace Mcsg.Api.Areas.Social.Models;

using Common.Core.Enums;

/// <summary>
/// Card for the "posts I am commenting on" list: a post the user has commented on, ordered by its most recent
/// comment, with comment counters and the newest comment plus its author.
/// Flat shape on purpose so Dapper maps it in one row without a custom splitter.
/// </summary>
public class ActiveCommentPostResponse
{
    /// <summary>
    /// Post id
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// Post hash id
    /// </summary>
    public string? HashId { get; set; }

    /// <summary>
    /// Feed, Story, Comic or Document
    /// </summary>
    public PostType Type { get; set; }

    /// <summary>
    /// Post title; feed posts have no title so the first 100 characters of the body are used
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// All public, non-deleted comments on the post and its chapters, replies included
    /// </summary>
    public int TotalComments { get; set; }

    /// <summary>
    /// Comments by other people written after this user's last comment on the post (unseen replies)
    /// </summary>
    public int NewComments { get; set; }

    /// <summary>
    /// When the newest comment was created (UTC)
    /// </summary>
    public DateTime LastCommentOn { get; set; }

    /// <summary>
    /// Body of the newest comment
    /// </summary>
    public string? LastCommentBody { get; set; }

    /// <summary>
    /// Newest comment author: user name
    /// </summary>
    public string? LastCommentUserName { get; set; }

    /// <summary>
    /// Newest comment author: display name
    /// </summary>
    public string? LastCommentProfileName { get; set; }

    /// <summary>
    /// Newest comment author: avatar url
    /// </summary>
    public string? LastCommentAvatar { get; set; }
}
