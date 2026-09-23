using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

/// <summary>
/// Game post: one post = one HTML game.
/// Inherits BasePost like ComicPost but only ThumbnailUrl is used (CoverUrl stays unused)
/// and adds GameUrl pointing to the uploaded .html file on the public bucket.
/// </summary>
public partial class GamePost : BasePost
{
    /// <summary>
    /// Public URL of the uploaded HTML game file (served from the media domain, embedded via sandboxed iframe)
    /// </summary>
    public string? GameUrl { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("GamePosts")]
    public virtual User User { get; set; } = null!;

    [InverseProperty("Post")]
    public virtual ICollection<GamePostComment> GamePostComments { get; set; } = new List<GamePostComment>();

    [InverseProperty("Target")]
    public virtual ICollection<GamePostReaction> GamePostReactions { get; set; } = new List<GamePostReaction>();

    [InverseProperty("Post")]
    public virtual ICollection<GameResource> GameResources { get; set; } = new List<GameResource>();

    [InverseProperty("Post")]
    public virtual ICollection<GameTagPost> GameTagPosts { get; set; } = new List<GameTagPost>();
}
