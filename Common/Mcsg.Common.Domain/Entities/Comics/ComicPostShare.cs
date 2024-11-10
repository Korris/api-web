using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicPostShare : BasePostShare
{
    [ForeignKey("PostId")]
    [InverseProperty("ComicPostShares")]
    public virtual ComicPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ComicPostShares")]
    public virtual User User { get; set; } = null!;
}
