using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialPostHide : BasePostHide
{
    [ForeignKey("PostId")]
    [InverseProperty("SocialPostHides")]
    public virtual SocialPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("SocialPostHides")]
    public virtual User User { get; set; } = null!;
}
