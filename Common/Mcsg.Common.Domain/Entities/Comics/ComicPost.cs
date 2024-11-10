using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicPost : BasePost
{
    [InverseProperty("Post")]
    public virtual ICollection<ComicPostComment> ComicPostComments { get; set; } = new List<ComicPostComment>();

    [InverseProperty("Post")]
    public virtual ICollection<ComicPostFavorite> ComicPostFavorites { get; set; } = new List<ComicPostFavorite>();

    [InverseProperty("Post")]
    public virtual ICollection<ComicPostHide> ComicPostHides { get; set; } = new List<ComicPostHide>();

    [InverseProperty("Post")]
    public virtual ICollection<ComicPostLink> ComicPostLinks { get; set; } = new List<ComicPostLink>();

    [InverseProperty("Target")]
    public virtual ICollection<ComicPostReaction> ComicPostReactions { get; set; } = new List<ComicPostReaction>();

    [InverseProperty("Post")]
    public virtual ICollection<ComicPostShare> ComicPostShares { get; set; } = new List<ComicPostShare>();

    [InverseProperty("Post")]
    public virtual ICollection<ComicSubPost> ComicSubPosts { get; set; } = new List<ComicSubPost>();

    [InverseProperty("Post")]
    public virtual ICollection<ComicTagPost> ComicTagPosts { get; set; } = new List<ComicTagPost>();

    [ForeignKey("UserId")]
    [InverseProperty("ComicPosts")]
    public virtual User User { get; set; } = null!;
}
