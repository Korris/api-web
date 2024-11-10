using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicPostCommentReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("ComicPostCommentReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("ComicPostCommentReactions")]
    public virtual ComicPostComment Target { get; set; } = null!;
}
