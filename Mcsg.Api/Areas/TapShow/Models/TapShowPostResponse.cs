namespace Mcsg.Api.Areas.TapShow.Models;

using Common.Core.Enums;

/// <summary>
/// TapShow post returned by create / update / detail / list
/// </summary>
public class TapShowPostResponse
{
    public Guid Id { get; set; }
    public string HashId { get; set; } = default!;
    public string? Title { get; set; }
    public string? Body { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? AuthorName { get; set; }
    public bool IsCurrentUserAuthor { get; set; }
    public bool IsMature { get; set; }
    public bool IsCompleted { get; set; }
    public PostPermission Permission { get; set; }
    public PostStatus Status { get; set; }
    public PostType Type { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public string? ProfileName { get; set; }
    public string? ProfileId { get; set; }
    public string? UserAvatar { get; set; }

    /// <summary>
    /// Public chapters (all chapters for the owner)
    /// </summary>
    public int ChapterCount { get; set; }

    /// <summary>
    /// Public comments + replies
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// Reaction summary (same shape as Social / Comic)
    /// </summary>
    public ReactionSummaryResponse? Reaction { get; set; }

    /// <summary>
    /// Characters ordered by Order. Filled on create / update / detail only (null in lists); avatarHashId only for the owner
    /// </summary>
    public List<CharacterResponse>? Characters { get; set; }

    /// <summary>
    /// Most-reacted root comments (preview, same as Story list). Filled in lists only (null in detail)
    /// </summary>
    public CommentPagedResults<MostReactionCommentResponse>? Comments { get; set; }

    /// <summary>
    /// Hashtag names (without '#')
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();
}
