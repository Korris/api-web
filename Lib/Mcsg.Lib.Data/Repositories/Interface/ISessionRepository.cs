using Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Lib.Data.Repositories
{
    public interface ISessionRepository : IRepository<Session>
    {
        Task<IEnumerable<Session>> GetByUserIdAsync(Guid userId);

        Task<IEnumerable<Session>> GetByUserNameAsync(string userName);
    }
}