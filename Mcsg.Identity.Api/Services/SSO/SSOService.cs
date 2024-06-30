using Dapper;
using Newtonsoft.Json;

namespace Mcsg.Identity.Api.Services;

using Interfaces;
using Lib.Common.Web.Security;
using Lib.Data.Domain.Entities;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Response;

public abstract partial class SSOService : ISSOService
{
    protected JsonSerializerSettings serializerSettings = new();
    protected ILogger<SSOService> _logger;
    private readonly IRepository<UserSocial> _userSocialRepository;
    protected readonly ISecurityService _securityService;

    public SSOService(ILogger<SSOService> logger, IUnitOfWork unitOfWork, ISecurityService securityService)
    {
        serializerSettings.TypeNameHandling = TypeNameHandling.All;
        _logger = logger;
        _userSocialRepository = unitOfWork.GetRepository<UserSocial>();
        _securityService = securityService;

    }

    public async Task<UserSocial> GetUserSocialBySocialId(string socialType, string socialId)
    {
        return await _userSocialRepository
                    .Connection.QueryFirstOrDefaultAsync<UserSocial>(GetUserSocialQuery, new { SocialId = socialId, Type = socialType.ToLower() });
    }

    public async Task<bool> AddUserSocial(UserSocial userSocial)
    {
        var iResult = await _userSocialRepository.InsertAsync(userSocial);
        return iResult > 0;
    }

    public abstract Task<SocialTokenResponse> VerifyToken(string socialToken);
}
