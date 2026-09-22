namespace Mcsg.Common.Domain.Entities;

/// <summary>
/// TapShowSegment / TapShowSegmentChoice domain helpers
/// </summary>
partial class TapShowSegment
{
    #region -- Methods --

    /// <summary>
    /// Constructor
    /// </summary>
    public TapShowSegment()
    {
    }

    /// <summary>
    /// Create a segment inside a chapter
    /// </summary>
    public static TapShowSegment Create(Guid chapterId, Guid? characterId, string? title, string? imageUrl, string? narration, string? audioUrl, float order, bool isEnding, Guid createdBy)
    {
        return new TapShowSegment
        {
            ChapterId = chapterId,
            CharacterId = characterId,
            Title = title,
            ImageUrl = imageUrl,
            Narration = narration,
            AudioUrl = audioUrl,
            Order = order,
            IsEnding = isEnding,
            CreatedBy = createdBy
        };
    }

    /// <summary>
    /// Update editable fields
    /// </summary>
    public void Update(Guid? characterId, string? title, string? imageUrl, string? narration, string? audioUrl, float order, bool isEnding, Guid modifiedBy)
    {
        CharacterId = characterId;
        Title = title;
        ImageUrl = imageUrl;
        Narration = narration;
        AudioUrl = audioUrl;
        Order = order;
        IsEnding = isEnding;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft delete
    /// </summary>
    public void Delete(Guid modifiedBy)
    {
        IsDelete = true;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    #endregion
}

partial class TapShowSegmentChoice
{
    #region -- Methods --

    /// <summary>
    /// Constructor
    /// </summary>
    public TapShowSegmentChoice()
    {
    }

    /// <summary>
    /// Create a branch from one segment to another
    /// </summary>
    public static TapShowSegmentChoice Create(Guid segmentId, Guid targetSegmentId, string? label, int order, Guid createdBy)
    {
        return new TapShowSegmentChoice
        {
            SegmentId = segmentId,
            TargetSegmentId = targetSegmentId,
            Label = label,
            Order = order,
            CreatedBy = createdBy
        };
    }

    /// <summary>
    /// Soft delete
    /// </summary>
    public void Delete(Guid modifiedBy)
    {
        IsDelete = true;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    #endregion
}
