namespace Mcsg.Api.Areas.Game.Models;

using Common.Core.Enums;

/// <summary>
/// Game post returned by create / update / detail / list
/// </summary>
public class GamePostResponse
{
    public Guid Id { get; set; }
    public string HashId { get; set; } = default!;
    public string? Title { get; set; }
    public string? Body { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? GameUrl { get; set; }
    public string? AuthorName { get; set; }
    public bool IsCurrentUserAuthor { get; set; }
    public bool IsMature { get; set; }
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
    /// Public comments + replies
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// Reaction summary (same shape as Social / Comic)
    /// </summary>
    public ReactionSummaryResponse? Reaction { get; set; }

    /// <summary>
    /// Hashtag names (without '#')
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();
}
