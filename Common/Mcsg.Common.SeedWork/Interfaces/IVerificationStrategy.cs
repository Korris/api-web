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

namespace Mcsg.Common.SeedWork.Interfaces;

using Responses;

/// <summary>
/// Interface verification<br/>
/// https://refactoring.guru/design-patterns/strategy/csharp/example
/// </summary>
public interface IVerificationStrategy
{
    #region -- Methods --

    /// <summary>
    /// Verify token
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns>Return the result</returns>
    Task<SingleResponse> Verify(string? token);

    /// <summary>
    /// Set HTTP client
    /// </summary>
    /// <param name="client">HTTP client</param>
    void SetHttpClient(HttpClient client);

    #endregion
}
