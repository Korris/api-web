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

using System.Net.Http.Headers;
using System.Text;

namespace Mcsg.Common.SeedWork;

using Responses;

/// <summary>
/// REST API
/// </summary>
public class RestApi : IRestApi
{
    #region -- Implements --

    /// <summary>
    /// Set basic authorization
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    public void SetBasicAuthorization(string clientId, string clientSecret)
    {
        var bytes = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
        var param = Convert.ToBase64String(bytes);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", param);
    }

    /// <summary>
    /// SetBearerToken
    /// </summary>
    /// <param name="token">Token</param>
    public void SetBearerToken(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// SetRequestHeader
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    public void SetRequestHeader(string key, string value)
    {
        _client.DefaultRequestHeaders.Add(key, value);
    }

    /// <summary>
    /// Get async
    /// </summary>
    /// <param name="uri">URI</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> GetAsync(string uri)
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
    public async Task<SingleResponse> PostAsync(string uri, HttpContent? content)
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
    public async Task<SingleResponse> PostJsonAsync(string uri, string? data)
    {
        if (data == null)
        {
            return await PostAsync(uri, null);
        }

        var content = new StringContent(data, Encoding.UTF8, "application/json");

        return await PostAsync(uri, content);
    }

    /// <summary>
    /// Post form async
    /// </summary>
    /// <param name="uri">URI</param>
    /// <param name="data">Form input data</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> PostFormAsync(string uri, Dictionary<string, string>? data)
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

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="client">HTTP client</param>
    public RestApi(HttpClient client)
    {
        _client = client;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// HTTP client
    /// </summary>
    private readonly HttpClient _client;

    #endregion
}
