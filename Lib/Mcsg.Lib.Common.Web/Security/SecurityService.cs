using NETCore.Encrypt;
using Newtonsoft.Json;

namespace Mcsg.Lib.Common.Web.Security;

public class SecurityService : ISecurityService
{
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
    private string FromBase64UrltoBase64String(string base64Url)
    {
        string padded = base64Url.Length % 4 == 0
            ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
        string base64 = padded.Replace("_", "/")
                              .Replace("-", "+");
        return base64;
    }
    public string ToBase64String(string s)
    {
        return EncryptProvider.Base64Encrypt(s);
    }
    public string FromBase64ToString(string s)
    {
        return EncryptProvider.Base64Decrypt(FromBase64UrltoBase64String(s));
    }
}
