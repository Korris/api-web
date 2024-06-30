using System.Security.Claims;

namespace Mcsg.Identity.Api.Response
{
    public class AppleTokenResponse
    {
        public AppleProfileModel Profile { get; set; }
        public List<Claim> Claims { get; set; }
        public List<string> Error { get; set; }
        public bool IsExists { get; set; }
    }
    public class AppleProfileModel
    {
        public string Sub { get; set; }
        public string Email { get; set; }
        public string Given_name { get; set; }
        public string Family_name { get; set; }
        public string Mobile { get; set; }
    }
    public class ApplePublicKey
    {
        public string Kty { get; set; }
        public string Kid { get; set; }
        public string Alg { get; set; }
        public string N { get; set; }
        public string E { get; set; }
    }
    public class ApplePublicKeysResponse
    {
        public List<ApplePublicKey> Keys { get; set; }
    }
}
