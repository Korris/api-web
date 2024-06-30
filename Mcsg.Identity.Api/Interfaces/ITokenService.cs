namespace Mcsg.Identity.Api.Interfaces
{
    using DTOs;
    using DTOs.Response;
    using Lib.Data.Domain.Entities;

    public interface ITokenService
    {
        TokenResponse GenerateAccessToken(Guid sessionId);

        Task<RefreshTokenDto> AddUserRefreshTokenAsync(User user);

        Task<Guid> IsValidRefreshTokenAsync(string refreshToken);

        Guid GetSessionIdFromToken(string accessToken);
        Task<bool> DeleteRefreshTokenAsync(Guid userId);
    }
}
