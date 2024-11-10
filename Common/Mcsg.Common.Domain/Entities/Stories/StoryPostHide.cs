using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StoryPostHide : BasePostHide
{
    [ForeignKey("PostId")]
    [InverseProperty("StoryPostHides")]
    public virtual StoryPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("StoryPostHides")]
    public virtual User User { get; set; } = null!;
}
