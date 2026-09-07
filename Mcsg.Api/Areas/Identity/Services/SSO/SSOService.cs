using Newtonsoft.Json;

namespace Mcsg.Api.Areas.Identity.Services;

using Common.Core.Interfaces;
using Mcsg.Api.Areas.Identity.Interfaces;
using Responses;

public abstract class SSOService : ISSOService
{
    #region -- Methods --

    public SSOService(ILogger<SSOService> logger, ISecurityService securityService)
    {
        _logger = logger;
        _securityService = securityService;
        serializerSettings.TypeNameHandling = TypeNameHandling.All;
    }

    public abstract Task<SocialTokenResponse> VerifyToken(string socialToken);

    #endregion

    #region -- Properties --

    protected ILogger<SSOService> _logger;
    protected readonly ISecurityService _securityService;
    protected JsonSerializerSettings serializerSettings = new();

    #endregion
}
