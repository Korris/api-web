using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicSubPostComment : BasePostComment
{
    [ForeignKey("AuthorId")]
    [InverseProperty("ComicSubPostComments")]
    public virtual User Author { get; set; } = null!;

    [InverseProperty("Target")]
    public virtual ICollection<ComicSubPostCommentReaction> ComicSubPostCommentReactions { get; set; } = new List<ComicSubPostCommentReaction>();

    [ForeignKey("PostId")]
    [InverseProperty("ComicSubPostComments")]
    public virtual ComicSubPost Post { get; set; } = null!;

    [ForeignKey("ResourceId")]
    [InverseProperty("ComicSubPostComments")]
    public virtual ComicResource? Resource { get; set; }
}
