using Mcsg.Lib.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Mcsg.Social.Api.Models
{
    public class UserProfileUpdateRequest
    {
        [Required]
        public string ProfileName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderEnum? Gender { get; set; }
        public string Location { get; set; }
        public string PhoneNumber { get; set; }

    }
}
