namespace Mcsg.Identity.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class AuthenticationLoginSocialR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// SocialType
    /// </summary>
    public string SocialType { get; set; } = default!;

    /// <summary>
    /// SocialToken
    /// </summary>
    public string SocialToken { get; set; } = default!;

    /// <summary>
    /// OtpCode
    /// </summary>
    public string? OtpCode { get; set; }

    #endregion
}
