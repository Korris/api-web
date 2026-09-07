using System.ComponentModel.DataAnnotations;

namespace Mcsg.Api.Areas.Identity.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class AuthenticationVerifyOtpR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// OTP Code
    /// </summary>
    [Required]
    public string OtpCode { get; set; } = default!;

    /// <summary>
    /// IsUseRecovery
    /// </summary>
    public bool? IsRecoveryMode { get; set; }

    #endregion
}
