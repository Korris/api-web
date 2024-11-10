using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicSubPostReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("ComicSubPostReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("ComicSubPostReactions")]
    public virtual ComicSubPost Target { get; set; } = null!;
}
