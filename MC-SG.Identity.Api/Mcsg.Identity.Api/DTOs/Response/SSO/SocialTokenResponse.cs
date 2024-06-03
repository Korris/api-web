using System.Security.Claims;

namespace Mcsg.Identity.Api.DTOs.Response.SSO
{
    public class SocialTokenResponse
    {
        public SocialProfileModel Profile { get; set; }
        public List<Claim> Claims { get; set; }
        public List<string> Error { get; set; }
    }
    public class SocialProfileModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
