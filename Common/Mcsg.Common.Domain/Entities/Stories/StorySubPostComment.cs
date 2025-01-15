using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StorySubPostComment : BasePostComment
{
    [ForeignKey("AuthorId")]
    [InverseProperty("StorySubPostComments")]
    public virtual User Author { get; set; } = null!;

    [InverseProperty("Target")]
    public virtual ICollection<StorySubPostCommentReaction> StorySubPostCommentReactions { get; set; } = new List<StorySubPostCommentReaction>();

    [ForeignKey("PostId")]
    [InverseProperty("StorySubPostComments")]
    public virtual StorySubPost Post { get; set; } = null!;

    [ForeignKey("ResourceId")]
    [InverseProperty("StorySubPostComments")]
    public virtual StoryResource? Resource { get; set; }

    public Guid? ParagraphId { get; set; }
}
