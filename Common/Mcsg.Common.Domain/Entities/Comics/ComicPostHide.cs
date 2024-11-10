using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicPostHide : BasePostHide
{
    [ForeignKey("PostId")]
    [InverseProperty("ComicPostHides")]
    public virtual ComicPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ComicPostHides")]
    public virtual User User { get; set; } = null!;
}
