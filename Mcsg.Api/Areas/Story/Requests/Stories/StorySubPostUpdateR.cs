namespace Mcsg.Api.Areas.Story.Requests;

/// <summary>
/// Request
/// </summary>
public class StorySubPostUpdateR : StorySubPostFormBaseR
{
    #region -- Properties --

    /// <summary>
    /// Chapter order
    /// </summary>
    public float ChapterOrder { get; set; }

    #endregion
}
