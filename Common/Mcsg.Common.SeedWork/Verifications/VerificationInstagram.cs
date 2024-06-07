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

namespace Mcsg.Common.SeedWork.Verifications;

using Dtos;
using Responses;
using static Common.SeedWork.Constants.Provider;

/// <summary>
/// Verification Instagram
/// </summary>
public class VerificationInstagram : VerificationStrategy
{
    #region -- Overrides --

    /// <summary>
    /// Verify token
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns>Return the result</returns>
    public override async Task<SingleResponse> Verify(string? token)
    {
        ArgumentNullException.ThrowIfNull(_secret, nameof(_secret));

        var dic = new Dictionary<string, string>
        {
            { "client_id",_secret.AppId },
            { "client_secret", _secret.Secret },
            { "grant_type", "authorization_code" },
            { "redirect_uri", _secret.RedirectUri },
            { "code", token + "" }
        };

        var res = await PostFormAsync(_secret.ApiUrl, dic);
        if (!res.Succeeded)
        {
            res.SetError(res.Message + "");
            return res;
        }

        var userId = GetProperty(res, "user_id").GetInt64();

        //JsonElement je;

        string? email = null;
        //TODO
        /*data.TryGetProperty("email", out je);
        if (je.ValueKind == JsonValueKind.String)
        {
            email = je.GetString();
        }*/

        string? firstName = null;
        //TODO
        /*data.TryGetProperty("given_name", out je);
        if (je.ValueKind == JsonValueKind.String)
        {
            firstName = je.GetString();
        }*/

        string? lastName = null;
        //TODO
        /*data.TryGetProperty("family_name", out je);
        if (je.ValueKind == JsonValueKind.String)
        {
            lastName = je.GetString();
        }*/

        var o = new VerificationResultDto
        {
            LoginProvider = Instagram,
            ProviderKey = userId + "",
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };
        res.SetSuccess(o);

        return res;
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="secret">Secret</param>
    public VerificationInstagram(VerificationSecretDto secret)
    {
        _secret = secret;
    }

    #endregion
}
