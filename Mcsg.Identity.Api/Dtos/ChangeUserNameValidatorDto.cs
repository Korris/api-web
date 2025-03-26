namespace Mcsg.Identity.Api.Dtos;

/// <summary>
/// ChangeUserNameValidatorDto
/// </summary>
public class ChangeUserNameValidatorDto
{
    #region -- Properties --

    /// <summary>
    /// CanUpdateUserName
    /// </summary>
    public bool CanUpdateUserName { get; set; }

    /// <summary>
    /// UpdatedUserName
    /// </summary>
    public bool UpdatedUserName { get; set; }

    /// <summary>
    /// TimeRemaining
    /// </summary>
    public string? TimeRemaining { get; set; }

    /// <summary>
    /// TimeWaiting
    /// </summary>
    public DateTime TimeWaiting { get; set; }

    /// <summary>
    /// ModifiedCount
    /// </summary>
    public int ModifiedCount { get; set; }

    /// <summary>
    /// UserNameWaitingChangedAfter
    /// </summary>
    public double UserNameWaitingChangedAfter { get; set; }

    /// <summary>
    /// UserNameChangedInRemaining
    /// </summary>
    public double UserNameChangedInRemaining { get; set; }

    /// <summary>
    /// TimePassed
    /// </summary>
    public TimeSpan TimePassed { get; set; }

    /// <summary>
    /// HasPremium
    /// </summary>
    public bool HasPremium { get; set; }

    #endregion
}
