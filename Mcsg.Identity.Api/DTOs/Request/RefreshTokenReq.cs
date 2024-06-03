using System.ComponentModel.DataAnnotations;

namespace Mcsg.Identity.Api.DTOs.Request
{
    public class RefreshTokenReq
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
