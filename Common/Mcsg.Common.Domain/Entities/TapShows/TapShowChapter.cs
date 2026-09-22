using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

/// <summary>
/// Chapter of a TapShow post (copy of ComicSubPost without premium / publish notification).
/// Holds the segment graph: the segment with the lowest Order is the entry point.
/// </summary>
public partial class TapShowChapter : BaseSubPost
{
    [ForeignKey("PostId")]
    [InverseProperty("TapShowChapters")]
    public virtual TapShowPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("TapShowChapters")]
    public virtual User User { get; set; } = null!;

    [InverseProperty("Chapter")]
    public virtual ICollection<TapShowSegment> TapShowSegments { get; set; } = new List<TapShowSegment>();
}
