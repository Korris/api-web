using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

/// <summary>
/// Uploaded image for the TapShow area (post thumbnail, chapter thumbnail, segment image, character avatar), copy of GameResource.
/// Created as a temp row (IsDelete = true) by the upload API, attached to a post / segment / character on use.
/// SubPostId from BaseResource = chapter id for a chapter thumbnail (no FK, looked up by value).
/// </summary>
public partial class TapShowResource : BaseResource
{
    /// <summary>
    /// Post that uses this file (thumbnail or segment image); null while not attached
    /// </summary>
    public Guid? PostId { get; set; }

    /// <summary>
    /// Segment that uses this file as its image; null for thumbnails and unattached uploads
    /// </summary>
    public Guid? SegmentId { get; set; }

    /// <summary>
    /// Character that uses this file as its avatar; null otherwise
    /// </summary>
    public Guid? CharacterId { get; set; }

    [ForeignKey("AuthorId")]
    [InverseProperty("TapShowResources")]
    public virtual User? Author { get; set; }

    [ForeignKey("PostId")]
    [InverseProperty("TapShowResources")]
    public virtual TapShowPost? Post { get; set; }

    [ForeignKey("SegmentId")]
    [InverseProperty("TapShowResources")]
    public virtual TapShowSegment? Segment { get; set; }

    [ForeignKey("CharacterId")]
    [InverseProperty("TapShowResources")]
    public virtual TapShowCharacter? Character { get; set; }
}
