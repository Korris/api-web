namespace Mcsg.Lib.Data.Repositories;

using Mcsg.Common.Domain.Entities;

public interface IUserRefreshTokenRepository : IRepository<UserRefreshToken>
{
    Task<IEnumerable<UserRefreshToken>> GetByUserIdAsync(Guid userId);

    Task<UserRefreshToken> GetByRefreshTokenAsync(string refreshToken);

    Task<UserRefreshToken> GetByRefreshTokenAsync(Guid userId, string refreshToken);
}