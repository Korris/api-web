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

namespace Mcsg.Common.SeedWork;

/// <summary>
/// Interface SecurityAes
/// </summary>
public interface ISecurityAes
{
    #region -- Methods --

    /// <summary>
    /// Encrypt text
    /// </summary>
    /// <param name="plainText">Plain text</param>
    /// <param name="urlEncode">URL encode</param>
    /// <returns>Return the cipher text</returns>
    string? EncryptText(string? plainText, bool urlEncode = false);

    /// <summary>
    /// Decrypt text
    /// </summary>
    /// <param name="cipherText">Cipher text</param>
    /// <param name="urlDecode">URL decode</param>
    /// <returns>Return the plain text</returns>
    string? DecryptText(string? cipherText, bool urlDecode = false);

    #endregion
}
