namespace Mcsg.Api.Areas.Identity.Requests;

using Common.Core.Requests;

/// <summary>
/// FormBase request
/// </summary>
public class AuthenticationFormBaseR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Phone
    /// </summary>
    public string? Phone { get; set; }

    #endregion
}
