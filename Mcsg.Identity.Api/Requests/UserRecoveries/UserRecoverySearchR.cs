namespace Mcsg.Identity.Api;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class UserRecoverySearchR : PagingR
{
    #region -- region --

    /// <summary>
    /// Password
    /// </summary>
    public string? Password { get; set; }

    #endregion
}
