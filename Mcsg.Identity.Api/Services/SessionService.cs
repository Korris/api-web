using Dapper;
using Microsoft.Extensions.Options;

namespace Mcsg.Identity.Api.Services;

using Interfaces;
using Lib.Common.Models;
using Lib.Data.Domain.Entities;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;

public partial class SessionService : ISessionService
{
    private readonly JwtSetting _jwtConfiguration;
    private readonly IRepository<Session> _sessionRepository;
    public SessionService(
        IUnitOfWork unitOfWork,
        IOptions<JwtSetting> jwtConfiguration)
    {
        _jwtConfiguration = jwtConfiguration.Value;
        _sessionRepository = unitOfWork.GetRepository<Session>();
    }

    public async Task<Session> CreateSessionAsync(User user, string provider)
    {
        var roles = await _sessionRepository.Connection.QueryAsync<string>(GetUserRoleQuery, new { userId = user.Id });
        var utcNow = DateTime.UtcNow;
        var session = new Session
        {
            Id = Guid.NewGuid(),
            LoginProvider = provider,
            ExpiredDateUtc = utcNow.AddMinutes(_jwtConfiguration.ExpiredTokenTimeInMinute),
            LastActionDateUtc = utcNow,
            UserId = user.Id,
            Email = user.Email ?? "",
            UserName = user.UserName,
            ProfileName = user.ProfileName,
            PremiumDate = user.PremiumDate,
            UserAvatar = user.Avatar,
            ProfileId = user.ProfileId,
            Roles = string.Join(",", roles),

        };
        await _sessionRepository.InsertAsync(session);
        return session;
    }

    public async Task ExpireSession(Session session)
    {
        session.ExpiredDateUtc = DateTime.UtcNow;
        await _sessionRepository.UpdateAsync(session);
    }
    public async Task ExpireSessionByUserId(Guid userId)
    {
        await _sessionRepository.Connection.ExecuteAsync(ExpiredUserSessionsQuery, new { userid = userId });
    }
}
