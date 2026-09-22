using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

/// <summary>
/// Comment (or reply via ParentId) on a TapShow post. Copy of GamePostComment.
/// </summary>
public partial class TapShowPostComment : BasePostComment
{
    [ForeignKey("AuthorId")]
    [InverseProperty("TapShowPostComments")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("PostId")]
    [InverseProperty("TapShowPostComments")]
    public virtual TapShowPost Post { get; set; } = null!;

    [InverseProperty("Target")]
    public virtual ICollection<TapShowPostCommentReaction> TapShowPostCommentReactions { get; set; } = new List<TapShowPostCommentReaction>();
}
