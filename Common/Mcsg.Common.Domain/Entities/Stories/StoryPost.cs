using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StoryPost : BasePost
{
    [InverseProperty("Post")]
    public virtual ICollection<StoryPostComment> StoryPostComments { get; set; } = new List<StoryPostComment>();

    [InverseProperty("Post")]
    public virtual ICollection<StoryPostFavorite> StoryPostFavorites { get; set; } = new List<StoryPostFavorite>();

    [InverseProperty("Post")]
    public virtual ICollection<StoryPostHide> StoryPostHides { get; set; } = new List<StoryPostHide>();

    [InverseProperty("Post")]
    public virtual ICollection<StoryPostLink> StoryPostLinks { get; set; } = new List<StoryPostLink>();

    [InverseProperty("Target")]
    public virtual ICollection<StoryPostReaction> StoryPostReactions { get; set; } = new List<StoryPostReaction>();

    [InverseProperty("Post")]
    public virtual ICollection<StoryPostShare> StoryPostShares { get; set; } = new List<StoryPostShare>();

    [InverseProperty("Post")]
    public virtual ICollection<StorySubPost> StorySubPosts { get; set; } = new List<StorySubPost>();

    [InverseProperty("Post")]
    public virtual ICollection<StoryTagPost> StoryTagPosts { get; set; } = new List<StoryTagPost>();

    [ForeignKey("UserId")]
    [InverseProperty("StoryPosts")]
    public virtual User User { get; set; } = null!;
}
