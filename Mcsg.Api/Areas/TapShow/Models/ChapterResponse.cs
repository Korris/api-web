namespace Mcsg.Api.Areas.TapShow.Models;

using Common.Core.Enums;

/// <summary>
/// Chapter summary (list items, create / update result)
/// </summary>
public class ChapterResponse
{
    public Guid Id { get; set; }
    public string HashId { get; set; } = default!;
    public Guid PostId { get; set; }
    public string? PostHashId { get; set; }
    public string? Title { get; set; }
    public float Order { get; set; }
    public PostStatus Status { get; set; }
    public DateTime? PublishDate { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }

    /// <summary>
    /// Number of segments (screens) in the chapter
    /// </summary>
    public int SegmentCount { get; set; }

    /// <summary>
    /// Number of segments flagged IsEnding
    /// </summary>
    public int EndingCount { get; set; }
}

/// <summary>
/// Chapter with its full segment graph, what the reader / editor loads to play or edit
/// </summary>
public class ChapterDetailResponse : ChapterResponse
{
    /// <summary>
    /// True when the post is Premium and the viewer is neither premium nor the owner: Segments is then empty
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Entry point (segment with the lowest Order), null when the chapter has no segment
    /// </summary>
    public Guid? StartSegmentId { get; set; }

    public List<SegmentResponse> Segments { get; set; } = new();
}
