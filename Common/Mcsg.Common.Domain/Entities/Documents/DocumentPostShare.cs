using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentPostShare : BasePostShare
{
    [ForeignKey("PostId")]
    [InverseProperty("DocumentPostShares")]
    public virtual DocumentPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("DocumentPostShares")]
    public virtual User User { get; set; } = null!;
}
