namespace Mcsg.Admin.Api.DTOs.Users
{
    public class UserAddReq
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsConfirmEmail { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsConfirmPhone { get; set; }
        public string ReferralCode { get; set; }
        public string RoleName { get; set; }
    }
}
