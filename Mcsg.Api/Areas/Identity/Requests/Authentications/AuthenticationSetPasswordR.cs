namespace Mcsg.Api.Areas.Identity.Requests;

/// <summary>
/// Request
/// </summary>
public class AuthenticationSetPasswordR : AuthenticationFormBaseR
{
    #region -- Properties --

    /// <summary>
    /// Otp
    /// </summary>
    public string Otp { get; set; } = default!;

    /// <summary>
    /// OtpToken
    /// </summary>
    public string OtpToken { get; set; } = default!;

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; } = default!;

    /// <summary>
    /// ConfirmPassword
    /// </summary>
    public string ConfirmPassword { get; set; } = default!;

    #endregion
}
