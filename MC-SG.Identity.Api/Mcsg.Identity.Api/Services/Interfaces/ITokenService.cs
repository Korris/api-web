using Mcsg.Identity.Api.DTOs;
using Mcsg.Identity.Api.DTOs.Response;
using Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Identity.Api.Services.Interfaces
{
    public interface ITokenService
    {
        TokenResponse GenerateAccessToken(Guid sessionId);

        Task<RefreshTokenDto> AddUserRefreshTokenAsync(User user);

        Task<Guid> IsValidRefreshTokenAsync(string refreshToken);

        Guid GetSessionIdFromToken(string accessToken);
        Task<bool> DeleteRefreshTokenAsync(Guid userId);
    }
}
