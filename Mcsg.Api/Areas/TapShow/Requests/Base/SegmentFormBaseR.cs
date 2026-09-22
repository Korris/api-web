namespace Mcsg.Api.Areas.TapShow.Requests;

using Common.Core.Requests;

/// <summary>
/// Shared fields for create / update segment (image + narration)
/// </summary>
public class SegmentFormBaseR : IdBaseR
{
    #region -- Properties --

    /// <summary>
    /// Authoring label (optional)
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Character speaking this segment (from api/tapshow/character of the same post), optional
    /// </summary>
    public Guid? CharacterId { get; set; }

    /// <summary>
    /// hashId returned by POST api/tapshow/file/upload-media; null → no image.
    /// On update: same hashId keeps the current image, a new one replaces it, null removes it.
    /// </summary>
    public string? ImageHashId { get; set; }

    /// <summary>
    /// Dialogue / narration text
    /// </summary>
    public string? Narration { get; set; }

    /// <summary>
    /// hashId returned by POST api/tapshow/file/upload-audio (voice-over of the dialogue); null → no audio.
    /// On update: same hashId keeps the current audio, a new one replaces it, null removes it.
    /// </summary>
    public string? AudioHashId { get; set; }

    /// <summary>
    /// Position among the segments of the chapter (lowest = entry point); null on create → appended at the end
    /// </summary>
    public float? Order { get; set; }

    /// <summary>
    /// True = the story stops here (an ending). Cannot be combined with choices.
    /// </summary>
    public bool IsEnding { get; set; }

    #endregion
}
