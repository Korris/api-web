using Mcsg.Lib.Data.Enums;

namespace Mcsg.Identity.Api.DTOs.Request
{
    public class ResetPasswordReq : BaseUserReq
    {
        public UserOtpType Type { get; set; }
        public string Password { get; set; }
        public string RetypePassword { get; set; }
        public string OtpToken { get; set; }
        public string OtpCode { get; set; }
    }
}
