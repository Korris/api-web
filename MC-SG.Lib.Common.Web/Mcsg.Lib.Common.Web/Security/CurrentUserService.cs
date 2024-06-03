using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Security.Models;
using Mcsg.Lib.Common.Web.Extensions;
using Mcsg.Lib.Data.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mcsg.Lib.Common.Web.Security
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor), "It is required to inject HttpContextAccessor");
            }

            _httpContextAccessor = httpContextAccessor;
        }

        public Session Session
        {
            get
            {
                var currentSession = _httpContextAccessor.HttpContext.Items[nameof(Session).ToLower()] as Session;
                return currentSession;
            }
        }

        public Task<CurrentUserModel> GetCurrentUserAsync()
        {
            var userIdClaim = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type.Equals(SecurityClaimTypes.UserIdClaimName));
            var sessionIdClaim = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault(claim => claim.Type.Equals(SecurityClaimTypes.SessionIdClaimName));
            var userId = string.IsNullOrWhiteSpace(userIdClaim?.Value) ? (Guid?)null : new Guid(userIdClaim.Value);
            return Task.FromResult(new CurrentUserModel
            {
                UserId = userId,
                SessionId = sessionIdClaim?.Value ?? "",
                Claims = _httpContextAccessor?.HttpContext?.User?.Claims,
            });
        }
        public Task<bool> RemoveCurrentUserAsync()
        {
            _httpContextAccessor?.HttpContext?.RemoveAuthorization();
            return Task.FromResult(true);
        }
    }
}
