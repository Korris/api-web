namespace Mcsg.Identity.Api.Interfaces;

using Common.Core.Dtos;
using Common.Domain.Entities;

public interface ITokenService
{
    TokenDto GenerateAccessToken(Guid sessionId, User user);

    Task<RefreshTokenDto?> AddUserRefreshTokenAsync(User user);

    Task<Guid> IsValidRefreshTokenAsync(string refreshToken);

    Guid GetSessionIdFromToken(string accessToken);

    Task<bool> DeleteRefreshTokenAsync(Guid userId);
}
