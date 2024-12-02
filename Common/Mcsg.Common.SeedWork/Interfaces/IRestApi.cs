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

namespace Mcsg.Common.SeedWork;

using Responses;

/// <summary>
/// Interface RestApi
/// </summary>
public interface IRestApi
{
    #region -- Methods --

    /// <summary>
    /// Set basic authorization
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    void SetBasicAuthorization(string clientId, string clientSecret);

    /// <summary>
    /// SetBearerToken
    /// </summary>
    /// <param name="token">Token</param>
    void SetBearerToken(string token);

    /// <summary>
    /// SetRequestHeader
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    void SetRequestHeader(string key, string value);

    /// <summary>
    /// Get async
    /// </summary>
    /// <param name="uri">URI</param>
    /// <returns>Return the result</returns>
    Task<SingleResponse> GetAsync(string uri);

    /// <summary>
    /// Post async
    /// </summary>
    /// <param name="uri">URI</param>
    /// <param name="content">HTTP content</param>
    /// <returns>Return the result</returns>
    Task<SingleResponse> PostAsync(string uri, HttpContent? content);

    /// <summary>
    /// Post JSON async
    /// </summary>
    /// <param name="uri">URI</param>
    /// <param name="data">JSON data</param>
    /// <returns>Return the result</returns>
    Task<SingleResponse> PostJsonAsync(string uri, string? data);

    /// <summary>
    /// Post form async
    /// </summary>
    /// <param name="uri">URI</param>
    /// <param name="data">Form input data</param>
    /// <returns>Return the result</returns>
    Task<SingleResponse> PostFormAsync(string uri, Dictionary<string, string>? data);

    #endregion
}
