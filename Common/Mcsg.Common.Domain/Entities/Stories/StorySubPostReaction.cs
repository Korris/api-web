using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StorySubPostReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("StorySubPostReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("StorySubPostReactions")]
    public virtual StorySubPost Target { get; set; } = null!;
}
