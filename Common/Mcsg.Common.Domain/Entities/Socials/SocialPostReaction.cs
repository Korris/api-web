using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialPostReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("SocialPostReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("SocialPostReactions")]
    public virtual SocialPost Target { get; set; } = null!;
}
