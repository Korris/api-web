namespace Mcsg.Lib.Data.Repositories;

using Domain.Entities;

public interface IUserSocialRepository : IRepository<UserSocial>
{
    Task<IEnumerable<UserSocial>> GetByUserIdAsync(Guid userId);

    Task<UserSocial> GetByTypeAsync(Guid userId, string type);

    Task<UserSocial> GetBySocialIdAsync(string socialId, string type);
}