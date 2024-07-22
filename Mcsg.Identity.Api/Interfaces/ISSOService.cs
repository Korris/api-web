namespace Mcsg.Identity.Api.Interfaces;

using Common.Domain.Entities;
using Response;

public interface ISSOService
{
    Task<UserSocial> GetUserSocialBySocialId(string socialType, string socialId);
    Task<bool> AddUserSocial(UserSocial userSocial);
    abstract Task<SocialTokenResponse> VerifyToken(string socialToken);
}
