namespace Mcsg.Api.Areas.Social.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class ReportViewR : IdBaseR
{
    #region -- Properties --

    public string? NotificationType { get; set; }

    public string? EntityType { get; set; }

    #endregion
}
