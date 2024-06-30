namespace Mcsg.Identity.Api.Interfaces
{
    using DTOs.Response;
    using Lib.Data.Domain.Entities;

    public interface ISSOService
    {
        Task<UserSocial> GetUserSocialBySocialId(string socialType, string socialId);
        Task<bool> AddUserSocial(UserSocial userSocial);
        abstract Task<SocialTokenResponse> VerifyToken(string socialToken);
    }
}
