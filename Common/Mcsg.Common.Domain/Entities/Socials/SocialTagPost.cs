using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialTagPost : BaseTagPost
{
    [ForeignKey("PostId")]
    [InverseProperty("SocialTagPosts")]
    public virtual SocialPost Post { get; set; } = null!;

    [ForeignKey("TagId")]
    [InverseProperty("SocialTagPosts")]
    public virtual Tag Tag { get; set; } = null!;
}
