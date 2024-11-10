using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StoryPostComment : BasePostComment
{
    [ForeignKey("AuthorId")]
    [InverseProperty("StoryPostComments")]
    public virtual User Author { get; set; } = null!;

    [InverseProperty("Target")]
    public virtual ICollection<StoryPostCommentReaction> StoryPostCommentReactions { get; set; } = new List<StoryPostCommentReaction>();

    [ForeignKey("PostId")]
    [InverseProperty("StoryPostComments")]
    public virtual StoryPost Post { get; set; } = null!;

    [ForeignKey("ResourceId")]
    [InverseProperty("StoryPostComments")]
    public virtual StoryResource? Resource { get; set; }
}
