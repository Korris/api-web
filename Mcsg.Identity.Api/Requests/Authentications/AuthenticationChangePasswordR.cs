using System.ComponentModel.DataAnnotations;

namespace Mcsg.Identity.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class AuthenticationChangePasswordR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// OldPassword
    /// </summary>
    [Required]
    public string OldPassword { get; set; } = default!;

    /// <summary>
    /// NewPassword
    /// </summary>
    [Required]
    public string NewPassword { get; set; } = default!;

    /// <summary>
    /// ConfirmPassword
    /// </summary>
    [Required]
    public string ConfirmPassword { get; set; } = default!;

    #endregion
}
