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

using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Mcsg.Common.SeedWork;

/// <summary>
/// Security Advanced Encryption Standard
/// </summary>
/// <remarks>
/// Initialize
/// </remarks>
/// <param name="passphrase">Passphrase</param>
public class SecurityAes(string passphrase)
{
    #region -- Methods --

    /// <summary>
    /// Encrypt text
    /// </summary>
    /// <param name="plainText">Plain text</param>
    /// <param name="urlEncode">URL encode</param>
    /// <returns>Return the cipher text</returns>
    public string EncryptText(string plainText, bool urlEncode = false)
    {
        return EncryptText(plainText, urlEncode, _passphrase);
    }

    /// <summary>
    /// Decrypt text
    /// </summary>
    /// <param name="cipherText">Cipher text</param>
    /// <param name="urlDecode">URL decode</param>
    /// <returns>Return the plain text</returns>
    public string DecryptText(string cipherText, bool urlDecode = false)
    {
        return DecryptText(cipherText, urlDecode, _passphrase);
    }

    /// <summary>
    /// Encrypt text
    /// </summary>
    /// <param name="plainText">Plain text</param>
    /// <param name="urlEncode">URL encode</param>
    /// <param name="passphrase">Passphrase</param>
    /// <returns>Return the cipher text</returns>
    public static string EncryptText(string plainText, bool urlEncode = false, string passphrase = Passphrase)
    {
        var salt = Encoding.UTF8.GetBytes(passphrase.Length.ToString());

        using var aesAlg = Aes.Create();
        Rfc2898DeriveBytes keyDerivationFunction = new(passphrase, salt, iterations: 10000, HashAlgorithmName.SHA256);
        aesAlg.Key = keyDerivationFunction.GetBytes(aesAlg.KeySize / 8);
        aesAlg.IV = keyDerivationFunction.GetBytes(aesAlg.BlockSize / 8);

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msEncrypt = new();
        using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (StreamWriter swEncrypt = new(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        var cipherText = Convert.ToBase64String(msEncrypt.ToArray());
        return urlEncode ? HttpUtility.UrlEncode(cipherText) : cipherText;
    }

    /// <summary>
    /// Decrypt text
    /// </summary>
    /// <param name="cipherText">Cipher text</param>
    /// <param name="urlDecode">URL decode</param>
    /// <param name="passphrase">Passphrase</param>
    /// <returns>Return the plain text</returns>
    public static string DecryptText(string cipherText, bool urlDecode = false, string passphrase = Passphrase)
    {
        if (urlDecode)
        {
            cipherText = HttpUtility.UrlDecode(cipherText);
            cipherText = cipherText.Replace(" ", "+");
        }

        var salt = Encoding.UTF8.GetBytes(passphrase.Length.ToString());
        var cipherBytes = Convert.FromBase64String(cipherText);

        using var aesAlg = Aes.Create();
        Rfc2898DeriveBytes keyDerivationFunction = new(passphrase, salt, iterations: 10000, HashAlgorithmName.SHA256);
        aesAlg.Key = keyDerivationFunction.GetBytes(aesAlg.KeySize / 8);
        aesAlg.IV = keyDerivationFunction.GetBytes(aesAlg.BlockSize / 8);

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msDecrypt = new(cipherBytes);
        using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
        using StreamReader srDecrypt = new(csDecrypt);

        return srDecrypt.ReadToEnd();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Passphrase
    /// </summary>
    private readonly string _passphrase = passphrase;

    #endregion

    #region -- Constants --

    /// <summary>
    /// Passphrase
    /// </summary>
    private const string Passphrase = "MySecretPassphrase";

    #endregion
}
