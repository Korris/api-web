using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialResource : BaseResource
{
    [ForeignKey("AuthorId")]
    [InverseProperty("SocialResources")]
    public virtual User? Author { get; set; }

    [InverseProperty("Resource")]
    public virtual ICollection<SocialPostComment> SocialPostComments { get; set; } = new List<SocialPostComment>();

    [InverseProperty("Resource")]
    public virtual ICollection<SocialSubPostComment> SocialSubPostComments { get; set; } = new List<SocialSubPostComment>();

    [ForeignKey("SubPostId")]
    [InverseProperty("SocialResources")]
    public virtual SocialSubPost? SubPost { get; set; }
}
