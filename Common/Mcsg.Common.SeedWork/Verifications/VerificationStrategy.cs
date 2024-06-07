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
        _client = client;
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Get async
    /// </summary>
    /// <param name="uri">URI</param>
    /// <returns>Return the result</returns>
    protected async Task<SingleResponse> GetAsync(string uri)
    {
        var res = new SingleResponse();

        ArgumentNullException.ThrowIfNull(_client, nameof(_client));

        try
        {
            var rsp = await _client.GetAsync(uri);
            if (rsp.IsSuccessStatusCode)
            {
                var data = await rsp.Content.ReadAsStringAsync();
                res.SetSuccess(data);
            }
            else
            {
                res.SetError(rsp.ReasonPhrase + "");
            }
        }
        catch (Exception ex)
        {
            res.SetError(ex.Message);
        }

        return res;
    }

    /// <summary>
    /// Post async
    /// </summary>
    /// <param name="uri">URI</param>
    /// <param name="content">HTTP content</param>
    /// <returns>Return the result</returns>
    protected async Task<SingleResponse> PostAsync(string uri, HttpContent? content)
    {
        var res = new SingleResponse();

        ArgumentNullException.ThrowIfNull(_client, nameof(_client));

        try
        {
            var rsp = await _client.PostAsync(uri, content);
            if (rsp.IsSuccessStatusCode)
            {
                var data = await rsp.Content.ReadAsStringAsync();
                res.SetSuccess(data);
            }
            else
            {
                res.SetError(rsp.ReasonPhrase + "");
            }
        }
        catch (Exception ex)
        {
            res.SetError(ex.Message);
        }

        return res;
    }

    /// <summary>
    /// Post JSON async
    /// </summary>
    /// <param name="uri">URI</param>
    /// <param name="data">JSON data</param>
    /// <returns>Return the result</returns>
    protected async Task<SingleResponse> PostJsonAsync(string uri, string? data)
    {
        if (data == null)
        {
            return await PostAsync(uri, null);
        }

        var content = new StringContent(data, Encoding.UTF8, ContentType);

        return await PostAsync(uri, content);
    }

    /// <summary>
    /// Post form async
    /// </summary>
    /// <param name="uri">URI</param>
    /// <param name="data">Form input data</param>
    /// <returns>Return the result</returns>
    protected async Task<SingleResponse> PostFormAsync(string uri, Dictionary<string, string>? data)
    {
        if (data == null)
        {
            return await PostAsync(uri, null);
        }

        var content = new MultipartFormDataContent();
        foreach (var i in data)
        {
            content.Add(new StringContent(i.Value), i.Key);
        }

        return await PostAsync(uri, content);
    }

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
    private HttpClient? _client;

    #endregion

    #region -- Constants --

    /// <summary>
    /// Content type
    /// </summary>
    private const string ContentType = "application/json";

    #endregion
}
