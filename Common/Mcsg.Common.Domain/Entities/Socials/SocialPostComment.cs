using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialPostComment : BasePostComment
{
    [ForeignKey("AuthorId")]
    [InverseProperty("SocialPostComments")]
    public virtual User Author { get; set; } = null!;

    [InverseProperty("Target")]
    public virtual ICollection<SocialPostCommentReaction> SocialPostCommentReactions { get; set; } = new List<SocialPostCommentReaction>();

    [ForeignKey("PostId")]
    [InverseProperty("SocialPostComments")]
    public virtual SocialPost Post { get; set; } = null!;

    [ForeignKey("ResourceId")]
    [InverseProperty("SocialPostComments")]
    public virtual SocialResource? Resource { get; set; }
}
