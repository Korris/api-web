namespace Mcsg.Identity.Api.Response
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; }
        public DateTime ExpiredDate { get; set; } = DateTime.UtcNow.AddMinutes(180);
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiredDate { get; set; } = DateTime.UtcNow.AddDays(30);
        public string SubscriptionKey { get; set; }
    }
}
