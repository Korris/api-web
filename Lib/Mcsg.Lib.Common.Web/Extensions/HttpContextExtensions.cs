using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace Mcsg.Lib.Common.Web.Extensions
{
    using Mcsg.Lib.Common.Extensions;

    public static class HttpContextExtensions
    {
        public static Guid GetSessionId(this HttpContext context) => (context.User?.Claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sid)?.Value ?? string.Empty).ToGuid();

        public static void RemoveAuthorization(this HttpContext context)
        {
            context.Request.Headers.Remove(HeaderNames.Authorization);
            context.User = null;
        }
    }
}
