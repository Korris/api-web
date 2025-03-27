namespace Mcsg.Identity.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class AuthenticationChangeRoleR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// TargetUserId
    /// </summary>
    public Guid TargetUserId { get; set; }

    /// <summary>
    /// RoleName
    /// </summary>
    public string? RoleName { get; set; }

    #endregion
}
