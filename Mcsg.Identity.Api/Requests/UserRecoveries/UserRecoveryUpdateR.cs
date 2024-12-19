namespace Mcsg.Identity.Api;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class UserRecoveryUpdateR : BaseR
{
    #region -- region --

    /// <summary>
    /// Password
    /// </summary>
    public string? Password { get; set; }

    #endregion
}
