using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using SeedWork.Constants;

/// <summary>
/// One screen of a TapShow chapter: an image + narration text, then a set of choices leading to other segments.
/// A segment with no outgoing choices is an ending.
/// </summary>
public partial class TapShowSegment : AuditableEntity
{
    /// <summary>
    /// Authoring label (optional, e.g. "Good ending")
    /// </summary>
    [StringLength(Validator.Title.Max)]
    public string? Title { get; set; }

    /// <summary>
    /// Public URL of the uploaded image (from api/tapshow/file/upload-media), optional
    /// </summary>
    [StringLength(Validator.Url.Max)]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Dialogue / narration text shown with the image
    /// </summary>
    public string? Narration { get; set; }

    /// <summary>
    /// Public URL of the uploaded voice-over audio of the dialogue (from api/tapshow/file/upload-audio), optional
    /// </summary>
    [StringLength(Validator.Url.Max)]
    public string? AudioUrl { get; set; }

    /// <summary>
    /// Authoring order inside the chapter; the lowest one is the chapter entry point.
    /// A segment without choices continues to the next one by Order ("Next").
    /// </summary>
    public float Order { get; set; }

    /// <summary>
    /// Marks an ending: the story stops here (no Next, no choices). Several endings per chapter are expected.
    /// </summary>
    public bool IsEnding { get; set; }

    public Guid ChapterId { get; set; }

    /// <summary>
    /// Character speaking this segment (must belong to the same post), optional
    /// </summary>
    public Guid? CharacterId { get; set; }

    [ForeignKey("ChapterId")]
    [InverseProperty("TapShowSegments")]
    public virtual TapShowChapter Chapter { get; set; } = null!;

    [ForeignKey("CharacterId")]
    [InverseProperty("TapShowSegments")]
    public virtual TapShowCharacter? Character { get; set; }

    /// <summary>
    /// Outgoing choices (branches from this segment)
    /// </summary>
    [InverseProperty("Segment")]
    public virtual ICollection<TapShowSegmentChoice> Choices { get; set; } = new List<TapShowSegmentChoice>();

    /// <summary>
    /// Choices of other segments that lead here
    /// </summary>
    [InverseProperty("TargetSegment")]
    public virtual ICollection<TapShowSegmentChoice> IncomingChoices { get; set; } = new List<TapShowSegmentChoice>();

    [InverseProperty("Segment")]
    public virtual ICollection<TapShowResource> TapShowResources { get; set; } = new List<TapShowResource>();
}
