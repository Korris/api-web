using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentPostCommentReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("DocumentPostCommentReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("DocumentPostCommentReactions")]
    public virtual DocumentPostComment Target { get; set; } = null!;
}
