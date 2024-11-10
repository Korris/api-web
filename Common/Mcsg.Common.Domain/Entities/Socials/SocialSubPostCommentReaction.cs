using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialSubPostCommentReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("SocialSubPostCommentReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("SocialSubPostCommentReactions")]
    public virtual SocialSubPostComment Target { get; set; } = null!;
}
