using Dapper;
using MC_SG.Lib.Common.Web.Security;
using Mcsg.Identity.Api.DTOs.Response.SSO;
using Mcsg.Identity.Api.Services.Interfaces;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Newtonsoft.Json;

namespace Mcsg.Identity.Api.Services.SSO
{
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
}
