using System.Text.Json.Serialization;

namespace Mcsg.Api.Areas.Identity.Requests;

/// <summary>
/// Request
/// </summary>
public class AuthenticationLoginUserR : AuthenticationFormBaseR
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
    /// Password
    /// </summary>
    public string Password { get; set; } = default!;

    /// <summary>
    /// For admin
    /// </summary>
    [JsonIgnore]
    public bool IsForAdmin { get; private set; }

    /// <summary>
    /// OtpCode
    /// </summary>
    public string? OtpCode { get; set; }

    /// <summary>
    /// IsUseRecovery
    /// </summary>
    public bool? IsRecoveryMode { get; set; }

    /// <summary>
    /// DeviceToken
    /// </summary>
    public string? DeviceToken { get; set; }

    #endregion
}
