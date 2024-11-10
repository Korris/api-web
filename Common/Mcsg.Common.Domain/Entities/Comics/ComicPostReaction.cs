using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicPostReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("ComicPostReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("ComicPostReactions")]
    public virtual ComicPost Target { get; set; } = null!;
}
