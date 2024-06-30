namespace Mcsg.Identity.Api.Response
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiredDate { get; set; } = DateTime.UtcNow.AddMinutes(30);
        public DateTime RefreshTokenExpiredDate { get; set; } = DateTime.UtcNow.AddDays(30);
        public string SubscriptionKey { get; set; }
        public string Roles { get; set; }
    }
}
