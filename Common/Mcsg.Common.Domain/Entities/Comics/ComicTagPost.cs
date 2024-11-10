using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicTagPost : BaseTagPost
{
    [ForeignKey("PostId")]
    [InverseProperty("ComicTagPosts")]
    public virtual ComicPost Post { get; set; } = null!;

    [ForeignKey("TagId")]
    [InverseProperty("ComicTagPosts")]
    public virtual Tag Tag { get; set; } = null!;
}
