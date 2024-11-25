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
using Interfaces;
using Responses;

/// <summary>
/// Verification strategy
/// </summary>
public class VerificationStrategy : IVerificationStrategy
{
    #region -- Implements --

    /// <summary>
    /// Verify token
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns>Return the result</returns>
    public virtual Task<SingleResponse> Verify(string? token)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Set HTTP client
    /// </summary>
    /// <param name="client">HTTP client</param>
    public void SetHttpClient(HttpClient client)
    {
        _client = new RestApi(client);
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// GetProperty
    /// </summary>
    /// <param name="response">Response</param>
    /// <param name="property">Property</param>
    /// <returns>Return the result</returns>
    protected JsonElement GetProperty(SingleResponse response, string property)
    {
        var jd = JsonDocument.Parse(response?.Data + "");
        return jd.RootElement.GetProperty(property);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Secret
    /// </summary>
    protected VerificationSecretDto? _secret;

    /// <summary>
    /// HTTP client
    /// </summary>
    protected IRestApi? _client;

    #endregion
}
