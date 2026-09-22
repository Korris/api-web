namespace Mcsg.Api.Areas.TapShow.Requests;

using Common.Core.Requests;

/// <summary>
/// One branch of a segment
/// </summary>
public class SegmentChoiceItemR
{
    /// <summary>
    /// Text on the choice button
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Segment (same chapter) the reader continues at
    /// </summary>
    public Guid TargetSegmentId { get; set; }
}

/// <summary>
/// Replace ALL outgoing choices of a segment (Id from the route). Empty list = the segment becomes an ending.
/// Order = position in the list.
/// </summary>
public class SegmentChoicesSetR : IdBaseR
{
    public List<SegmentChoiceItemR> Choices { get; set; } = new();
}
