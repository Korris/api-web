namespace Mcsg.Api.Areas.Identity.Interfaces;

using Responses;

public interface ISSOService
{
    abstract Task<SocialTokenResponse> VerifyToken(string socialToken);
}
