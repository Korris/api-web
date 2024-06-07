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

namespace Mcsg.Common.SeedWork.Dtos;

/// <summary>
/// API data transfer object
/// </summary>
public class ApiDto
{
    #region -- Properties --

    /// <summary>
    /// Credential
    /// </summary>
    public string Credential { get; set; } = default!;

    /// <summary>
    /// Health
    /// </summary>
    public string Health { get; set; } = default!;

    /// <summary>
    /// Notification
    /// </summary>
    public string Notification { get; set; } = default!;

    /// <summary>
    /// Services
    /// </summary>
    public string Services { get; set; } = default!;

    /// <summary>
    /// Shop
    /// </summary>
    public string Shop { get; set; } = default!;

    /// <summary>
    /// Social
    /// </summary>
    public string Social { get; set; } = default!;

    #endregion
}
