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
/// Interface verification client
/// </summary>
public interface IVerificationClient
{
    #region -- Methods --

    /// <summary>
    /// Set strategy
    /// </summary>
    /// <param name="strategy">Strategy</param>
    void SetStrategy(IVerificationStrategy strategy);

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns>Return the result</returns>
    Task<SingleResponse> Handle(string? token);

    #endregion

    #region -- Properties --

    /// <summary>
    /// Client
    /// </summary>
    HttpClient? Client { get; }

    #endregion
}
