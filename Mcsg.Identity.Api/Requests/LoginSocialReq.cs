namespace Mcsg.Identity.Api.Requests;

public class LoginSocialReq
{
    public string SocialType { get; set; }
    public string SocialToken { get; set; }
}
