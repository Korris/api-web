namespace Mcsg.Common.Core.Dtos;

using Common.Core.Enums;

/// <summary>
/// Affiliate data transfer object
/// </summary>
public class AffiliateDto
{
    #region -- Properties --

    /// <summary>
    /// AffiliateUserId
    /// </summary>
    public Guid AffiliateUserId { get; set; }

    /// <summary>
    /// EntityHashId
    /// </summary>
    public string EntityHashId { get; set; } = default!;

    /// <summary>
    /// EntityType
    /// </summary>
    public AffiliateEntityType EntityType { get; set; }

    #endregion
}
