namespace Mcsg.Identity.Api.Requests;

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
    public string Email { get; set; }

    /// <summary>
    /// SatisfactionLevel
    /// </summary>
    public string? SatisfactionLevel { get; set; }

    /// <summary>
    /// ReasonText
    /// </summary>
    public string? ReasonText { get; set; }

    /// <summary>
    /// PostType
    /// </summary>
    public string? PostType { get; set; }

    #endregion
}