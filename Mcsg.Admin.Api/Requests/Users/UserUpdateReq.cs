namespace Mcsg.Admin.Api.Requests
{
    public class UserUpdateReq
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool? IsConfirmEmail { get; set; }
        public string PhoneNumber { get; set; }
        public bool? IsConfirmPhone { get; set; }

    }
}
