namespace Mcsg.Identity.Api.Models
{
    public class OtpSetting
    {
        public int ExpiryInMinutes { get; set; }
        public int OtpLength { get; set; }
        public int OtpTokenLength { get; set; }
    }
}
