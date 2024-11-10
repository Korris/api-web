using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StorySubPostCommentReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("StorySubPostCommentReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("StorySubPostCommentReactions")]
    public virtual StorySubPostComment Target { get; set; } = null!;
}
