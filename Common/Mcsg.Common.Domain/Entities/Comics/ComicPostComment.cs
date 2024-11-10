using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicPostComment : BasePostComment
{
    [ForeignKey("AuthorId")]
    [InverseProperty("ComicPostComments")]
    public virtual User Author { get; set; } = null!;

    [InverseProperty("Target")]
    public virtual ICollection<ComicPostCommentReaction> ComicPostCommentReactions { get; set; } = new List<ComicPostCommentReaction>();

    [ForeignKey("PostId")]
    [InverseProperty("ComicPostComments")]
    public virtual ComicPost Post { get; set; } = null!;

    [ForeignKey("ResourceId")]
    [InverseProperty("ComicPostComments")]
    public virtual ComicResource? Resource { get; set; }
}
