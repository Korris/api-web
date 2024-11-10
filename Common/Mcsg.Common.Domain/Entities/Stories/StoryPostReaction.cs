using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StoryPostReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("StoryPostReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("StoryPostReactions")]
    public virtual StoryPost Target { get; set; } = null!;
}
