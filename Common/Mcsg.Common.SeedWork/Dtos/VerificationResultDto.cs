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
/// VerificationResult data transfer object
/// </summary>
public class VerificationResultDto
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public VerificationResultDto()
    {
        LoginProvider = string.Empty;
        ProviderKey = string.Empty;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Login provider
    /// </summary>
    public string LoginProvider { get; set; }

    /// <summary>
    /// Provider key
    /// </summary>
    public string ProviderKey { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// First name
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name
    /// </summary>
    public string? LastName { get; set; }

    #endregion
}
