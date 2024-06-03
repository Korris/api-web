namespace Mcsg.Lib.Common.Models
{
    public class JwtSetting
    {
        public string Key { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string Subject { get; set; }

        public double ExpiredTokenTimeInMinute { get; set; }

        public double RefreshTokenExpiredTimeInDay { get; set; }

        public double ConfirmationTokenExpiredTimeInHour { get; set; }
    }
}
