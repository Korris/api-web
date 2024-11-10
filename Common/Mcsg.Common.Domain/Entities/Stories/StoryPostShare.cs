using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StoryPostShare : BasePostShare
{
    [ForeignKey("PostId")]
    [InverseProperty("StoryPostShares")]
    public virtual StoryPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("StoryPostShares")]
    public virtual User User { get; set; } = null!;
}
