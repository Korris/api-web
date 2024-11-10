using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StoryTagPost : BaseTagPost
{
    [ForeignKey("PostId")]
    [InverseProperty("StoryTagPosts")]
    public virtual StoryPost Post { get; set; } = null!;

    [ForeignKey("TagId")]
    [InverseProperty("StoryTagPosts")]
    public virtual Tag Tag { get; set; } = null!;
}
