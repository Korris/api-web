namespace Mcsg.Identity.Api.Requests;

/// <summary>
/// Request
/// </summary>
public class AuthenticationLoginUserR : AuthenticationFormBaseR
{
    #region -- Properties --

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; } = default!;

    #endregion
}
