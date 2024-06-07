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
/// VerificationSecret data transfer object
/// </summary>
public class VerificationSecretDto
{
    #region -- Properties --

    /// <summary>
    /// Gets or sets the AppId
    /// </summary>
    /// <value>
    /// The AppId
    /// </value>
    public string AppId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the secret
    /// </summary>
    /// <value>
    /// The secret
    /// </value>
    public string Secret { get; set; } = default!;

    /// <summary>
    /// Gets or sets the API URL
    /// </summary>
    /// <value>
    /// The API URL
    /// </value>
    public string ApiUrl { get; set; } = default!;

    /// <summary>
    /// Gets or sets the redirect URI
    /// </summary>
    /// <value>
    /// The redirect URI
    /// </value>
    public string RedirectUri { get; set; } = default!;

    #endregion
}
