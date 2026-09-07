namespace Mcsg.Api.Areas.Comic.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class ReportCreateR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// EntityId
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// EntityType
    /// </summary>
    public string? EntityType { get; set; }

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
