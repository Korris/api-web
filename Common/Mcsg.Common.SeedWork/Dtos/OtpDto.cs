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
/// OTP data transfer object
/// </summary>
public class OtpDto
{
    #region -- Properties --

    /// <summary>
    /// ExpiryInMinutes
    /// </summary>
    public int ExpiryInMinutes { get; set; }

    /// <summary>
    /// OtpLength
    /// </summary>
    public int OtpLength { get; set; }

    /// <summary>
    /// OtpTokenLength
    /// </summary>
    public int OtpTokenLength { get; set; }

    /// <summary>
    /// OtpExpired
    /// </summary>
    public int OtpExpired { get; set; }

    #endregion
}
