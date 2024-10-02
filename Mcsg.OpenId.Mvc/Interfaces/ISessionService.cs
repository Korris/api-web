namespace Mcsg.OpenId.Mvc.Interfaces;

using Common.Domain.Entities;

public interface ISessionService
{
    Task<Session> CreateSessionAsync(User user, string provider);
    Task ExpireSession(Session session);
    Task ExpireSessionByUserId(Guid userId);
}
