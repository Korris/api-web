namespace Mcsg.Api.Areas.Identity.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class FeedbackCreateR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// FeedbackType
    /// </summary>
    public string? FeedbackType { get; set; }

    /// <summary>
    /// ReasonText
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// File
    /// </summary>
    public List<IFormFile>? Files { get; set; }

    #endregion
}