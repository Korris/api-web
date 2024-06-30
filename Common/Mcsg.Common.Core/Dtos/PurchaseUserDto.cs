namespace Mcsg.Common.Core.Dtos;

/// <summary>
/// PurchaseUser data transfer object
/// </summary>
public class PurchaseUserDto
{
    #region -- Properties --

    /// <summary>
    /// UserId
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Amount
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// Chapter quantity
    /// </summary>
    public int ChapterQuantity { get; set; }

    #endregion
}
