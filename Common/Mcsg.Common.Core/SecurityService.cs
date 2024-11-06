using NETCore.Encrypt;
using Newtonsoft.Json;

namespace Mcsg.Common.Core;

using Interfaces;

/// <summary>
/// SecurityService
/// </summary>
public class SecurityService : ISecurityService
{
    #region -- Implements --

    /// <summary>
    /// RsaVerifySignature
    /// </summary>
    /// <param name="tokenString"></param>
    /// <param name="modulus"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    public bool RsaVerifySignature(string tokenString, string modulus, string exponent)
    {
        string tokenStr = tokenString;
        string[] tokenParts = tokenStr.Split('.');
        var key = new
        {
            Modulus = FromBase64UrltoBase64String(modulus),
            Exponent = FromBase64UrltoBase64String(exponent)
        };
        var result = EncryptProvider.RSAVerify(tokenParts[0] + '.' + tokenParts[1], FromBase64UrltoBase64String(tokenParts[2]), JsonConvert.SerializeObject(key));
        return result;
    }

    /// <summary>
    /// ToBase64String
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public string ToBase64String(string s)
    {
        return EncryptProvider.Base64Encrypt(s);
    }

    /// <summary>
    /// FromBase64ToString
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public string FromBase64ToString(string s)
    {
        return EncryptProvider.Base64Decrypt(FromBase64UrltoBase64String(s));
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// FromBase64UrltoBase64String
    /// </summary>
    /// <param name="base64Url"></param>
    /// <returns></returns>
    private string FromBase64UrltoBase64String(string base64Url)
    {
        string padded = base64Url.Length % 4 == 0
            ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
        string base64 = padded.Replace("_", "/")
                              .Replace("-", "+");
        return base64;
    }

    #endregion
}
