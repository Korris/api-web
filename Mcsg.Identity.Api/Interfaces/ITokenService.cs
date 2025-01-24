namespace Mcsg.Identity.Api.Interfaces;

using Common.Core.Dtos;
using Common.Domain.Entities;

public interface ITokenService
{
    /// <summary>
    /// IsValid async
    /// </summary>
    /// <param name="rt">Refresh token</param>
    /// <returns>Returns the result</returns>
    Task<Guid?> IsValidAsync(string? rt);

    /// <summary>
    /// Add async
    /// </summary>
    /// <param name="user">User</param>
    /// <returns>Returns the result</returns>
    Task<RefreshTokenDto?> AddAsync(User? user);

    /// <summary>
    /// Delete async
    /// </summary>
    /// <param name="userId">UserId</param>
    /// <param name="rt">RefreshToken</param>
    /// <returns>Returns the result</returns>
    Task<bool> DeleteAsync(Guid? userId, string? rt);

    /// <summary>
    /// Delete async
    /// </summary>
    /// <param name="rt">RefreshToken</param>
    /// <returns>Returns the result</returns>
    Task<bool> DeleteAsync(string? rt);
}
