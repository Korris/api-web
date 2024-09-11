namespace Mcsg.Identity.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class RatingCreateR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// SatisfactionLevel
    /// </summary>
    public string? Satisfaction { get; set; }

    /// <summary>
    /// ReasonText
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    public string? Type { get; set; }

    #endregion
}