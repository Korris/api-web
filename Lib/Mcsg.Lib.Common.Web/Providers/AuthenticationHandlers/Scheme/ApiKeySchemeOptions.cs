using Microsoft.AspNetCore.Authentication;

namespace Mcsg.Lib.Common.Web.Providers.AuthHandlers.Scheme
{
    public class ApiKeySchemeOptions : AuthenticationSchemeOptions
    {
        public string AuthKey { get; set; }
        public string HeaderName { get; set; }
    }
}
