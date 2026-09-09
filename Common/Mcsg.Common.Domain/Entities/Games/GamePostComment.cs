using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

/// <summary>
/// Comment (or reply via ParentId) on a game post. Copy of ComicPostComment without the Resource navigation.
/// </summary>
public partial class GamePostComment : BasePostComment
{
    [ForeignKey("AuthorId")]
    [InverseProperty("GamePostComments")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("PostId")]
    [InverseProperty("GamePostComments")]
    public virtual GamePost Post { get; set; } = null!;

    [InverseProperty("Target")]
    public virtual ICollection<GamePostCommentReaction> GamePostCommentReactions { get; set; } = new List<GamePostCommentReaction>();
}
