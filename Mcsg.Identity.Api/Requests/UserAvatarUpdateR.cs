namespace Mcsg.Identity.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class UserAvatarUpdateR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// Avatar
    /// </summary>
    public IFormFile? Avatar { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    public string? Type { get; set; } = "Avatar";

    #endregion
}
