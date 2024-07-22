namespace Mcsg.Common.SeedWork;

/// <summary>
/// Tracking entity
/// </summary>
public class TrackingEntity : EntityId
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public TrackingEntity()
    {
        CreatedOn = DateTime.UtcNow;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Created by
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Created on
    /// </summary>
    public DateTime CreatedOn { get; set; }

    #endregion
}
