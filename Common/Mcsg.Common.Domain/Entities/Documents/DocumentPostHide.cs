using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentPostHide : BasePostHide
{
    [ForeignKey("PostId")]
    [InverseProperty("DocumentPostHides")]
    public virtual DocumentPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("DocumentPostHides")]
    public virtual User User { get; set; } = null!;
}
