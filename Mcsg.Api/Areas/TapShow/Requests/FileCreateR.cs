namespace Mcsg.Api.Areas.TapShow.Requests;

using Common.Core.Requests;

/// <summary>
/// Upload request for a TapShow image (multipart form)
/// </summary>
public class FileCreateR : BaseR
{
    /// <summary>
    /// Uploaded file
    /// </summary>
    public IFormFile? File { get; set; }
}
