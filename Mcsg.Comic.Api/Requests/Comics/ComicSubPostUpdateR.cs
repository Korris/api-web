namespace Mcsg.Comic.Api.Requests;

/// <summary>
/// Request
/// </summary>
public class ComicSubPostUpdateR : ComicSubPostFormBaseR
{
    #region -- Properties --

    /// <summary>
    /// Chapter order
    /// </summary>
    public float ChapterOrder { get; set; }

    #endregion
}
