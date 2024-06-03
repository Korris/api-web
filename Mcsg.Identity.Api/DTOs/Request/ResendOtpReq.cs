using Mcsg.Lib.Data.Enums;

namespace Mcsg.Identity.Api.DTOs.Request
{
    public class ResendOtpReq
    {
        public UserOtpType Type { get; set; }
        public string OtpToken { get; set; }
    }
}
