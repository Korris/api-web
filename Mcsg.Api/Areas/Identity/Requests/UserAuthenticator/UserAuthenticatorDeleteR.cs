namespace Mcsg.Api.Areas.Identity.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class UserAuthenticatorDeleteR : BaseR
{
    /// <summary>
    /// OptCode
    /// </summary>
    public string OtpCode { get; set; } = default!;
}
