using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

/// <summary>
/// Reaction on a TapShow post comment (one row per user per comment)
/// </summary>
public partial class TapShowPostCommentReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("TapShowPostCommentReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("TapShowPostCommentReactions")]
    public virtual TapShowPostComment Target { get; set; } = null!;
}
