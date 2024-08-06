namespace Mcsg.Identity.Api.Interfaces;

using Response;

public interface ISSOService
{
    abstract Task<SocialTokenResponse> VerifyToken(string socialToken);
}
