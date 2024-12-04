namespace Mcsg.Realtime.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class AuthenticationResetPasswordR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Phone
    /// </summary>
    public string? Phone { get; set; }

    #endregion
}
