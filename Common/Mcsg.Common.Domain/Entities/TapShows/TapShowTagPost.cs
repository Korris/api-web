using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

/// <summary>
/// Hashtag ↔ TapShow post link (tapshow."TapShowTagPosts"), same shape as ComicTagPost. Soft-deleted when the tag is removed from the post.
/// </summary>
public partial class TapShowTagPost : BaseTagPost
{
    [ForeignKey("PostId")]
    [InverseProperty("TapShowTagPosts")]
    public virtual TapShowPost Post { get; set; } = null!;

    [ForeignKey("TagId")]
    [InverseProperty("TapShowTagPosts")]
    public virtual Tag Tag { get; set; } = null!;
}
