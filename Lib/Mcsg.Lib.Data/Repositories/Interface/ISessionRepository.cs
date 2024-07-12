namespace Mcsg.Lib.Data.Repositories;

using Domain.Entities;

public interface ISessionRepository : IRepository<Session>
{
    Task<IEnumerable<Session>> GetByUserIdAsync(Guid userId);

    Task<IEnumerable<Session>> GetByUserNameAsync(string userName);
}