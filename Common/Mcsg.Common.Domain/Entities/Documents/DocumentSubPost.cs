using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentSubPost : BaseSubPost
{
    public bool IsPremium { get; set; }

    public float Sort { get; set; } = 0;

    public bool IsAllowDownload { get; set; }

    [InverseProperty("SubPost")]
    public virtual ICollection<DocumentResource> DocumentResources { get; set; } = new List<DocumentResource>();

    [InverseProperty("Post")]
    public virtual ICollection<DocumentSubPostComment> DocumentSubPostComments { get; set; } = new List<DocumentSubPostComment>();

    [InverseProperty("Target")]
    public virtual ICollection<DocumentSubPostReaction> DocumentSubPostReactions { get; set; } = new List<DocumentSubPostReaction>();

    [ForeignKey("PostId")]
    [InverseProperty("DocumentSubPosts")]
    public virtual DocumentPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("DocumentSubPosts")]
    public virtual User User { get; set; } = null!;
}
