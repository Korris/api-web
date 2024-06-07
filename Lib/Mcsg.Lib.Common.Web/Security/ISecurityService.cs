namespace Mcsg.Lib.Common.Web.Security
{
    public interface ISecurityService
    {
        bool RsaVerifySignature(string tokenString, string modulus, string exponent);
        string ToBase64String(string s);
        string FromBase64ToString(string s);
    }
}
