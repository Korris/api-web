namespace Mcsg.Admin.Api.DTOs.Users
{
    public class UserRespone
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string ProfileId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ReferralCode { get; set; }
        public string QRCode { get; set; }//ToDo
        public int Point { get; set; }//ToDo
        public string Avatar { get; set; }
        public string Cover { get; set; }//ToDo
        public string Role { get; set; }
        public string Group { get; set; }//ToDo
        public bool Primary { get; set; }//ToDo
        public bool Verified { get; set; }//ToDo
        public string SocialSSO { get; set; }//ToDo
        public DateOnly? PremiumDate { get; set; }
        public string Location { get; set; }
        public bool IsPremium { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }//ToDo
    }
}
