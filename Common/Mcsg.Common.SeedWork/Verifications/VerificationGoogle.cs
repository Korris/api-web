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
/// Verification Google
/// </summary>
public class VerificationGoogle : VerificationStrategy
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

        var uri = $"{_secret.ApiUrl}/tokeninfo?id_token={token}";
        var res = await GetAsync(uri);
        if (!res.Succeeded)
        {
            return res.SetError(res.Message + "");
        }

        var aud = GetProperty(res, "aud").GetString();
        if (aud != _secret.AppId)
        {
            return res.SetError("Invalid AppId");
        }

        var userId = GetProperty(res, "sub").GetString();
        var email = GetProperty(res, "email").GetString();
        var firstName = GetProperty(res, "given_name").GetString();
        var lastName = GetProperty(res, "family_name").GetString();

        var o = new VerificationResultDto
        {
            LoginProvider = Google,
            ProviderKey = userId + "",
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };
        return res.SetSuccess(o);
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="secret">Secret</param>
    public VerificationGoogle(VerificationSecretDto secret)
    {
        _secret = secret;
    }

    #endregion
}
