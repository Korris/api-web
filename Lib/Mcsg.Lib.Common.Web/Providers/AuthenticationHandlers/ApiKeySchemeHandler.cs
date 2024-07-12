using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Mcsg.Lib.Common.Web.Providers.AuthHandlers;

using Constants;
using Mcsg.Common.SeedWork.Exceptions;
using Scheme;

public class ApiKeySchemeHandler : AuthenticationHandler<ApiKeySchemeOptions>
{
    public ApiKeySchemeHandler(IOptionsMonitor<ApiKeySchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName.ToLower(), out StringValues requestApiKey) || !Options.AuthKey.Equals(requestApiKey))
            throw new UnauthorizedAccessException(ErrorCodes.InvalidApiKey);

        var identity = new ClaimsIdentity(new Claim[] { }, nameof(ApiKeySchemeHandler));
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
