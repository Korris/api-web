using System.Text.Json.Serialization;

namespace Mcsg.Identity.Api.Requests;

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

    #endregion
}
