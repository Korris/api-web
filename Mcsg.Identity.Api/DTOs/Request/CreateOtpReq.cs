using Mcsg.Lib.Data.Enums;

namespace Mcsg.Identity.Api.DTOs.Request
{
    public class CreateOtpReq : BaseUserReq
    {
        public UserOtpType Type { get; set; }
    }
}
