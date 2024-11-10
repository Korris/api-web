using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialPostFavorite : BasePostFavorite
{
    [ForeignKey("PostId")]
    [InverseProperty("SocialPostFavorites")]
    public virtual SocialPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("SocialPostFavorites")]
    public virtual User User { get; set; } = null!;
}
