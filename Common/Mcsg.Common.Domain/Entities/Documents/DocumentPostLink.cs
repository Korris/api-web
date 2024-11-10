using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentPostLink : BasePostLink
{
    [ForeignKey("PostId")]
    [InverseProperty("DocumentPostLinks")]
    public virtual DocumentPost Post { get; set; } = null!;
}
