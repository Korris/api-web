namespace Mcsg.Identity.Api.DTOs.Request
{
    public class LoginSocialReq
    {
        public string SocialType { get; set; }
        public string SocialToken { get; set; }
    }
}
