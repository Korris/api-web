using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using SeedWork.Constants;

/// <summary>
/// Character of a TapShow post (name + avatar). Segments can reference the character that speaks the narration.
/// </summary>
public partial class TapShowCharacter : AuditableEntity
{
    /// <summary>
    /// Display name
    /// </summary>
    [StringLength(Validator.Title.Max)]
    public string? Name { get; set; }

    /// <summary>
    /// Public URL of the uploaded avatar (from api/tapshow/file/upload-media), optional
    /// </summary>
    [StringLength(Validator.Url.Max)]
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Display order in the character list
    /// </summary>
    public int Order { get; set; }

    public Guid PostId { get; set; }

    [ForeignKey("PostId")]
    [InverseProperty("TapShowCharacters")]
    public virtual TapShowPost Post { get; set; } = null!;

    [InverseProperty("Character")]
    public virtual ICollection<TapShowSegment> TapShowSegments { get; set; } = new List<TapShowSegment>();

    [InverseProperty("Character")]
    public virtual ICollection<TapShowResource> TapShowResources { get; set; } = new List<TapShowResource>();
}
