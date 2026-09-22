namespace Mcsg.Api.Areas.TapShow.Models;

/// <summary>
/// How the reader leaves a segment
/// </summary>
public enum SegmentKind
{
    /// <summary>
    /// "Next" button → NextSegmentId (next segment by Order; null = end of chapter)
    /// </summary>
    Next,

    /// <summary>
    /// The reader picks one of Choices → TargetSegmentId
    /// </summary>
    Choice,

    /// <summary>
    /// The story stops here
    /// </summary>
    Ending
}

/// <summary>
/// A branch of a segment
/// </summary>
public class SegmentChoiceResponse
{
    public Guid Id { get; set; }
    public string? Label { get; set; }
    public int Order { get; set; }
    public Guid TargetSegmentId { get; set; }
}

/// <summary>
/// One screen of a chapter: image + narration (+ speaker, audio), then Next / Choice / Ending.
/// </summary>
public class SegmentResponse
{
    public Guid Id { get; set; }
    public Guid ChapterId { get; set; }
    public string? Title { get; set; }
    public string? ImageUrl { get; set; }

    /// <summary>
    /// hashId of the attached image (owner only, used to keep the image on update); null otherwise
    /// </summary>
    public string? ImageHashId { get; set; }

    public string? Narration { get; set; }

    /// <summary>
    /// Voice-over audio of the dialogue, optional
    /// </summary>
    public string? AudioUrl { get; set; }

    /// <summary>
    /// hashId of the attached audio (owner only, used to keep the audio on update); null otherwise
    /// </summary>
    public string? AudioHashId { get; set; }

    /// <summary>
    /// Speaking character (null = narrator / none)
    /// </summary>
    public Guid? CharacterId { get; set; }
    public string? CharacterName { get; set; }
    public string? CharacterAvatarUrl { get; set; }

    public float Order { get; set; }

    /// <summary>
    /// Authored ending flag (raw value; Kind is what the reader should use)
    /// </summary>
    public bool IsEnding { get; set; }

    /// <summary>
    /// Choice when Choices is not empty, Ending when IsEnding, otherwise Next
    /// </summary>
    public SegmentKind Kind { get; set; }

    /// <summary>
    /// For Kind = Next: the next segment by Order in the chapter; null when this is the last one (chapter over)
    /// </summary>
    public Guid? NextSegmentId { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public List<SegmentChoiceResponse> Choices { get; set; } = new();
}
