using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicSubPost : BaseSubPost
{
    public float Order { get; set; }

    public bool IsPremium { get; set; }

    public float Sort { get; set; } = 0;

    [InverseProperty("SubPost")]
    public virtual ICollection<ComicResource> ComicResources { get; set; } = new List<ComicResource>();

    [InverseProperty("Post")]
    public virtual ICollection<ComicSubPostComment> ComicSubPostComments { get; set; } = new List<ComicSubPostComment>();

    [InverseProperty("Target")]
    public virtual ICollection<ComicSubPostReaction> ComicSubPostReactions { get; set; } = new List<ComicSubPostReaction>();

    [ForeignKey("PostId")]
    [InverseProperty("ComicSubPosts")]
    public virtual ComicPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ComicSubPosts")]
    public virtual User User { get; set; } = null!;
}
