using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentPostComment : BasePostComment
{
    [ForeignKey("AuthorId")]
    [InverseProperty("DocumentPostComments")]
    public virtual User Author { get; set; } = null!;

    [InverseProperty("Target")]
    public virtual ICollection<DocumentPostCommentReaction> DocumentPostCommentReactions { get; set; } = new List<DocumentPostCommentReaction>();

    [ForeignKey("PostId")]
    [InverseProperty("DocumentPostComments")]
    public virtual DocumentPost Post { get; set; } = null!;

    [ForeignKey("ResourceId")]
    [InverseProperty("DocumentPostComments")]
    public virtual DocumentResource? Resource { get; set; }
}
