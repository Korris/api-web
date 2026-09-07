namespace Mcsg.Api.Areas.Identity.Requests;

using Common.Core.Enums;

/// <summary>
/// Request
/// </summary>
public class AuthenticationResendOtpR : AuthenticationFormBaseR
{
    #region -- Properties --

    /// <summary>
    /// Type
    /// </summary>
    public UserOtpType Type { get; set; }

    /// <summary>
    /// OtpToken
    /// </summary>
    public string OtpToken { get; set; } = default!;

    #endregion
}
