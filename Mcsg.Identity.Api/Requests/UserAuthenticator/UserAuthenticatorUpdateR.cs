namespace Mcsg.Identity.Api;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class UserAuthenticatorUpdateR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// OptCode
    /// </summary>
    public string? OtpCode { get; set; } = default!;

    /// <summary>
    /// IsLogin
    /// </summary>
    public bool? IsLogin { get; set; }

    /// <summary>
    /// IsTransaction
    /// </summary>
    public bool? IsTransaction { get; set; }

    #endregion
}
