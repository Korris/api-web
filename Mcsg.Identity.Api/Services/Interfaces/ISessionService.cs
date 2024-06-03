using Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Identity.Api.Services.Interfaces
{
    public interface ISessionService
    {
        Task<Session> CreateSessionAsync(User user, string provider);
        Task ExpireSession(Session session);
        Task ExpireSessionByUserId(Guid userId);
    }
}
