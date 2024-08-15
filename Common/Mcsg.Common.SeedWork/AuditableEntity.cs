using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.SeedWork;

/// <summary>
/// Auditable entity
/// </summary>
public class AuditableEntity : TrackingEntity
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public AuditableEntity()
    {
        ModifiedOn = DateTime.UtcNow;
    }

    #endregion

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

    #endregion
}
