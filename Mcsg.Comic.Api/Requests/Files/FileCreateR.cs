namespace Mcsg.Comic.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class FileCreateR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// File
    /// </summary>
    public IFormFile? File { get; set; }

    /// <summary>
    /// Upload to public bucket
    /// </summary>
    public bool? IsPublic { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    public string? Type { get; set; }

    #endregion
}
