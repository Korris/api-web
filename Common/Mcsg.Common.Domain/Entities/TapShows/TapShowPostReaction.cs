using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

/// <summary>
/// Reaction on a TapShow post (one row per user per post)
/// </summary>
public partial class TapShowPostReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("TapShowPostReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("TapShowPostReactions")]
    public virtual TapShowPost Target { get; set; } = null!;
}
