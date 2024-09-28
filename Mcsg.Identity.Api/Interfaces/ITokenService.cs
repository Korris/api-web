namespace Mcsg.Identity.Api.Interfaces;

using Common.Core.Dtos;
using Common.Domain.Entities;

public interface ITokenService
{
    Task<RefreshTokenDto?> AddUserRefreshTokenAsync(User user);

    Task<Guid> IsValidRefreshTokenAsync(string refreshToken);

    Task<bool> DeleteRefreshTokenAsync(Guid userId);
}
