using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

/// <summary>
/// Reaction on a game post comment (one row per user per comment)
/// </summary>
public partial class GamePostCommentReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("GamePostCommentReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("GamePostCommentReactions")]
    public virtual GamePostComment Target { get; set; } = null!;
}
