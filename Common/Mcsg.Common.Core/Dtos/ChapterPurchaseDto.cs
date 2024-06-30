namespace Mcsg.Common.Core.Dtos;

/// <summary>
/// ChapterPurchase data transfer object
/// </summary>
public class ChapterPurchaseDto
{
    #region -- Properties --

    /// <summary>
    /// ChapterId
    /// </summary>
    public Guid ChapterId { get; set; }

    /// <summary>
    /// Purchases
    /// </summary>
    public int Purchases { get; set; }

    #endregion
}
