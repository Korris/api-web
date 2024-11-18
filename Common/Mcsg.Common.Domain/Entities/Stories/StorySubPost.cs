using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StorySubPost : BaseSubPost
{
    public bool IsPremium { get; set; }

    public float Sort { get; set; } = 0;

    [InverseProperty("SubPost")]
    public virtual ICollection<StoryResource> StoryResources { get; set; } = new List<StoryResource>();

    [InverseProperty("Post")]
    public virtual ICollection<StorySubPostComment> StorySubPostComments { get; set; } = new List<StorySubPostComment>();

    [InverseProperty("Target")]
    public virtual ICollection<StorySubPostReaction> StorySubPostReactions { get; set; } = new List<StorySubPostReaction>();

    [ForeignKey("PostId")]
    [InverseProperty("StorySubPosts")]
    public virtual StoryPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("StorySubPosts")]
    public virtual User User { get; set; } = null!;
}
