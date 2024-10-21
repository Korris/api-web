using System.Text.Json.Serialization;

namespace Mcsg.Identity.Api.Requests;

/// <summary>
/// Request
/// </summary>
public class AuthenticationRegisterUserR : AuthenticationFormBaseR
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
    /// Referral code
    /// </summary>
    public string? ReferralCode { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Confirm password
    /// </summary>
    public string? ConfirmPassword { get; set; }

    /// <summary>
    /// For admin
    /// </summary>
    [JsonIgnore]
    public bool IsForAdmin { get; private set; }

    #endregion
}
