using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

/// <summary>
/// Hashtag ↔ game post link (game."GameTagPosts"), same shape as ComicTagPost. Soft-deleted when the tag is removed from the post.
/// </summary>
public partial class GameTagPost : BaseTagPost
{
    [ForeignKey("PostId")]
    [InverseProperty("GameTagPosts")]
    public virtual GamePost Post { get; set; } = null!;

    [ForeignKey("TagId")]
    [InverseProperty("GameTagPosts")]
    public virtual Tag Tag { get; set; } = null!;
}
