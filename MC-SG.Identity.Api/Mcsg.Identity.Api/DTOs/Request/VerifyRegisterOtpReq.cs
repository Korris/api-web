using Mcsg.Lib.Data.Enums;

namespace Mcsg.Identity.Api.DTOs.Request
{
    public class VerifyRegisterOtpReq : BaseUserReq
    {
        public UserOtpType Type { get; set; }
        public string Otp { get; set; }
        public string OtpToken { get; set; }
    }
}
