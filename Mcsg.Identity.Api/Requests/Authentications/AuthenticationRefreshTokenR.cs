using System.ComponentModel.DataAnnotations;

namespace Mcsg.Identity.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class AuthenticationRefreshTokenR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// RefreshToken
    /// </summary>
    [Required]
    public string RefreshToken { get; set; } = default!;

    #endregion
}
