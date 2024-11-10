using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialSubPostReaction : BaseReaction
{
    [ForeignKey("AuthorId")]
    [InverseProperty("SocialSubPostReactions")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("TargetId")]
    [InverseProperty("SocialSubPostReactions")]
    public virtual SocialSubPost Target { get; set; } = null!;
}
