namespace Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// Create a chapter under a TapShow post
/// </summary>
public class ChapterCreateR : ChapterFormBaseR
{
    /// <summary>
    /// HashId of the post that owns the chapter
    /// </summary>
    public string? PostHashId { get; set; }
}
