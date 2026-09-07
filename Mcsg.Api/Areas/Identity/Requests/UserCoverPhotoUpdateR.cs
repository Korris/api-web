namespace Mcsg.Api.Areas.Identity.Requests;

using Common.Core.Requests;
using static Common.Core.Constants.Setting;

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
    public string? Type { get; set; } = PostResourceType.Cover;

    #endregion
}
