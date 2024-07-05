namespace Mcsg.Admin.Api.DTOs.Users
{
    using Common.Core.Enums;

    public class UserStatusReq
    {
        public UserStatus Status { get; set; }
        public string Reason { get; set; }
        public int StatusPeriodDays { get; set; }
    }
}
