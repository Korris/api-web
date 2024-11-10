using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentTagPost : BaseTagPost
{
    [ForeignKey("PostId")]
    [InverseProperty("DocumentTagPosts")]
    public virtual DocumentPost Post { get; set; } = null!;

    [ForeignKey("TagId")]
    [InverseProperty("DocumentTagPosts")]
    public virtual Tag Tag { get; set; } = null!;
}
