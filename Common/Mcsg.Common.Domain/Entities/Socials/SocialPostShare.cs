using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialPostShare : BasePostShare
{
    [ForeignKey("PostId")]
    [InverseProperty("SocialPostShares")]
    public virtual SocialPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("SocialPostShares")]
    public virtual User User { get; set; } = null!;
}
