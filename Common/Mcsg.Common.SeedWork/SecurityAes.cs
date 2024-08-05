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

using Extensions;

/// <summary>
/// Security Advanced Encryption Standard
/// </summary>
public class SecurityAes : ISecurityAes
{
    #region -- Implements --

    /// <summary>
    /// Encrypt text
    /// </summary>
    /// <param name="plainText">Plain text</param>
    /// <param name="urlEncode">URL encode</param>
    /// <returns>Return the cipher text</returns>
    public string? EncryptText(string? plainText, bool urlEncode = false)
    {
        return EncryptText(plainText, urlEncode, _passphrase);
    }

    /// <summary>
    /// Decrypt text
    /// </summary>
    /// <param name="cipherText">Cipher text</param>
    /// <param name="urlDecode">URL decode</param>
    /// <returns>Return the plain text</returns>
    public string? DecryptText(string? cipherText, bool urlDecode = false)
    {
        try
        {
            return DecryptText(cipherText, urlDecode, _passphrase);
        }
        catch
        {
            return cipherText;
        }
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="passphrase">Passphrase</param>
    public SecurityAes(string passphrase)
    {
        _passphrase = passphrase;
    }

    /// <summary>
    /// Encrypt text
    /// </summary>
    /// <param name="plainText">Plain text</param>
    /// <param name="urlEncode">URL encode</param>
    /// <param name="passphrase">Passphrase</param>
    /// <returns>Return the cipher text</returns>
    public static string? EncryptText(string? plainText, bool urlEncode = false, string passphrase = Passphrase)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return plainText;
        }

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
    public static string? DecryptText(string? cipherText, bool urlDecode = false, string passphrase = Passphrase)
    {
        if (string.IsNullOrEmpty(cipherText))
        {
            return cipherText;
        }

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

    /// <summary>
    /// Encrypt
    /// </summary>
    /// <param name="plainText">Plain text</param>
    /// <returns>Return the result</returns>
    public static string Encrypt(string plainText)
    {
        var keyS = 16.GetRandomString();
        var ivS = 16.GetRandomString();
        var keyB = Encoding.UTF8.GetBytes(keyS);
        var ivB = Encoding.UTF8.GetBytes(ivS);

        using (var aes = Aes.Create())
        {
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.FeedbackSize = 128;

            var encryptor = aes.CreateEncryptor(keyB, ivB);

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    var bytes = Encoding.ASCII.GetBytes(plainText);
                    cs.Write(bytes, 0, bytes.Length);
                    cs.FlushFinalBlock();

                    return Convert.ToBase64String(ms.ToArray()) + keyS + ivS;
                }
            }
        }
    }

    /// <summary>
    /// Decrypt
    /// </summary>
    /// <param name="cipherText">Cipher text</param>
    /// <returns>Return the result</returns>
    public static string? Decrypt(string? cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
        {
            return null;
        }

        var ivS = cipherText.Substring(cipherText.Length - 16, 16);
        var ivB = Encoding.UTF8.GetBytes(ivS);

        var keyS = cipherText.Substring(cipherText.Length - 32, 16);
        var keyB = Encoding.UTF8.GetBytes(keyS);

        var data = cipherText.Substring(0, cipherText.Length - 32);
        var encrypted = Convert.FromBase64String(data);

        return DecryptStringFromBytes(encrypted, keyB, ivB);
    }

    /// <summary>
    /// Decrypt string from bytes
    /// </summary>
    /// <param name="encrypted">Encrypted</param>
    /// <param name="key">Key</param>
    /// <param name="iv">IV</param>
    /// <returns>Return the result</returns>
    private static string DecryptStringFromBytes(byte[] encrypted, byte[] key, byte[] iv)
    {
        var res = string.Empty;

        using (Aes aes = Aes.Create())
        {
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.FeedbackSize = 128;
            aes.Key = key;
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (var ms = new MemoryStream(encrypted))
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs))
                    {
                        res = sr.ReadToEnd();
                    }
                }
            }
        }

        return res;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Passphrase
    /// </summary>
    private readonly string _passphrase;

    #endregion

    #region -- Constants --

    /// <summary>
    /// Passphrase
    /// </summary>
    private const string Passphrase = "MySecretPassphrase";

    #endregion
}
