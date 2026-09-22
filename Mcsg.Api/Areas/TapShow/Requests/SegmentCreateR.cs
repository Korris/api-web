namespace Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// Create a segment inside a chapter
/// </summary>
public class SegmentCreateR : SegmentFormBaseR
{
    /// <summary>
    /// HashId of the chapter that owns the segment
    /// </summary>
    public string? ChapterHashId { get; set; }
}
