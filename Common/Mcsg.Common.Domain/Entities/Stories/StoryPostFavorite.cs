using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StoryPostFavorite : BasePostFavorite
{
    [ForeignKey("PostId")]
    [InverseProperty("StoryPostFavorites")]
    public virtual StoryPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("StoryPostFavorites")]
    public virtual User User { get; set; } = null!;
}
