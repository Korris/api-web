using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicPostFavorite : BasePostFavorite
{
    [ForeignKey("PostId")]
    [InverseProperty("ComicPostFavorites")]
    public virtual ComicPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ComicPostFavorites")]
    public virtual User User { get; set; } = null!;
}
