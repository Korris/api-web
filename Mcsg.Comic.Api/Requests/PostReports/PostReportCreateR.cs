namespace Mcsg.Comic.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class PostReportCreateR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// PostId
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// ReasonType
    /// </summary>
    public string? ReasonType { get; set; }

    /// <summary>
    /// ReasonText
    /// </summary>
    public string? ReasonText { get; set; }

    #endregion
}