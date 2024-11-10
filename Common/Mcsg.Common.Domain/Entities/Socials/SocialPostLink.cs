using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialPostLink : BasePostLink
{
    [ForeignKey("PostId")]
    [InverseProperty("SocialPostLinks")]
    public virtual SocialPost Post { get; set; } = null!;
}
