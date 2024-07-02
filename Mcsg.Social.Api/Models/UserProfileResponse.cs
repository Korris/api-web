namespace Mcsg.Social.Api.Models
{
    public class UserProfileAvatarResponse
    {
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string AvatarUrl { get; set; }

    }
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ProfileName { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string CoverPhotoUrl { get; set; }
        public string Location { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string ProfileId { get; set; }
        public DateOnly? PremiumDate { get; set; }
        public bool IsPremium { get; set; }
    }
}
