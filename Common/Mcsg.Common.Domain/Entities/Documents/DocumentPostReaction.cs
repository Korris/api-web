using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentPostReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("DocumentPostReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("DocumentPostReactions")]
    public virtual DocumentPost Target { get; set; } = null!;
}
