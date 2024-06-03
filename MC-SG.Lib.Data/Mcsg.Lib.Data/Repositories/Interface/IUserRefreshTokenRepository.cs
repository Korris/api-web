using Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Lib.Data.Repositories
{
    public interface IUserRefreshTokenRepository : IRepository<UserRefreshToken>
    {
        Task<IEnumerable<UserRefreshToken>> GetByUserIdAsync(Guid userId);

        Task<UserRefreshToken> GetByRefreshTokenAsync(string refreshToken);

        Task<UserRefreshToken> GetByRefreshTokenAsync(Guid userId, string refreshToken);
    }
}