using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialSubPost : BaseSubPost
{
    public int Order { get; set; }

    [InverseProperty("SubPost")]
    public virtual ICollection<SocialResource> SocialResources { get; set; } = new List<SocialResource>();

    [InverseProperty("Post")]
    public virtual ICollection<SocialSubPostComment> SocialSubPostComments { get; set; } = new List<SocialSubPostComment>();

    [InverseProperty("Target")]
    public virtual ICollection<SocialSubPostReaction> SocialSubPostReactions { get; set; } = new List<SocialSubPostReaction>();

    [ForeignKey("PostId")]
    [InverseProperty("SocialSubPosts")]
    public virtual SocialPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("SocialSubPosts")]
    public virtual User User { get; set; } = null!;

    [InverseProperty("SubPost")]
    public virtual ICollection<UserExclusiveSubPost> UserExclusiveSubPosts { get; set; } = new List<UserExclusiveSubPost>();
}
