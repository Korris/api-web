namespace Mcsg.Api.Areas.Game.Requests;

using Common.Core.Requests;

/// <summary>
/// Upload request for game thumbnail / game .html file (multipart form)
/// </summary>
public class FileCreateR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// Uploaded file
    /// </summary>
    public IFormFile? File { get; set; }

    #endregion
}
