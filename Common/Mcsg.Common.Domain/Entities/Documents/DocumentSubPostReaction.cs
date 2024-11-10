using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentSubPostReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("DocumentSubPostReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("DocumentSubPostReactions")]
    public virtual DocumentSubPost Target { get; set; } = null!;
}
