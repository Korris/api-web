namespace Mcsg.Identity.Api.Requests;

/// <summary>
/// Request
/// </summary>
public class AuthenticationResetPasswordR : AuthenticationResendOtpR
{
    #region -- Properties --

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; } = default!;

    /// <summary>
    /// ConfirmPassword
    /// </summary>
    public string ConfirmPassword { get; set; } = default!;

    /// <summary>
    /// OtpCode
    /// </summary>
    public string OtpCode { get; set; } = default!;

    /// <summary>
    /// RefreshToken
    /// </summary>
    public string? RefreshToken { get; set; }

    #endregion
}
