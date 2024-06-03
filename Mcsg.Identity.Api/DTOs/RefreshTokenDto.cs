namespace Mcsg.Identity.Api.DTOs
{
    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
