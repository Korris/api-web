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
    /// RetypePassword
    /// </summary>
    public string RetypePassword { get; set; } = default!;

    /// <summary>
    /// OtpCode
    /// </summary>
    public string OtpCode { get; set; } = default!;

    #endregion
}
