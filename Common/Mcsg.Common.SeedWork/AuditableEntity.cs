using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.SeedWork;

using SeedWork.Constants;

/// <summary>
/// Auditable entity
/// </summary>
public class AuditableEntity : TrackingEntity
{
    #region -- Properties --

    /// <summary>
    /// Modified by
    /// </summary>
    public Guid? ModifiedBy { get; set; }

    /// <summary>
    /// Modified date
    /// </summary>
    [Column(TypeName = "timestamp")]
    public DateTime? ModifiedOn { get; set; }

    /// <summary>
    /// Is delete
    /// </summary>
    public bool IsDelete { get; set; }

    /// <summary>
    /// Synced on
    /// </summary>
    [Column(TypeName = "timestamp")]
    public DateTime? SyncedOn { get; set; }

    /// <summary>
    /// Sync error
    /// </summary>
    [StringLength(Validator.Description.Max)]
    public string? SyncError { get; set; }

    /// <summary>
    /// Example: ;video;link;11;1;
    /// </summary>
    [StringLength(Validator.TagData.Max)]
    public string? TagData { get; set; }

    #endregion
}
