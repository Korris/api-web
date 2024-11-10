using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentPost : BasePost
{
    public bool IsAllowDownload { get; set; }

    [InverseProperty("Post")]
    public virtual ICollection<DocumentPostComment> DocumentPostComments { get; set; } = new List<DocumentPostComment>();

    [InverseProperty("Post")]
    public virtual ICollection<DocumentPostFavorite> DocumentPostFavorites { get; set; } = new List<DocumentPostFavorite>();

    [InverseProperty("Post")]
    public virtual ICollection<DocumentPostHide> DocumentPostHides { get; set; } = new List<DocumentPostHide>();

    [InverseProperty("Post")]
    public virtual ICollection<DocumentPostLink> DocumentPostLinks { get; set; } = new List<DocumentPostLink>();

    [InverseProperty("Target")]
    public virtual ICollection<DocumentPostReaction> DocumentPostReactions { get; set; } = new List<DocumentPostReaction>();

    [InverseProperty("Post")]
    public virtual ICollection<DocumentPostShare> DocumentPostShares { get; set; } = new List<DocumentPostShare>();

    [InverseProperty("Post")]
    public virtual ICollection<DocumentSubPost> DocumentSubPosts { get; set; } = new List<DocumentSubPost>();

    [InverseProperty("Post")]
    public virtual ICollection<DocumentTagPost> DocumentTagPosts { get; set; } = new List<DocumentTagPost>();

    [ForeignKey("UserId")]
    [InverseProperty("DocumentPosts")]
    public virtual User User { get; set; } = null!;
}
