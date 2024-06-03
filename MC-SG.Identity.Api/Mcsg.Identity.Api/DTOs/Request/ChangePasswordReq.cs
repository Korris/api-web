using System.ComponentModel.DataAnnotations;

namespace Mcsg.Identity.Api.DTOs.Request
{
    public class ChangePasswordReq
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}
