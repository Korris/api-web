namespace Mcsg.Api.Areas.Identity.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class AuthenticationDeleteUserR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// Password
    /// </summary>
    public string? Password { get; set; }

    #endregion
}
