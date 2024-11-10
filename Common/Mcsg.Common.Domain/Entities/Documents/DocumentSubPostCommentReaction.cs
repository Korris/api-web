using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentSubPostCommentReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("DocumentSubPostCommentReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("DocumentSubPostCommentReactions")]
    public virtual DocumentSubPostComment Target { get; set; } = null!;
}
