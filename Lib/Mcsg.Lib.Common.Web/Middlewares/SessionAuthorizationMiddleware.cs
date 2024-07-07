using Microsoft.AspNetCore.Http;
using System.Data;
using System.Security.Claims;

namespace Mcsg.Lib.Common.Web.Middlewares
{
    using Constants;
    using Extensions;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Repositories;
    using Mcsg.Common.SeedWork.Exceptions;

    public class SessionAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private IRepository<Session> _sessionRepository;
        public SessionAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, IRepository<Session> sessionRepository)
        {
            _sessionRepository = sessionRepository;

            if (!(context.User?.Identity?.IsAuthenticated ?? false))
            {
                await _next(context);
                return;
            }

            Guid sessionId = context.GetSessionId();
            Session session = await _sessionRepository.GetByIdAsync(sessionId, newConection: true);
            if (session == null || session.ExpiredDateUtc <= DateTime.UtcNow)
                throw new UnauthorizedAccessException(ErrorCodes.InvalidSession);
            context.Items[nameof(Session).ToLower()] = session;
            await UpdateUserSessionAsync(session);
            UpdateUserClaims(session, context);

            await _next(context);
        }

        #region Private Methods
        private async Task<bool> UpdateUserSessionAsync(Session session)
        {
            session.LastActionDateUtc = DateTime.UtcNow;
            return await _sessionRepository.UpdateAsync(session, newConection: true);
        }
        private void UpdateUserClaims(Session session, HttpContext context)
        {
            var claims = new List<Claim>();
            claims.AddRange(session.Roles.Split(",").Select(role => new Claim(ClaimTypes.Role, role)));
            claims.Add(new Claim(SecurityClaimTypes.UserNameClaimName, session.UserName ?? ""));
            claims.Add(new Claim(SecurityClaimTypes.ProfileNameClaimName, session.ProfileName ?? ""));
            claims.Add(new Claim(SecurityClaimTypes.UserIdClaimName, session.UserId.ToString()));
            claims.Add(new Claim(SecurityClaimTypes.SessionIdClaimName, session.Id.ToString()));
            claims.Add(new Claim(SecurityClaimTypes.UserAvatarClaimName, session.UserAvatar ?? ""));

            context.User.AddIdentity(new ClaimsIdentity(claims));
        }
        #endregion
    }
}
