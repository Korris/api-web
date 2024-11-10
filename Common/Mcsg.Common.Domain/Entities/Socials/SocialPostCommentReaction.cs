using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialPostCommentReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("SocialPostCommentReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("SocialPostCommentReactions")]
    public virtual SocialPostComment Target { get; set; } = null!;
}
