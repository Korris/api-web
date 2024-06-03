using Mcsg.Lib.Model.Enums;

namespace Mcsg.Admin.Api.DTOs.Users
{
    public class UserStatusReq
    {
        public UserStatus Status { get; set; }
        public string Reason { get; set; }
        public int StatusPeriodDays { get; set; }
    }
}
