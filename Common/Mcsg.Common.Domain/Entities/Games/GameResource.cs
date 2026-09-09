using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

/// <summary>
/// Uploaded file for the game area (thumbnail image or game .html), copy of ComicResource.
/// Created as a temp row (IsDelete = true) by the upload API, attached to a post (PostId, IsDelete = false) on create.
/// SubPostId from BaseResource is unused (games have no chapters).
/// </summary>
public partial class GameResource : BaseResource
{
    /// <summary>
    /// Post that uses this file; null while the upload is not attached to any post yet
    /// </summary>
    public Guid? PostId { get; set; }

    [ForeignKey("AuthorId")]
    [InverseProperty("GameResources")]
    public virtual User? Author { get; set; }

    [ForeignKey("PostId")]
    [InverseProperty("GameResources")]
    public virtual GamePost? Post { get; set; }
}
