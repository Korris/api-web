namespace Mcsg.Identity.Api.Requests;

/// <summary>
/// Request
/// </summary>
public class AuthenticationLogoutR : AuthenticationRefreshTokenR
{
    #region -- Properties --

    /// <summary>
    /// DeviceToken
    /// </summary>
    public string? DeviceToken { get; set; }

    #endregion
}
