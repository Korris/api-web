namespace Mcsg.Identity.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class UserCoverPhotoUpdateR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// CoverPhoto
    /// </summary>
    public IFormFile? CoverPhoto { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    public string? Type { get; set; } = "Cover";

    #endregion
}
