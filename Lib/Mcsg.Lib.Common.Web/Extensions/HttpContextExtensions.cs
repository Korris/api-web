using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Mcsg.Lib.Common.Web.Extensions
{
    using Mcsg.Common.Core.Constants;
    using Mcsg.Lib.Common.Extensions;

    public static class HttpContextExtensions
    {
        public static Guid GetSessionId(this HttpContext context) => (context.User?.Claims?.FirstOrDefault(c => c.Type == Setting.SecurityClaim.SessionId)?.Value ?? string.Empty).ToGuid();

        public static void RemoveAuthorization(this HttpContext context)
        {
            context.Request.Headers.Remove(HeaderNames.Authorization);
            context.User = null;
        }
    }
}
