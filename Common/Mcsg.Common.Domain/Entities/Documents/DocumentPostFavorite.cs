using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentPostFavorite : BasePostFavorite
{
    [ForeignKey("PostId")]
    [InverseProperty("DocumentPostFavorites")]
    public virtual DocumentPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("DocumentPostFavorites")]
    public virtual User User { get; set; } = null!;
}
