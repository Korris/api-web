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

using System.Text;

namespace Mcsg.Common.SeedWork.Verifications;

using Dtos;
using Responses;
using static Common.SeedWork.Constants.Provider;

/// <summary>
/// Verification Paypal
/// </summary>
public class VerificationPaypal : VerificationStrategy
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
        ArgumentNullException.ThrowIfNull(_client, nameof(_client));

        var content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
        _client.SetBasicAuthorization(_secret.AppId, _secret.Secret);
        var res = await _client.PostAsync($"{_secret.ApiUrl}/v1/oauth2/token", content);
        if (!res.Succeeded)
        {
            return res.SetError(res.Message + "");
        }

        var appId = GetProperty(res, "app_id").GetString();
        var accessToken = GetProperty(res, "access_token").GetString();
        var expiresIn = GetProperty(res, "expires_in").GetInt32();

        var o = new VerificationResultDto
        {
            LoginProvider = Paypal,
            ProviderKey = appId + "",
            AccessToken = accessToken,
            ExpiresIn = expiresIn
        };
        return res.SetSuccess(o);
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="secret">Secret</param>
    public VerificationPaypal(VerificationSecretDto secret)
    {
        _secret = secret;
    }

    #endregion
}
