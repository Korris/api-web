using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicSubPostCommentReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("ComicSubPostCommentReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("ComicSubPostCommentReactions")]
    public virtual ComicSubPostComment Target { get; set; } = null!;
}
