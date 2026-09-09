using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

/// <summary>
/// Reaction on a game post (one row per user per post)
/// </summary>
public partial class GamePostReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("GamePostReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("GamePostReactions")]
    public virtual GamePost Target { get; set; } = null!;
}
