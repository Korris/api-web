using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialSubPostComment : BasePostComment
{
    [ForeignKey("AuthorId")]
    [InverseProperty("SocialSubPostComments")]
    public virtual User Author { get; set; } = null!;

    [InverseProperty("Target")]
    public virtual ICollection<SocialSubPostCommentReaction> SocialSubPostCommentReactions { get; set; } = new List<SocialSubPostCommentReaction>();

    [ForeignKey("PostId")]
    [InverseProperty("SocialSubPostComments")]
    public virtual SocialSubPost Post { get; set; } = null!;

    [ForeignKey("ResourceId")]
    [InverseProperty("SocialSubPostComments")]
    public virtual SocialResource? Resource { get; set; }
}
