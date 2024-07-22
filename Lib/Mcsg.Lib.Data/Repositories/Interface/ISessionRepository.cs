namespace Mcsg.Lib.Data.Repositories;

using Mcsg.Common.Domain.Entities;

public interface ISessionRepository : IRepository<Session>
{
    Task<IEnumerable<Session>> GetByUserIdAsync(Guid userId);

    Task<IEnumerable<Session>> GetByUserNameAsync(string userName);
}