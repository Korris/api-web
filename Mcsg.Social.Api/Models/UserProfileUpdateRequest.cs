using System.ComponentModel.DataAnnotations;

namespace Mcsg.Social.Api.Models
{
    using Lib.Common.Enums;

    public class UserProfileUpdateRequest
    {
        [Required]
        public string? ProfileName { get; set; }
        public string? UserName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderEnum? Gender { get; set; }
        public string? Location { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
