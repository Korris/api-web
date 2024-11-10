using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentSubPostComment : BasePostComment
{
    [ForeignKey("AuthorId")]
    [InverseProperty("DocumentSubPostComments")]
    public virtual User Author { get; set; } = null!;

    [InverseProperty("Target")]
    public virtual ICollection<DocumentSubPostCommentReaction> DocumentSubPostCommentReactions { get; set; } = new List<DocumentSubPostCommentReaction>();

    [ForeignKey("PostId")]
    [InverseProperty("DocumentSubPostComments")]
    public virtual DocumentSubPost Post { get; set; } = null!;

    [ForeignKey("ResourceId")]
    [InverseProperty("DocumentSubPostComments")]
    public virtual DocumentResource? Resource { get; set; }
}
