using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentResource : BaseResource
{
    [ForeignKey("AuthorId")]
    [InverseProperty("DocumentResources")]
    public virtual User? Author { get; set; }

    [InverseProperty("Resource")]
    public virtual ICollection<DocumentPostComment> DocumentPostComments { get; set; } = new List<DocumentPostComment>();

    [InverseProperty("Resource")]
    public virtual ICollection<DocumentSubPostComment> DocumentSubPostComments { get; set; } = new List<DocumentSubPostComment>();

    [ForeignKey("SubPostId")]
    [InverseProperty("DocumentResources")]
    public virtual DocumentSubPost? SubPost { get; set; }
}
