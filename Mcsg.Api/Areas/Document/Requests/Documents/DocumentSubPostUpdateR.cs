namespace Mcsg.Api.Areas.Document.Requests;

/// <summary>
/// Request
/// </summary>
public class DocumentSubPostUpdateR : DocumentSubPostFormBaseR
{
    #region -- Properties --

    /// <summary>
    /// Chapter order
    /// </summary>
    public float ChapterOrder { get; set; }

    #endregion
}
