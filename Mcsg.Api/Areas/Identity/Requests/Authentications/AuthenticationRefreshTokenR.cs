namespace Mcsg.Api.Areas.Identity.Requests;

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
    public string? RefreshToken { get; set; }

    #endregion
}
