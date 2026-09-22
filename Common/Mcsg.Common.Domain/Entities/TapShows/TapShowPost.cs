using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

/// <summary>
/// TapShow post: an interactive branching visual story (post → chapters → segments → choices).
/// Inherits BasePost like ComicPost; only ThumbnailUrl is used (CoverUrl stays unused).
/// </summary>
public partial class TapShowPost : BasePost
{
    [ForeignKey("UserId")]
    [InverseProperty("TapShowPosts")]
    public virtual User User { get; set; } = null!;

    [InverseProperty("Post")]
    public virtual ICollection<TapShowChapter> TapShowChapters { get; set; } = new List<TapShowChapter>();

    [InverseProperty("Post")]
    public virtual ICollection<TapShowCharacter> TapShowCharacters { get; set; } = new List<TapShowCharacter>();

    [InverseProperty("Post")]
    public virtual ICollection<TapShowPostComment> TapShowPostComments { get; set; } = new List<TapShowPostComment>();

    [InverseProperty("Target")]
    public virtual ICollection<TapShowPostReaction> TapShowPostReactions { get; set; } = new List<TapShowPostReaction>();

    [InverseProperty("Post")]
    public virtual ICollection<TapShowResource> TapShowResources { get; set; } = new List<TapShowResource>();
}
