using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StoryPostCommentReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("StoryPostCommentReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("StoryPostCommentReactions")]
    public virtual StoryPostComment Target { get; set; } = null!;
}
