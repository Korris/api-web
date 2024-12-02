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

using SeedWork.Interfaces;
using SeedWork.Responses;

/// <summary>
/// Verification client
/// </summary>
public class VerificationClient : IVerificationClient
{
    #region -- Implements --

    /// <summary>
    /// Set strategy
    /// </summary>
    /// <param name="strategy">Strategy</param>
    public void SetStrategy(IVerificationStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(_client, nameof(_client));

        strategy.SetHttpClient(_client);
        _strategy = strategy;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns>Return the result</returns>
    public Task<SingleResponse> Handle(string? token)
    {
        ArgumentNullException.ThrowIfNull(_strategy, nameof(_strategy));

        return _strategy.Verify(token);
    }

    /// <summary>
    /// Client
    /// </summary>
    public HttpClient? Client => _client;

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="client">HTTP client</param>
    public VerificationClient(HttpClient? client)
    {
        _client = client;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Strategy
    /// </summary>
    private IVerificationStrategy? _strategy;

    /// <summary>
    /// HTTP client
    /// </summary>
    private readonly HttpClient? _client;

    #endregion
}
