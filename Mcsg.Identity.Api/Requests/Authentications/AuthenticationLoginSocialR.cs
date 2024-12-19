using System.Text.Json.Serialization;

namespace Mcsg.Identity.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class AuthenticationLoginSocialR : BaseR
{
    #region -- Methods --

    /// <summary>
    /// Sets the IsForAdmin flag.
    /// </summary>
    /// <param name="isForAdmin">A boolean value indicating whether the action is for admin.</param>
    public void SetForAdmin(bool isForAdmin)
    {
        IsForAdmin = isForAdmin;
    }

    #endregion

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

    /// <summary>
    /// IsUseRecovery
    /// </summary>
    public bool? IsRecoveryMode { get; set; }

    /// <summary>
    /// For admin
    /// </summary>
    [JsonIgnore]
    public bool IsForAdmin { get; private set; }

    #endregion
}
