using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StoryPostLink : BasePostLink
{
    [ForeignKey("PostId")]
    [InverseProperty("StoryPostLinks")]
    public virtual StoryPost Post { get; set; } = null!;
}
