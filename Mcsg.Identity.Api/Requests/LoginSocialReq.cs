namespace Mcsg.Identity.Api.Requests;

using Common.Core.Requests;

public class LoginSocialReq : BaseR
{
    public string SocialType { get; set; }
    public string SocialToken { get; set; }
}
