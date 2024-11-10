using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StoryResource : BaseResource
{
    [ForeignKey("AuthorId")]
    [InverseProperty("StoryResources")]
    public virtual User? Author { get; set; }

    [InverseProperty("Resource")]
    public virtual ICollection<StoryPostComment> StoryPostComments { get; set; } = new List<StoryPostComment>();

    [InverseProperty("Resource")]
    public virtual ICollection<StorySubPostComment> StorySubPostComments { get; set; } = new List<StorySubPostComment>();

    [ForeignKey("SubPostId")]
    [InverseProperty("StoryResources")]
    public virtual StorySubPost? SubPost { get; set; }
}
