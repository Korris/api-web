#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 07:03
 * Update       : 2024-Jan-21 07:03
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

namespace Mcsg.Common.Core.Dtos;

/// <summary>
/// API data transfer object
/// </summary>
public class ApiDto
{
    #region -- Properties --

    /// <summary>
    /// Admin
    /// </summary>
    public string Admin { get; set; } = default!;

    /// <summary>
    /// Analytic
    /// </summary>
    public string Analytic { get; set; } = default!;

    /// <summary>
    /// Identity
    /// </summary>
    public string Identity { get; set; } = default!;

    /// <summary>
    /// Media
    /// </summary>
    public string Media { get; set; } = default!;

    /// <summary>
    /// Realtime
    /// </summary>
    public string Realtime { get; set; } = default!;

    /// <summary>
    /// Social
    /// </summary>
    public string Social { get; set; } = default!;

    /// <summary>
    /// Wallet
    /// </summary>
    public string Wallet { get; set; } = default!;

    #endregion
}
