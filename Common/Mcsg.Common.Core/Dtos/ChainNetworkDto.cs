namespace Mcsg.Common.Core.Dtos;

using Enums;

/// <summary>
/// ChainNetwork data transfer object
/// </summary>
public class ChainNetworkDto
{
    #region -- Properties --

    /// <summary>
    /// Symbol
    /// </summary>
    public string CurrencyUnit { get; set; } = default!;

    /// <summary>
    /// ProviderLink
    /// </summary>
    public string ProviderLink { get; set; } = default!;

    /// <summary>
    /// ChainType
    /// </summary>
    public ChainType ChainType { get; set; }

    /// <summary>
    /// ContractAddress (Smart Contract)
    /// </summary>
    public string? ContractAddress { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Network
    /// </summary>
    public string Network { get; set; } = default!;

    #endregion
}
