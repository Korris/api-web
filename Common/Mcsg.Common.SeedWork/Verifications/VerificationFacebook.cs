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

using System.Text.Json;

namespace Mcsg.Common.SeedWork.Verifications;

using Dtos;
using Responses;
using static Common.SeedWork.Constants.Provider;

/// <summary>
/// Verification Facebook
/// </summary>
public class VerificationFacebook : VerificationStrategy
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

        var uri = $"{_secret.ApiUrl}/oauth/access_token?client_id={_secret.AppId}&client_secret={_secret.Secret}&grant_type=client_credentials";
        var res = await _client.GetAsync(uri);
        if (!res.Succeeded)
        {
            return res.SetError(res.Message + "");
        }

        var at = GetProperty(res, "access_token").GetString();
        uri = $"{_secret.ApiUrl}/debug_token?input_token={token}&access_token={at}";
        res = await _client.GetAsync(uri);
        if (!res.Succeeded)
        {
            return res.SetError(res.Message + "");
        }

        var data = GetProperty(res, "data");
        var ok = data.GetProperty("is_valid").GetBoolean();
        if (!ok)
        {
            return res.SetError("Invalid token");
        }

        var userId = data.GetProperty("user_id").GetString();
        JsonElement je;

        string? email = null;
        data.TryGetProperty("email", out je);
        if (je.ValueKind == JsonValueKind.String)
        {
            email = je.GetString();
        }

        string? firstName = null;
        data.TryGetProperty("given_name", out je);
        if (je.ValueKind == JsonValueKind.String)
        {
            firstName = je.GetString();
        }

        string? lastName = null;
        data.TryGetProperty("family_name", out je);
        if (je.ValueKind == JsonValueKind.String)
        {
            lastName = je.GetString();
        }

        var o = new VerificationResultDto
        {
            LoginProvider = Facebook,
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
    public VerificationFacebook(VerificationSecretDto secret)
    {
        _secret = secret;
    }

    #endregion
}
