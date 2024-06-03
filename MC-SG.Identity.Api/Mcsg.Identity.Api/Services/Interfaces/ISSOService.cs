using Mcsg.Identity.Api.DTOs.Response.SSO;
using Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Identity.Api.Services.Interfaces
{
    public interface ISSOService
    {
        Task<UserSocial> GetUserSocialBySocialId(string socialType, string socialId);
        Task<bool> AddUserSocial(UserSocial userSocial);
        abstract Task<SocialTokenResponse> VerifyToken(string socialToken);
    }
}
