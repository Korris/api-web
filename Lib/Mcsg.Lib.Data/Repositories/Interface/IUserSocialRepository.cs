using Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Lib.Data.Repositories
{
    public interface IUserSocialRepository : IRepository<UserSocial>
    {
        Task<IEnumerable<UserSocial>> GetByUserIdAsync(Guid userId);

        Task<UserSocial> GetByTypeAsync(Guid userId, string type);

        Task<UserSocial> GetBySocialIdAsync(string socialId, string type);
    }
}