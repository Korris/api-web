namespace Mcsg.Identity.Api.Requests;

using Common.Core.Requests;

public class AuthenticationLoginSocialR : BaseR
{
    public string SocialType { get; set; }
    public string SocialToken { get; set; }
}
