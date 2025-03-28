using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialPost : BasePost
{
    [InverseProperty("Post")]
    public virtual ICollection<BackgroundMediaPost> BackgroundMediaPosts { get; set; } = new List<BackgroundMediaPost>();

    [InverseProperty("Post")]
    public virtual ICollection<SocialPostComment> SocialPostComments { get; set; } = new List<SocialPostComment>();

    [InverseProperty("Post")]
    public virtual ICollection<SocialPostFavorite> SocialPostFavorites { get; set; } = new List<SocialPostFavorite>();

    [InverseProperty("Post")]
    public virtual ICollection<SocialPostHide> SocialPostHides { get; set; } = new List<SocialPostHide>();

    [InverseProperty("Post")]
    public virtual ICollection<SocialPostLink> SocialPostLinks { get; set; } = new List<SocialPostLink>();

    [InverseProperty("Target")]
    public virtual ICollection<SocialPostReaction> SocialPostReactions { get; set; } = new List<SocialPostReaction>();

    [InverseProperty("Post")]
    public virtual ICollection<SocialPostShare> SocialPostShares { get; set; } = new List<SocialPostShare>();

    [InverseProperty("Post")]
    public virtual ICollection<SocialSubPost> SocialSubPosts { get; set; } = new List<SocialSubPost>();

    [InverseProperty("Post")]
    public virtual ICollection<SocialTagPost> SocialTagPosts { get; set; } = new List<SocialTagPost>();

    [ForeignKey("UserId")]
    [InverseProperty("SocialPosts")]
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// IsLongText
    /// </summary>
    public bool IsLongText { get; set; }
}
