namespace Mcsg.Common.Core.Interfaces;

/// <summary>
/// ISecurityService
/// </summary>
public interface ISecurityService
{
    #region -- Methods --

    /// <summary>
    /// RsaVerifySignature
    /// </summary>
    /// <param name="tokenString"></param>
    /// <param name="modulus"></param>
    /// <param name="exponent"></param>
    /// <returns></returns>
    bool RsaVerifySignature(string tokenString, string modulus, string exponent);

    /// <summary>
    /// ToBase64String
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    string ToBase64String(string s);

    /// <summary>
    /// FromBase64ToString
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    string FromBase64ToString(string s);

    #endregion
}
