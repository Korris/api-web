namespace Mcsg.Identity.Api.Interfaces;

using Common.Core.Dtos;
using Lib.Data.Domain.Entities;
using Response;

public interface ITokenService
{
    TokenResponse GenerateAccessToken(Guid sessionId);

    Task<RefreshTokenDto?> AddUserRefreshTokenAsync(User user);

    Task<Guid> IsValidRefreshTokenAsync(string refreshToken);

    Guid GetSessionIdFromToken(string accessToken);

    Task<bool> DeleteRefreshTokenAsync(Guid userId);
}
