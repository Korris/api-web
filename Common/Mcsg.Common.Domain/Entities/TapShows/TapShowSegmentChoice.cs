using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using SeedWork.Constants;

/// <summary>
/// A branch: from Segment, the reader picks Label and continues at TargetSegment (same chapter).
/// </summary>
public partial class TapShowSegmentChoice : AuditableEntity
{
    /// <summary>
    /// Text shown on the choice button
    /// </summary>
    [StringLength(Validator.Title.Max)]
    public string? Label { get; set; }

    /// <summary>
    /// Display order among the choices of the same segment
    /// </summary>
    public int Order { get; set; }

    public Guid SegmentId { get; set; }
    public Guid TargetSegmentId { get; set; }

    [ForeignKey("SegmentId")]
    [InverseProperty("Choices")]
    public virtual TapShowSegment Segment { get; set; } = null!;

    [ForeignKey("TargetSegmentId")]
    [InverseProperty("IncomingChoices")]
    public virtual TapShowSegment TargetSegment { get; set; } = null!;
}
