namespace Mcsg.Common.Core.Dtos;

/// <summary>
/// ChapterView data transfer object
/// </summary>
public class ChapterViewDto
{
    #region -- Properties --

    /// <summary>
    /// ChapterId
    /// </summary>
    public Guid ChapterId { get; set; }

    /// <summary>
    /// Views
    /// </summary>
    public int Views { get; set; }

    #endregion
}
