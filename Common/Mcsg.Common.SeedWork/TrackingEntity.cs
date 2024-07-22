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
        CreatedDate = DateTime.UtcNow;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Created date
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Created by
    /// </summary>
    public Guid? CreatedBy { get; set; }

    #endregion
}
