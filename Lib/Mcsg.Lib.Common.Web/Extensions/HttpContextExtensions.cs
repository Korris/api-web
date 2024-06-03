using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;

namespace Mcsg.Lib.Common.Web.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid GetSessionId(this HttpContext context)
            => (context.User?.Claims?.FirstOrDefault(c => c.Type == SecurityClaimTypes.SessionIdClaimName)?.Value ?? string.Empty).ToGuid();
        public static void RemoveAuthorization(this HttpContext context)
        {
            context.Request.Headers.Remove(HeaderNames.Authorization);
            context.User = null;
        }
    }
}
